using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
  public float speed;
  public GameObject targetObject;
	
	void FixedUpdate()
  {
    Vector3 vectorTowardsEnemy = targetObject.transform.position - transform.position;
    Vector3 directionTowardsEnemy = Vector3.Normalize(vectorTowardsEnemy);
    Quaternion quaternionTowardsEnemy = Quaternion.LookRotation(directionTowardsEnemy, transform.up);

    transform.position += directionTowardsEnemy * speed * Time.fixedDeltaTime;
    transform.rotation = quaternionTowardsEnemy;

    if(vectorTowardsEnemy.magnitude < .1f)
      Destroy(gameObject);
	}
}
