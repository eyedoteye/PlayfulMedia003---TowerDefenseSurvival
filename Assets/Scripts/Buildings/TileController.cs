using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
TileController : MonoBehaviour
{
  public LayerMask buildingLayerMask;

  public int column, row;
  public bool blocked;
  public GameObject attachedBuilding;

  public void
  DestroyIfCollidingWithBuilding()
  {
    Debug.Log(buildingLayerMask);
    if(
      Physics.CheckSphere(
        transform.position,
        0.5f,
        buildingLayerMask))
    {
      Destroy(gameObject);
    }
  }
}
