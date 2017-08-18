using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GobHUDController : MonoBehaviour {

  public Text gobCountText;
  private int gobCount;

  public PlayerController playerController;
	
	private void
  Update()
  {
    if(gobCount != playerController.gobs)
    {
      gobCount = playerController.gobs;
      gobCountText.text = "" + gobCount;
    }
	}
}
