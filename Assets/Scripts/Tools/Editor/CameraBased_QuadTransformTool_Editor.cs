﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraBased_QuadTransformTool))]
public class CameraBased_QuadTransformTool_Editor : Editor
{
  private CameraBased_QuadTransformTool cameraBased_QuadTransformTool;


  private SerializedProperty targetObjectProperty;
  private SerializedProperty targetCameraProperty;

  private SerializedProperty pixelGridSizeProperty;
  private SerializedProperty gridEnabledProperty;

  private SerializedProperty showGridProperty;

  private const string targetObjectPropertyName = "targetObject";
  private const string targetCameraPropertyName = "targetCamera";

  private const string screenPositionPropertyName = "screenPosition"; 
  private const string relativeDistancePropertyName = "relativeDistance";

  private const string pixelGridSizePropertyName = "pixelGridSize";
  private const string gridEnabledPropertyName = "gridEnabled";

  private GUIContent gridEnabledPropertyLabel = new GUIContent("Show Grid");

  private bool isFirstAttach = true;

  private float RoundForInput(float number)
  {
    return (int)(number * 100f + 0.5f) / 100f;
  }

  private void OnFirstAttach()
  {
    if(isFirstAttach)
    {
      GameObject targetObject = cameraBased_QuadTransformTool.gameObject;
      Camera targetCamera = targetObject.GetComponent<Camera>();
      if(targetCamera != null)
        cameraBased_QuadTransformTool.targetCamera = targetCamera;
      else
        cameraBased_QuadTransformTool.targetObject = targetObject;

    }
    isFirstAttach = false;
  }

  public void UndoMorph()
  {
    if(Event.current.commandName.Equals("UndoRedoPerformed"))
    {
      cameraBased_QuadTransformTool.Cache_Mesh_Into_MeshVertices();
      cameraBased_QuadTransformTool.Apply_MeshVertices_To_TargetMesh();
    }
  }

  private void OnEnable()
  {
    cameraBased_QuadTransformTool = (CameraBased_QuadTransformTool)target;
    OnFirstAttach();

    targetObjectProperty = serializedObject.FindProperty(targetObjectPropertyName);
    targetCameraProperty = serializedObject.FindProperty(targetCameraPropertyName);

    pixelGridSizeProperty = serializedObject.FindProperty(pixelGridSizePropertyName);
    gridEnabledProperty = serializedObject.FindProperty(gridEnabledPropertyName);
  }

  private void UpdateVectorCache()
  {
  }

  private void ApplyVectorCache()
  {
  }

  bool meshMorphed = false;
  public void Build_MeshVertices_Editors()
  {
    if(cameraBased_QuadTransformTool.targetMesh == null)
      return;
    if(cameraBased_QuadTransformTool.meshVertices == null)
      return;
    
    for(
      int vertexIndex = 0;
      vertexIndex < cameraBased_QuadTransformTool.meshVertices.Length;
      ++vertexIndex)
    {
      CameraBased_QuadTransformTool.MeshVertex meshVertex =
        cameraBased_QuadTransformTool.meshVertices[vertexIndex];
      CameraBased_QuadTransformTool.MeshVertex.RelativeToCamera relativeToCamera =
        meshVertex.relativeToCamera;

      relativeToCamera.screenPosition.x = RoundForInput(relativeToCamera.screenPosition.x);
      relativeToCamera.screenPosition.y = RoundForInput(relativeToCamera.screenPosition.y);
      relativeToCamera.distance = RoundForInput(relativeToCamera.distance);

      EditorGUILayout.LabelField("Vertex " + vertexIndex);

      Vector2 pixelGridSize = cameraBased_QuadTransformTool.pixelGridSize;
      Vector2 gridPosition = relativeToCamera.screenPosition;
      gridPosition.x /= pixelGridSize.x;
      gridPosition.y /= pixelGridSize.y;

      gridPosition.x = RoundForInput(gridPosition.x);
      gridPosition.y = RoundForInput(gridPosition.y);

      gridPosition = EditorGUILayout.Vector2Field(
        " ↑ Grid Position",
        gridPosition);

      relativeToCamera.distance = EditorGUILayout.FloatField(
        " ↑ Distance From Camera",
        relativeToCamera.distance);

      relativeToCamera.screenPosition.x = gridPosition.x * pixelGridSize.x;
      relativeToCamera.screenPosition.y = gridPosition.y * pixelGridSize.y;

      CameraBased_QuadTransformTool.MeshVertex.RelativeToCamera oldRelativeToCamera =
        meshVertex.relativeToCamera;
      oldRelativeToCamera.screenPosition.x = RoundForInput(oldRelativeToCamera.screenPosition.x);
      oldRelativeToCamera.screenPosition.y = RoundForInput(oldRelativeToCamera.screenPosition.y);
      oldRelativeToCamera.distance = RoundForInput(oldRelativeToCamera.distance);

      if(
        oldRelativeToCamera.screenPosition != relativeToCamera.screenPosition ||
        oldRelativeToCamera.distance != relativeToCamera.distance)
      {
        meshMorphed = true;
        cameraBased_QuadTransformTool.meshVertices[vertexIndex].relativeToCamera =
          relativeToCamera;

        meshVertex.Apply_RelativeToCamera();
      }
    }
  }

