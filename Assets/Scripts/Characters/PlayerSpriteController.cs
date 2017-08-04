using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
  public PlayerController parent;

  public void ResetAnimationBools()
  {
    parent.ResetAnimationBools();
  }
}
