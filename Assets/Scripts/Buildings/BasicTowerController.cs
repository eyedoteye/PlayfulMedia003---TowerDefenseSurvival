using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTowerController : MonoBehaviour
{
  public LayerMask targetLayer;
  public float towerRange;
  public GameObject arrowObject;
  public Vector3 arrowStartOffset;
  public int powerCost;
  public int gobCost;

  private bool attackReady = false;

	void Start()
  {
    IEnumerator attackCooldownCoroutine = AttackCooldown();
    StartCoroutine(attackCooldownCoroutine);
	}
	
	void Update()
  {
    if(attackReady)
    {
      EnemyController enemy = GetEnemyWithLowestHealth();
      if(enemy != null) 
        FireArrowAt(enemy.gameObject);
      
      attackReady = false;
    }
	}

  private void FireArrowAt(GameObject enemy)
  {
    Vector3 vectorFromTowerToEnemy = enemy.transform.position - transform.position;
    Quaternion arrowRotation = Quaternion.LookRotation(vectorFromTowerToEnemy);
    Vector3 arrowStartPosition = arrowRotation * arrowStartOffset + transform.position;
    GameObject arrow = Instantiate<GameObject>(arrowObject, arrowStartPosition, arrowRotation);
    arrow.GetComponent<ArrowController>().targetObject = enemy;
  }

  IEnumerator AttackCooldown()
  {
    for(;;)
    {
      if(attackReady)
        yield return null;
      else
      {
        yield return new WaitForSeconds(3);
        attackReady = true;
      }
    }
  }

  private EnemyController GetEnemyWithLowestHealth()
  {
    Collider[] collisionsWithTargetsInRange = Physics.OverlapCapsule(
      this.transform.position,
      this.transform.position + this.transform.up * 10f,
      towerRange,
      targetLayer);

    EnemyController enemyWithLeastHealth = null;
      
    foreach(Collider collisionWithTarget in collisionsWithTargetsInRange)
    {
      EnemyController enemy = collisionWithTarget.gameObject.GetComponent<EnemyController>();
      if(enemy == null || enemy.isDead)
        continue;

      if(
        enemyWithLeastHealth == null
        || enemyWithLeastHealth.health > enemy.health)
      {
        enemyWithLeastHealth = enemy;
      }
    }

    return enemyWithLeastHealth;
  }
}
