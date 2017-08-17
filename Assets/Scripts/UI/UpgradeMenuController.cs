using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
  public PlayerController playerController; 
  public TowerController basicTowerController;
  public PowerHeadController powerHeadController;
  public WorldController worldController;

  public float menuRadialDistance;
  public float radialOffset;
  public Vector2 screenOffset;

  public GameObject[] upgrades;

  private GameObject selectedUpgrade;

  //Note: Replace with Awake() on Release
  private void Awake()
  {
    Vector3 intoScreen_Axis = new Vector3(0f, 0f, 1f);
    Vector3 basePosition = new Vector3(0f, menuRadialDistance);
    Vector3 localOffset = new Vector3(screenOffset.x, screenOffset.y, 0f); 

    for(
      int upgradeIndex = 0;
      upgradeIndex < upgrades.Length;
      ++upgradeIndex)
    {
      RectTransform upgrade_RectTransform = upgrades[upgradeIndex].GetComponent<RectTransform>();
      Quaternion rotation = Quaternion.AngleAxis(
        upgradeIndex / (float)upgrades.Length * 360 + radialOffset,
        intoScreen_Axis);
      Vector3 upgradePosition = rotation * basePosition + localOffset;
      upgrade_RectTransform.localPosition = upgradePosition;
    }
  }

  public void selectUpgrade(GameObject upgrade)
  {
    if(selectedUpgrade != null)
      selectedUpgrade.GetComponent<Image>().color = Color.white;

    selectedUpgrade = upgrade;
    selectedUpgrade.GetComponent<Image>().color = Color.green;
  }

  public void unselectUpgrade(GameObject upgrade)
  {
    if(selectedUpgrade == upgrade)
      selectedUpgrade.GetComponent<Image>().color = Color.white;

    selectedUpgrade = null;
  }

  public void confirmUpgrade()
  {
    if(selectedUpgrade == null)
      return;

    if(playerController.gobs >= 1)
      playerController.gobs--;
    else
      return;

    if(selectedUpgrade.name == "UpgradePower")
    {
      powerHeadController.maxPower += 1;
      powerHeadController.availablePower += 1;
    }
    else if(selectedUpgrade.name == "UpgradeTowers")
    {
      basicTowerController.Upgrade();
      worldController.update_all_towers(basicTowerController);  
    }

    Debug.Log("Selected upgrade:" + selectedUpgrade);
  }
}
