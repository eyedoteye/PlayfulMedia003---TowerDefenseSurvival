using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class
EnergyHUDController : MonoBehaviour
{
  public Sprite[] blipSprites;
  public PowerHeadController powerHeadController;
  public GameObject energyBlip_Base;

  private GameObject[] blips;
  private int blipCount;
  private int blipMax = 20;

  private void
  Start()
  {
    GetComponent<Image>().enabled = false;

    blips = new GameObject[blipMax];
    for(int blipIndex = 0; blipIndex < blipMax; ++blipIndex)
    {
      blips[blipIndex] = Instantiate<GameObject>(energyBlip_Base);
      blips[blipIndex].name = "Energy Blip " + blipIndex;
      blips[blipIndex].layer = LayerMask.NameToLayer("UI");
      blips[blipIndex].transform.SetParent(transform, false);
      blips[blipIndex].transform.position = transform.position;
      blips[blipIndex].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(
        blipIndex * -16,
        0f,
        0f);
      blips[blipIndex].transform.localRotation = Quaternion.identity;
      blips[blipIndex].GetComponent<Image>().enabled = false;
    }
    blipCount = 0;
  }

  private void
  Update()
  {
    int blipDiff = powerHeadController.availablePower - blipCount;

    if(blipDiff < 0)
      RemoveBlips(-blipDiff);
    else if(blipDiff > 0)
      AddBlips(blipDiff);
  }

  private void
  AddBlips(int count)
  {
    for(int iterator = 0; iterator < count; ++iterator)
      AddBlip();
  }

  private void
  RemoveBlips(int count)
  {
    for(int iterator = 0; iterator < count; ++iterator)
    {
      RemoveBlip();
    }
  }

  private void
  AddBlip()
  {
    Image spriteRenderer = blips[blipCount].GetComponent<Image>();
    
    switch((int)(Random.value * 3))
    {
      case 0:
      {
        spriteRenderer.sprite = blipSprites[0];
      } break;
      case 1:
      {
        spriteRenderer.sprite = blipSprites[1];
      } break;
      case 2:
      {
        spriteRenderer.sprite = blipSprites[2];
      } break;
    }

    spriteRenderer.enabled = true;
    blipCount++;
  }

  private void
  RemoveBlip()
  {
    blipCount--;
    blips[blipCount].GetComponent<Image>().enabled = false;
  }
}
