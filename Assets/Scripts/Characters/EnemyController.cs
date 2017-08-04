﻿using System.Collections;
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
  private Quaternion orientation;
  private Animator animator;

  public bool isDead = false;
  private float stunTime = 0f;

	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    navmeshAgent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();

    orientation = transform.localRotation;
    navmeshAgent.updateRotation = false;
    IEnumerator navigateCoroutine = Navigate();
    StartCoroutine(navigateCoroutine);
	}

  public void GetHit(float damage)
  {
    health -= damage;
    stunTime = 0.5f;
    if(health <= 0)
    {
      isDead = true;
      animator.SetBool("isDead", true);
      animator.SetBool("isHit", false);
    }
    else
    {
      animator.SetBool("isHit", true);
    }
  }

  void Update()
  {
    if(!isDead)
    {
      stunTime -= Time.deltaTime;
      if(stunTime < 0)
      {
        stunTime = 0;
        animator.SetBool("isHit", false);
      }

      Navigate();
    }
    transform.localRotation = orientation;
  }

  void Unexist()
  {
    Destroy(gameObject);
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
