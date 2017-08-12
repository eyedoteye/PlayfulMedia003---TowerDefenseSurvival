using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
  public GameObject selectedUpgrade;
	
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
    if(selectedUpgrade != null)
      Debug.Log("Selected upgrade:" + selectedUpgrade);
  }
}
