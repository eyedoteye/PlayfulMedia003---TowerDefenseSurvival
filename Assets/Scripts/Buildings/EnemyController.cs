using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  public GameObject navigationTarget;
  public float moveSpeed;
  public float health;

  new private Rigidbody rigidbody;
  private NavMeshAgent navmeshAgent;

	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    navmeshAgent = GetComponent<NavMeshAgent>();
    IEnumerator navigateCoroutine = Navigate();
    StartCoroutine(navigateCoroutine);
	}

  void Update()
  {
    Navigate();
  }

  private void FixedUpdate()
  {
    navmeshAgent.velocity = Vector3.zero;
    Vector3 moveDirection = Vector3.Normalize(navmeshAgent.desiredVelocity);
    rigidbody.AddForce(moveDirection * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
  }

  IEnumerator Navigate()
  {
    for(;;)
    {
      navmeshAgent.SetDestination(navigationTarget.transform.position);
      yield return new WaitForSeconds(.2f);
    }
  }
}
