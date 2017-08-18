using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
TowerController : MonoBehaviour
{
  public int powerCost;
  public int gobCost;
  public float towerRange;
  public float damage = .9f;
  public float attackCooldown = 3f;
  public Color color = Color.white;

  public LayerMask targetLayer;
  public GameObject arrowObject;
  public Vector3 arrowStartOffset;

  private bool attackReady = true;

	private void
  Start()
  {
    IEnumerator attackCooldownCoroutine = AttackCooldown();
    StartCoroutine(attackCooldownCoroutine);
	}
	
	private void
  Update()
  {
    if(attackReady)
    {
      EnemyController enemy = GetEnemyWithLowestHealth();
      if(enemy != null) 
        FireArrowAt(enemy.gameObject);
      
      attackReady = false;
    }
	}
  
  public void
  Upgrade()
  {
    if(attackCooldown > .6f)
      attackCooldown -= .1f;

    color.b = color.b * .808f;

    GetComponent<MeshRenderer>().material.color = color;
  }

  public void
  MatchUpgrade(TowerController towerController_base)
  {
    towerRange = towerController_base.towerRange;
    powerCost = towerController_base.powerCost;
    gobCost = towerController_base.gobCost;
    damage = towerController_base.damage;
    attackCooldown = towerController_base.attackCooldown;
    color = towerController_base.color;

    GetComponent<MeshRenderer>().material.color = color;
  }

  private void
  FireArrowAt(GameObject enemy)
  {
    Vector3 vectorFromTowerToEnemy = enemy.transform.position - transform.position;
    Quaternion arrowRotation = Quaternion.LookRotation(vectorFromTowerToEnemy);
    Vector3 arrowStartPosition = arrowRotation * arrowStartOffset + transform.position;

    GameObject arrow = Instantiate<GameObject>(arrowObject, arrowStartPosition, arrowRotation);

    arrow.GetComponent<ArrowController>().attackTarget = enemy;
    arrow.GetComponent<ArrowController>().damage = damage;

    arrow.SetActive(true);
  }

  private IEnumerator
  AttackCooldown()
  {
    for(;;)
    {
      if(attackReady)
        yield return null;
      else
      {
        yield return new WaitForSeconds(attackCooldown);
        attackReady = true;
      }
    }
  }

  private EnemyController
  GetEnemyWithLowestHealth()
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
      if(enemy == null || enemy.dead)
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
