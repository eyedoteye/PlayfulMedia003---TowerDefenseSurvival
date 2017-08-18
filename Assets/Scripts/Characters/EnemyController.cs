using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class
EnemyController : MonoBehaviour
{
  public float moveSpeed;
  public float health;
  public float attackRange;
  public float attackRangeBeforeMiss;

  public Transform attackPosition;
  public Animator animator;
  public SpriteRenderer spriteRenderer;
  public GameObject gobBase;

  public GameObject navigationTarget;

  new private Rigidbody rigidbody;
  private NavMeshAgent navmeshAgent;

  public bool dead = false;
  private float stunTime = 0f;

	private void
  Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    navmeshAgent = GetComponent<NavMeshAgent>();

    navmeshAgent.updateRotation = false;
    IEnumerator navigateCoroutine = Navigate();
    StartCoroutine(navigateCoroutine);
	}

  private void
  Update()
  {
    if(!dead)
    {
      stunTime -= Time.deltaTime;
      if(stunTime < 0)
      {
        stunTime = 0;
        animator.SetBool("isHit", false);
      }

      Navigate();

      Vector3 targetOffset = navigationTarget.transform.position - transform.position;
      targetOffset.y = 0;
      if(targetOffset.magnitude < attackRange)
      {
        animator.SetBool("isAttacking", true);
      }
      if(rigidbody.velocity.x < 0)
        spriteRenderer.flipX = true;
      else if(rigidbody.velocity.x > 0)
        spriteRenderer.flipX = false;
    }
  }

  private void
  FixedUpdate()
  {
    navmeshAgent.velocity = Vector3.zero;
    rigidbody.velocity = Vector3.zero;
    if(stunTime == 0)
    {
      float speedMultiplier = 1f;
      if(animator.GetBool("isAttacking"))
        speedMultiplier = 0f;

      Vector3 moveDirection = Vector3.Normalize(navmeshAgent.desiredVelocity);
      rigidbody.AddForce(moveDirection * moveSpeed * speedMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
    } 
    else
    {
      rigidbody.velocity = Vector3.zero;
    }
  }

  public void
  MeleeTarget()
  {
    Vector3 targetOffsetFromAttack = navigationTarget.transform.position - attackPosition.transform.position;
    targetOffsetFromAttack.y = 0;
    if(targetOffsetFromAttack.magnitude < attackRangeBeforeMiss)
    {
      PlayerController playerController = navigationTarget.GetComponent<PlayerController>();
      if(playerController != null)
        playerController.GetHit(1);
    }
  }

  public void
  ResetAnimationBools()
  {
    animator.SetBool("isHit", false);
    animator.SetBool("isAttacking", false);
  }

  public void
  GetHit(float damage)
  {
    health -= damage;
    stunTime = 0.5f;
    if(health <= 0)
    {
      dead = true;
      animator.SetBool("isDead", true);
      animator.SetBool("isHit", false);
    }
    else
    {
      animator.SetBool("isHit", true);
    }
  }

  public void
  Die()
  {
    GameObject gob = Instantiate(gobBase);
    Vector3 gobPosition = transform.position;
    gobPosition.y = gobBase.transform.position.y;
    gob.transform.position = gobPosition;

    Destroy(gameObject);
  }

  private IEnumerator
  Navigate()
  {
    for(;;)
    {
      navmeshAgent.SetDestination(navigationTarget.transform.position);
      yield return new WaitForSeconds(.2f);
    }
  }
}
