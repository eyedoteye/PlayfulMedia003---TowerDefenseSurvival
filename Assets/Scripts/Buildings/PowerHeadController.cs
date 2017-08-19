using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
PowerHeadController : MonoBehaviour
{
  public int availablePower;
  public int maxPower;

  public Canvas upgradeMenu;
  public LayerMask buildingLayerMask;
  public MouseController mouseController;

  private void
  Start()
  {
    upgradeMenu.enabled = false;
  }

  private void
  Update()
  {
    if(Input.GetMouseButton(0))
    {
      RaycastHit hit;
      if(
        Physics.Raycast(
          Camera.main.ScreenPointToRay(Input.mousePosition),
          out hit,
          300f,
          buildingLayerMask))
      {
        if(hit.transform.gameObject.Equals(gameObject))
        {
          upgradeMenu.enabled = true;
          mouseController.mouseInUse = true;
          mouseController.controlMode = MouseController.ControlMode.UpgradeMenu;
        }
      }
    }
    else
    {
      if(
        Input.GetMouseButtonUp(0)
        && mouseController.controlMode == MouseController.ControlMode.UpgradeMenu)
      {
        upgradeMenu.GetComponent<UpgradeMenuController>().confirmUpgrade();
        upgradeMenu.enabled = false;
        mouseController.mouseInUse = false;
        mouseController.controlMode = MouseController.ControlMode.None;
      }
    }
  }
}
