using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
  public PlayerController playerController; 
  public TowerController basicTowerController;
  public PowerHeadController powerHeadController;
  public WorldController worldController;

  public float menuRadius;
  public Vector2 screenOffset;

  public GameObject[] upgrades;

  private GameObject selectedUpgrade;

  public struct UpgradesTransformBuffer{
    public Vector3 intoScreenAxis;
    public Vector3 basePosition;
    public Vector3 localOffset;
  };

  private void Update()
  {
    UpgradesTransformBuffer upgradesTransformBuffer =
      GetUpgradesTransformBuffer();

    for(
      int upgradeIndex = 0;
      upgradeIndex < upgrades.Length;
      ++upgradeIndex)
    {
      Vector3 upgradePosition = GetUpgradePosition(
        upgradeIndex, upgrades.Length,
        upgradesTransformBuffer.intoScreenAxis,
        upgradesTransformBuffer.basePosition,
        upgradesTransformBuffer.localOffset);

      RectTransform upgradeRectTransform =
        upgrades[upgradeIndex].GetComponent<RectTransform>();
      upgradeRectTransform.position = upgradePosition;
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;

    UpgradesTransformBuffer upgradesTransformBuffer =
      GetUpgradesTransformBuffer();

    for(
      int upgradeIndex = 0;
      upgradeIndex < upgrades.Length;
      ++upgradeIndex)
    {
      Vector3 upgradePosition = GetUpgradePosition(
        upgradeIndex, upgrades.Length,
        upgradesTransformBuffer.intoScreenAxis,
        upgradesTransformBuffer.basePosition,
        upgradesTransformBuffer.localOffset);

      Vector3 menuCenter = transform.position;
      menuCenter.x += upgradesTransformBuffer.localOffset.x;
      menuCenter.y += upgradesTransformBuffer.localOffset.y;

      Gizmos.DrawLine(menuCenter, upgradePosition);
      Gizmos.DrawWireSphere(upgradePosition, 1f);
    }
  }

  public UpgradesTransformBuffer GetUpgradesTransformBuffer()
  {
    UpgradesTransformBuffer upgradesTransformBuffer;

    upgradesTransformBuffer.intoScreenAxis = new Vector3(0f, 0f, 1f);
    upgradesTransformBuffer.basePosition = new Vector3(0f, menuRadius);
    upgradesTransformBuffer.localOffset = new Vector3(
      screenOffset.x, screenOffset.y, 0f);

    return upgradesTransformBuffer;
  }

  public Vector3 GetUpgradePosition(
    int index, int maxIndices,
    Vector3 intoScreenAxis, Vector3 basePosition,
    Vector3 localOffset)
  {
    float radialOffset = 0;
    if(maxIndices == 2)
      radialOffset = 90;

    Quaternion rotation = Quaternion.AngleAxis(
      index / (float)maxIndices * 360 + radialOffset,
      intoScreenAxis);

    Vector3 upgradePosition =
      rotation * basePosition
      + localOffset + transform.position;

    return upgradePosition;
  }

  public Vector3 GetMenuRadiusHandleWorldPoint()
  {
    Vector3 menuRadiusHandleWorldPoint = transform.position;
    menuRadiusHandleWorldPoint.y += menuRadius;
//    menuRadiusHandleWorldPoint.x += screenOffset.x;
//    menuRadiusHandleWorldPoint.y += screenOffset.y;

    return menuRadiusHandleWorldPoint;
  }

  public void SetMenuRadiusHandleWorldPoint(
    Vector3 menuRadiusHandleWorldPoint)
  {
//    menuRadiusHandleWorldPoint.y -= screenOffset.y;
    menuRadiusHandleWorldPoint.y -= transform.position.y;

    menuRadius = menuRadiusHandleWorldPoint.y;
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
      worldController.UpdateAllTowers(basicTowerController);  
    }

    Debug.Log("Selected upgrade:" + selectedUpgrade);
  }
}
