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

  public Animator animator;
  public SpriteRenderer spriteRenderer;
  public GameObject gobBase;

  public GameObject attackTarget;

  new private Rigidbody rigidbody;
  private NavMeshAgent navMeshAgent;
  private GameObject primaryAttackTarget;
  private LayerMask attackableLayerMask;

  public bool dead = false;
  private float stunTime = 0f;

	private void
  Awake()
  {
    rigidbody = GetComponent<Rigidbody>();
    navMeshAgent = GetComponent<NavMeshAgent>();

    primaryAttackTarget = attackTarget;

    navMeshAgent.updateRotation = false;

    IEnumerator navigateCoroutine = Navigate();
    StartCoroutine(navigateCoroutine);

    attackableLayerMask = LayerMask.NameToLayer("Attackable");
	}

  private void
  Update()
  {
    if(dead)
      return;

    stunTime -= Time.deltaTime;
    if(stunTime < 0)
    {
      stunTime = 0;
      animator.SetBool("isHit", false);
    }

    Navigate();

    if(
      !navMeshAgent.pathPending
      && navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
    {
      GetComponentInChildren<SpriteRenderer>().color = Color.red;
      GameObject closestAttackable = GetClosestAttackable();
      Debug.Log(closestAttackable);
      if(closestAttackable != null)
        attackTarget = closestAttackable;
    }
    else
    {
      GetComponentInChildren<SpriteRenderer>().color = Color.white;
      attackTarget = primaryAttackTarget;
    }
    
    if(TargetIsInRange(attackRange))
      animator.SetBool("isAttacking", true);

    if(rigidbody.velocity.x < 0)
      spriteRenderer.flipX = true;
    else if(rigidbody.velocity.x > 0)
      spriteRenderer.flipX = false;
  }

  public bool 
  TargetIsInRange(float range)
  {
    if(attackTarget == null)
      return false;

    Attackable attackable = attackTarget.GetComponent<Attackable>();
    if(attackable == null)
      return false;

    SphereCollider attackableSphereCollider = attackable.attackable.GetComponent<SphereCollider>();

    if(attackableSphereCollider == null)
      return false;

    Vector3 targetOffset = attackableSphereCollider.transform.position - transform.position;
    targetOffset.y = 0;
    return targetOffset.magnitude < attackableSphereCollider.radius + range;
  }

  private void
  FixedUpdate()
  {
    navMeshAgent.velocity = Vector3.zero;
    rigidbody.velocity = Vector3.zero;
    if(stunTime == 0)
    {
      float speedMultiplier = 1f;
      if(animator.GetBool("isAttacking"))
        speedMultiplier = 0f;

      Vector3 moveDirection = Vector3.Normalize(navMeshAgent.desiredVelocity);
      rigidbody.AddForce(moveDirection * moveSpeed * speedMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    else
    {
      rigidbody.velocity = Vector3.zero;
    }
  }

  private void
  OnDrawGizmos()
  {
    Gizmos.color = Color.grey;
    Gizmos.DrawWireSphere(transform.position, 1f); 
  }

  private GameObject
  GetClosestAttackable()
  {
    RaycastHit hitInfo;

    if(!Physics.SphereCast(
      new Ray(transform.position, Vector3.Normalize(navMeshAgent.desiredVelocity)),
      0.6f,
      out hitInfo,
      Mathf.Infinity,
      ~LayerMask.NameToLayer("Attackable")))
    {
      return null;
    }

    Debug.DrawLine(transform.position, hitInfo.transform.position);

    return hitInfo.transform.gameObject;
  }

  public void
  MeleeTarget()
  {
    if(!TargetIsInRange(attackRangeBeforeMiss))
      return;

    PlayerController playerController = attackTarget.GetComponent<PlayerController>();
    if(playerController != null)
    {
      playerController.GetHit(1);
      return;
    }
    
    TowerController towerController = attackTarget.GetComponent<TowerController>();
    if(towerController != null)
    {
      towerController.GetHit(1);
      return;
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
      navMeshAgent.SetDestination(primaryAttackTarget.transform.position);
      yield return new WaitForSeconds(.2f);
    }
  }
}
