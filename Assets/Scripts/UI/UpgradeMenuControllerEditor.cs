using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeMenuController)), CanEditMultipleObjects]
public class UpgradeMenuControllerEditor : Editor
{
  private static Vector3 pointSnap = Vector3.one * 0.1f;

  private void OnSceneGUI()
  {
    UpgradeMenuController upgradeMenuController =
      target as UpgradeMenuController;

    Vector3 menuCenter = upgradeMenuController.transform.position;

    float crossScreenSize = 
      HandleUtility.GetHandleSize(menuCenter) * 0.1f;

    Handles.DrawLine(
      menuCenter
      + new Vector3(-crossScreenSize, crossScreenSize),
      menuCenter
      + new Vector3(crossScreenSize, -crossScreenSize));
    Handles.DrawLine(
      menuCenter
      + new Vector3(-crossScreenSize, -crossScreenSize),
      menuCenter
      + new Vector3(crossScreenSize, crossScreenSize)); 

    Vector3 oldMenuRadiusHandleWorldPoint =
      upgradeMenuController.GetMenuRadiusHandleWorldPoint();

    Handles.DrawDottedLine(
      menuCenter,
      oldMenuRadiusHandleWorldPoint,
      -4.0f); 

    float handleScreenSize = 
      HandleUtility.GetHandleSize(oldMenuRadiusHandleWorldPoint) * 0.1f;

    Vector3 newMenuRadiusHandleWorldPoint = Handles.FreeMoveHandle(
      oldMenuRadiusHandleWorldPoint, Quaternion.identity,
      handleScreenSize,
      pointSnap,
      Handles.DotHandleCap);

    if(oldMenuRadiusHandleWorldPoint != newMenuRadiusHandleWorldPoint)
    {
      upgradeMenuController.SetMenuRadiusHandleWorldPoint(
        newMenuRadiusHandleWorldPoint);
    }
  }
}
