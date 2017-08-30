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
      Undo.RecordObject(upgradeMenuController, "Update Upgrade Position");
      upgradeMenuController.SetMenuRadiusHandleWorldPoint(
        newMenuRadiusHandleWorldPoint);

      GameObject[] upgrades = upgradeMenuController.upgrades;

      for(
        int upgradeIndex = 0;
        upgradeIndex < upgrades.Length;
        ++upgradeIndex)
      {
        GameObject upgrade = upgrades[upgradeIndex];

        if(upgrade == null)
          continue;

        RectTransform upgradeRectTransform =
          upgrades[upgradeIndex].GetComponent<RectTransform>();
      
        Undo.RecordObject(upgradeRectTransform, "Update Upgrade Position");
      }

      upgradeMenuController.UpdateUpgradePositions();
    }
  }
}
