using UnityEngine;

public class CameraBased_QuadTransformTool : MonoBehaviour {

  public class MeshVertex
  {
    public GameObject targetObject;
    public Mesh targetMesh;
    public int vertexIndex;

    // Note: Stored In Local Coordinates
    public Vector3 vertex;

    // Note: Strored In Screen Coordinates
    public struct RelativeToCamera
    {
      public Vector2 screenPosition;
      public float distance;
    } public RelativeToCamera relativeToCamera;

    public Camera targetCamera;

    public bool Compute_RelativeToCamera()
    {
      if(targetCamera == null)
        return false;

      Vector3 worldSpace = targetObject.transform.localToWorldMatrix.MultiplyPoint3x4(vertex);
      Vector3 screenSpace = targetCamera.WorldToScreenPoint(worldSpace);
      relativeToCamera.screenPosition = new Vector2(screenSpace.x, screenSpace.y);
      relativeToCamera.distance = screenSpace.z;

      return true;
    }

    public bool Apply_RelativeToCamera()
    {
      if(targetCamera == null)
        return false;

      Vector3 worldSpace = targetCamera.ScreenToWorldPoint(new Vector3(
        relativeToCamera.screenPosition.x,
        relativeToCamera.screenPosition.y,
        relativeToCamera.distance));

      vertex =
        targetObject.transform.worldToLocalMatrix.MultiplyPoint3x4(worldSpace);

      return true;
    }

    public bool Recache()
    {
      if(targetObject == null)
        return false;

      vertex = targetMesh.vertices[vertexIndex];

      return Compute_RelativeToCamera();
    }
  }

  public GameObject targetObject;
  public Camera targetCamera;

  public Vector2 pixelGridSize = new Vector2(16f, 16f);
  public bool gridEnabled = true;

  public MeshVertex[] meshVertices;
  public Mesh targetMesh;

  private Vector2 cached_ScreenDimensions;
  private float cached_relativeDistance;

  private Color gridColor = new Color(1f, 1f, 0f, 0.5f);

  private Vector3 rotatePositionOverPoint(
    Vector3 position,
    Vector3 point,
    Quaternion rotation,
    Vector3 forward)
  {
    position = position - point;
    position = position - forward;
    position = rotation * position;
    position = position + forward;
    position = position + point;

    return position;
  }

  public void ClearMesh()
  {
    targetMesh = null;
    meshVertices = null;
  }

  public bool IsMeshInstantiated()
  {
    if(targetMesh == null)
      return false;

    MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
    if(meshFilter == null)
      return false;

    Mesh possible_targetMesh = meshFilter.sharedMesh;
    if(possible_targetMesh.name.Contains("Instance"))
      return true;

    return false;
  }

  public bool Recenter()
  {
    if(targetObject == null)
      return false;
    if(targetMesh == null)
      return false;

    Vector3 targetScreenPosition = targetCamera.WorldToScreenPoint(
      targetObject.transform.position);

    Vector3 mesh_ScreenCenter = new Vector3(0f, 0f, 0f);

    for(int vertexIndex = 0; vertexIndex < meshVertices.Length; ++vertexIndex)
    {
      MeshVertex.RelativeToCamera relativeToCamera = meshVertices[vertexIndex].relativeToCamera;
      mesh_ScreenCenter.x += relativeToCamera.screenPosition.x;
      mesh_ScreenCenter.y += relativeToCamera.screenPosition.y;
      mesh_ScreenCenter.z += relativeToCamera.distance;
    }
    mesh_ScreenCenter.x /= meshVertices.Length;
    mesh_ScreenCenter.y /= meshVertices.Length;
    mesh_ScreenCenter.z /= meshVertices.Length;

    Vector3 offset = targetScreenPosition - mesh_ScreenCenter;
    
    for(int vertexIndex = 0; vertexIndex < meshVertices.Length; ++vertexIndex)
    {
      MeshVertex.RelativeToCamera relativeToCamera = meshVertices[vertexIndex].relativeToCamera;
      relativeToCamera.screenPosition.x += offset.x;
      relativeToCamera.screenPosition.y += offset.y;
      relativeToCamera.distance += offset.z;

      meshVertices[vertexIndex].relativeToCamera = relativeToCamera;
      meshVertices[vertexIndex].Apply_RelativeToCamera();
    }

    Apply_MeshVertices_To_TargetMesh();

    return true;
  }

  public bool GetMesh()
  {
    targetMesh = null;

    if(targetObject == null)
      return false;

    MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
    if(meshFilter == null)
      return false;

    Mesh possible_targetMesh = meshFilter.sharedMesh;

    if(possible_targetMesh.name.Contains("Instance"))
      targetMesh = possible_targetMesh;
    else
    {
      targetMesh = Mesh.Instantiate(possible_targetMesh);
      targetMesh.name = targetMesh.name + " Instance";
      targetObject.GetComponent<MeshFilter>().mesh = targetMesh;
    }
    
    if(targetMesh == null)
      return false;

    return true;
  }

