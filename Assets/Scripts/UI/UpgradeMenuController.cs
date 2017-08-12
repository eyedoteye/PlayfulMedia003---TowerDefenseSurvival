using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
  public PlayerController playerController; 
  public BasicTowerController basicTowerController;
  public PowerHeadController powerHeadController;
	

  private GameObject selectedUpgrade;

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
    }

    Debug.Log("Selected upgrade:" + selectedUpgrade);
  }
}
