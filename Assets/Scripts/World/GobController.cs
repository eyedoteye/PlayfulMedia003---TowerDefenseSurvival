using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
GobController : MonoBehaviour
{
  private void
  OnTriggerEnter(Collider other)
  {
    PlayerController playerController = other.GetComponent<PlayerController>();
    if(playerController != null)
    {
      playerController.gobs++;
      Destroy(gameObject);
    }
  }
}
