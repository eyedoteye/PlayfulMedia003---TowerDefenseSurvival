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
  private Material material;
  private Quaternion orientation;

  private float stunTime = 0f;

	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    navmeshAgent = GetComponent<NavMeshAgent>();
    material = GetComponent<MeshRenderer>().material;

    orientation = transform.localRotation;
    navmeshAgent.updateRotation = false;
    IEnumerator navigateCoroutine = Navigate();
    StartCoroutine(navigateCoroutine);
	}

  public void GetHit(float damage)
  {
    health -= damage;
    stunTime = 0.5f;
  }

  void Update()
  {
    stunTime -= Time.deltaTime;
    if(stunTime < 0)
    {
      stunTime = 0;
      material.mainTextureOffset = new Vector2(0f, 0.75f);
    }
    else
    {
      material.mainTextureOffset = new Vector2(0.25f, 0.75f);
    }

    Navigate();
    transform.localRotation = orientation;
    Debug.Log(orientation);
  }

  private void FixedUpdate()
  {
    navmeshAgent.velocity = Vector3.zero;
    if(stunTime == 0)
    {
      Vector3 moveDirection = Vector3.Normalize(navmeshAgent.desiredVelocity);
      rigidbody.AddForce(moveDirection * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    } 
    else
    {
      rigidbody.velocity = Vector3.zero;
    }
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