  public void Apply_MeshVertices_To_TargetMesh()
  {
    Vector3[] vertices = new Vector3[meshVertices.Length];
    for(int vertexIndex = 0; vertexIndex < meshVertices.Length; ++vertexIndex)
    {
      vertices[vertexIndex] = meshVertices[vertexIndex].vertex;
    }
    targetMesh.vertices = vertices;
  }

  public bool Cache_Mesh_Into_MeshVertices()
  {
    if(targetMesh == null)
      return false;

    Vector3[] vertices = targetMesh.vertices;
    meshVertices = new MeshVertex[vertices.Length];
    for(int vertexIndex = 0; vertexIndex < targetMesh.vertices.Length; ++vertexIndex)
    {
      MeshVertex meshVertex = new MeshVertex();

      meshVertex.targetObject = targetObject;
      meshVertex.targetMesh = targetMesh;
      meshVertex.vertexIndex = vertexIndex;
      meshVertex.vertex = vertices[vertexIndex];
      meshVertex.targetCamera = targetCamera;
      meshVertex.Compute_RelativeToCamera();

      meshVertices[vertexIndex] = meshVertex;
    }

    return true;
  }

  public void UpdateGizmoCache()
  {
    if(targetCamera == null)
      return;
    if(targetObject == null)
      return;

    cached_relativeDistance = Vector3.Distance(
      targetCamera.transform.position,
      targetObject.transform.position);

    cached_ScreenDimensions = targetCamera.ViewportToScreenPoint(
      new Vector3(1f, 1f));
  }

  public void OnDrawGizmosSelected()
  {
    if(targetMesh != null)
    {
      if(gridEnabled)
        DrawGrid();

      DrawCross();
    }
  }

  private void DrawCross()
  {
    Vector2 objectMid = targetCamera.WorldToScreenPoint(targetObject.transform.position);
    Vector2 crossOffset = pixelGridSize / 2;
    Vector3 crossTopLeft = targetCamera.ScreenToWorldPoint(new Vector3(
      objectMid.x - crossOffset.x,
      objectMid.y + crossOffset.y,
      cached_relativeDistance));
    Vector3 crossBotRight = targetCamera.ScreenToWorldPoint(new Vector3(
      objectMid.x + crossOffset.x,
      objectMid.y - crossOffset.y,
      cached_relativeDistance));
    Vector3 crossBotLeft = targetCamera.ScreenToWorldPoint(new Vector3(
      objectMid.x - crossOffset.x,
      objectMid.y - crossOffset.y,
      cached_relativeDistance));
    Vector3 crossTopRight = targetCamera.ScreenToWorldPoint(new Vector3(
      objectMid.x + crossOffset.x,
      objectMid.y + crossOffset.y,
      cached_relativeDistance));
    
    Gizmos.DrawLine(crossTopLeft, crossBotRight);
    Gizmos.DrawLine(crossBotLeft, crossTopRight);
  }

  private void DrawGrid()
  {
    UpdateGizmoCache();

    if((int)pixelGridSize.x > 0)
    {
      float currentScreenXPosition = 0;

      while(currentScreenXPosition <= cached_ScreenDimensions.x)
      {
        Vector3 botPosition = targetCamera.ScreenToWorldPoint(new Vector3(
          currentScreenXPosition, 0f, cached_relativeDistance));
        Vector3 topPosition = targetCamera.ScreenToWorldPoint(new Vector3(
          currentScreenXPosition,
          cached_ScreenDimensions.y,
          cached_relativeDistance));
        Debug.DrawLine(
          botPosition, topPosition,
          gridColor,
          0,
          false);

        currentScreenXPosition += pixelGridSize.x;
      }
    }

    if((int)pixelGridSize.y > 0)
    {
      float currentScreenYPosition = 0;

      while(currentScreenYPosition <= cached_ScreenDimensions.y)
      {
        Vector3 leftPosition = targetCamera.ScreenToWorldPoint(new Vector3(
          0f, currentScreenYPosition, cached_relativeDistance));
        Vector3 rightPosition = targetCamera.ScreenToWorldPoint(new Vector3(
          cached_ScreenDimensions.x,
          currentScreenYPosition,
          cached_relativeDistance));
        Debug.DrawLine(
          leftPosition, rightPosition,
          gridColor,
          0,
          false);

        currentScreenYPosition += pixelGridSize.y;
      }
    }
  }
}
