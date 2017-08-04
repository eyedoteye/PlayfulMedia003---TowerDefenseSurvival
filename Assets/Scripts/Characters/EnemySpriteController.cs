using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteController : MonoBehaviour
{
  public EnemyController parent;

  public void MeleeTarget()
  {
    parent.MeleeTarget();
  }

  public void ResetAnimationBools()
  {
    parent.ResetAnimationBools();
  }

  public void Unexist()
  {
    parent.Unexist();
  }
}
