using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRangeController : MonoBehaviour
{
  private void
  OnCollisionEnter(Collision collision)
  {
    GameObject other = collision.gameObject;

    if(other.CompareTag("Building") || other.CompareTag("Player"))
    {
      EnemyController parent =
        transform.parent.gameObject.GetComponent<EnemyController>();

      if(parent.attackTarget != other.transform.parent.gameObject)
        return;

      if(!parent.dead && !parent.animator.GetBool("isHit"))
        parent.animator.SetBool("isAttacking", true);
    }
  }
}