  public override void OnInspectorGUI()
  {
    UndoMorph();

    serializedObject.Update();
    UpdateVectorCache();

    EditorGUILayout.PropertyField(targetObjectProperty);
    EditorGUILayout.PropertyField(targetCameraProperty);
    if(
      targetObjectProperty.objectReferenceValue != cameraBased_QuadTransformTool.targetObject 
      || targetCameraProperty.objectReferenceValue != cameraBased_QuadTransformTool.targetCamera)
    {
      serializedObject.ApplyModifiedProperties();

      if(
        cameraBased_QuadTransformTool.targetObject == null
        || cameraBased_QuadTransformTool.targetCamera == null)
      {
        cameraBased_QuadTransformTool.ClearMesh();
      }
      else
      {
        cameraBased_QuadTransformTool.GetMesh();
        cameraBased_QuadTransformTool.Cache_Mesh_Into_MeshVertices();
      }
    }

    EditorGUILayout.PropertyField(pixelGridSizeProperty);
    EditorGUILayout.PropertyField(gridEnabledProperty, gridEnabledPropertyLabel);

    if(
      cameraBased_QuadTransformTool.targetObject != null
      && cameraBased_QuadTransformTool.targetCamera != null)
    {
      Vector3 targetObjectPosition = 
        cameraBased_QuadTransformTool.targetCamera.WorldToScreenPoint(
        cameraBased_QuadTransformTool.targetObject.transform.position);
      Vector2 pixelGridSize = pixelGridSizeProperty.vector2Value;

      targetObjectPosition.x = RoundForInput(targetObjectPosition.x);
      targetObjectPosition.y = RoundForInput(targetObjectPosition.y);
      targetObjectPosition.z = RoundForInput(targetObjectPosition.z);

      Vector2 targetObjectGridPosition = targetObjectPosition;
      targetObjectGridPosition.x /= pixelGridSize.x;
      targetObjectGridPosition.y /= pixelGridSize.y;

      Vector2 newTargetObjectGridPosition = EditorGUILayout.Vector2Field(
        "Object Grid Position",
        targetObjectGridPosition);
      float newTargetRelativeDistance = EditorGUILayout.FloatField(
        "Object Relative Distance",
        targetObjectPosition.z);

      Vector3 newTargetObjectPosition;

      newTargetObjectPosition.x = newTargetObjectGridPosition.x * pixelGridSize.x;
      newTargetObjectPosition.y = newTargetObjectGridPosition.y * pixelGridSize.y;
      newTargetObjectPosition.z = newTargetRelativeDistance;
      newTargetObjectPosition =
        cameraBased_QuadTransformTool.targetCamera.ScreenToWorldPoint(
        newTargetObjectPosition);

      if(Vector3.Distance(newTargetObjectPosition,
        cameraBased_QuadTransformTool.targetObject.transform.position)
        > 0.001f)
      {
        Undo.RecordObject(
          cameraBased_QuadTransformTool.targetObject.transform,
          "Transform Object Position");
        cameraBased_QuadTransformTool.targetObject.transform.position
          = newTargetObjectPosition;
        cameraBased_QuadTransformTool.GetMesh();
        cameraBased_QuadTransformTool.Cache_Mesh_Into_MeshVertices();
      }

      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Recenter Mesh Vertices"))
      {
        Undo.RecordObject(
          cameraBased_QuadTransformTool.targetMesh,
          "Recenter Mesh Vertices");
        cameraBased_QuadTransformTool.Recenter();
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Save Mesh"))
      {
        Mesh targetMesh = cameraBased_QuadTransformTool.targetMesh;
        string savePath = EditorUtility.SaveFilePanelInProject(
          "Save Mesh",
          targetMesh.name + "Mesh", "asset",
          "Please enter the coolest filename you can think of.");
        AssetDatabase.CreateAsset(
          targetMesh, 
          savePath);
        AssetDatabase.SaveAssets();
      }
      EditorGUILayout.EndHorizontal();
    }
    Build_MeshVertices_Editors();

    ApplyVectorCache();
    serializedObject.ApplyModifiedProperties();
    
    if(meshMorphed)
    {
      Undo.RecordObject(
        cameraBased_QuadTransformTool.targetMesh,
        "Transform Mesh Vertex");
      cameraBased_QuadTransformTool.Apply_MeshVertices_To_TargetMesh();
      meshMorphed = false;
    }
  }
}
