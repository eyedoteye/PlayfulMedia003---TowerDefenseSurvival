using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyHUDController : MonoBehaviour
{
  public int blipMax;
  public Sprite[] blipSprites;
  public PowerHeadController powerHeadController;

  private GameObject[] blips;
  private int blipCount;

  private void Start()
  {
    GetComponent<SpriteRenderer>().enabled = false;
    blips = new GameObject[blipMax];
    for(int blipIndex = 0; blipIndex < blipMax; ++blipIndex)
    {
      blips[blipIndex] = new GameObject();
      blips[blipIndex].name = "Energy Blip " + blipIndex;
      blips[blipIndex].layer = LayerMask.NameToLayer("UI");
      blips[blipIndex].transform.parent = transform;
      blips[blipIndex].transform.position = transform.position;
      blips[blipIndex].transform.localPosition = new Vector3(
        blipIndex * -0.18f,
        0f,
        0f);
      blips[blipIndex].transform.localRotation = Quaternion.identity;
      blips[blipIndex].transform.localScale = Vector3.one;
      blips[blipIndex].AddComponent<SpriteRenderer>().enabled = false;
    }
    blipCount = 0;
  }

  private void Update()
  {
    int blipDiff = powerHeadController.availablePower - blipCount;
    if(blipDiff < 0)
      RemoveBlips(-blipDiff);
    else if(blipDiff > 0)
      AddBlips(blipDiff);
  }

  private void AddBlips(int count)
  {
    for(int iterator = 0; iterator < count; ++iterator)
    {
      AddBlip();
    }
  }

  private void RemoveBlips(int count)
  {
    for(int iterator = 0; iterator < count; ++iterator)
    {
      RemoveBlip();
    }
  }

  private void AddBlip()
  {
    SpriteRenderer spriteRenderer = blips[blipCount].GetComponent<SpriteRenderer>();
    
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

  private void RemoveBlip()
  {
    blipCount--;
    blips[blipCount].GetComponent<SpriteRenderer>().enabled = false;
  }


}
