using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
  public float damage = 0f;
  public float timeToHit = 5f;
	
  public GameObject attackTarget;
  
  private Vector3 startPosition;
  private float timePassed;
  private Vector3 targetPosition;

  private void
  Awake()
  {
    startPosition = transform.position;
    timePassed = 0f;
  }

  private void
  FixedUpdate()
  {
    float timeMultiplier = 1;
    if(attackTarget == null || attackTarget.GetComponent<EnemyController>().health < damage)
      timeMultiplier = 8;
    else
      targetPosition = attackTarget.transform.position;

    timePassed += Time.fixedDeltaTime * timeMultiplier;

    if(timePassed > timeToHit)
    {
      if(attackTarget != null)
        attackTarget.GetComponent<EnemyController>().GetHit(damage);

      Destroy(gameObject);
      return;
    }

    transform.position = Vector3.Lerp(startPosition, targetPosition, timePassed / timeToHit);

    Vector3 vectorTowardsEnemy = targetPosition - transform.position;
    Vector3 directionTowardsEnemy = Vector3.Normalize(vectorTowardsEnemy);
    Quaternion quaternionTowardsEnemy = Quaternion.LookRotation(directionTowardsEnemy, transform.up);
    transform.rotation = quaternionTowardsEnemy;
	}
}