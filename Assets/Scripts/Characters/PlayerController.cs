using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public float speed = 1f;
  public float health = 10f;
  public GameObject building;
  public GameObject healthHUD;
  public Animator animator;
  public PowerHeadController powerHead;

  new private Rigidbody rigidbody;
  private LayerMask groundTile_LayerMask;

  private GameObject cached_LastHitTile;

  public enum Player_ControlMode 
  {
    Move,
    Build
  } private Player_ControlMode player_controlMode =
    Player_ControlMode.Move;

	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    groundTile_LayerMask = LayerMask.GetMask("Ground Tile");
    healthHUD.GetComponent<Animator>().SetInteger("health", (int)health);
	}

  void PlaceBuildingUpdate()
  {
    if(cached_LastHitTile != null)
    {
      cached_LastHitTile.GetComponent<MeshRenderer>().material.color = Color.white;
      GameObject attachedBuilding = cached_LastHitTile.GetComponent<GroundTileProperties>().attachedBuilding;
      if(attachedBuilding != null)
        attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.white;
    }

    RaycastHit hit;
    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 300f, groundTile_LayerMask))
    {
      GameObject hitObject = hit.transform.gameObject;
      cached_LastHitTile = hitObject;

      GroundTileProperties hitGroundTile = hitObject.GetComponent<GroundTileProperties>();
      int powerCost = building.GetComponent<BasicTowerController>().powerCost;
      if(powerHead.availablePower >= powerCost && hitGroundTile.attachedBuilding == null)
      {
        hitObject.GetComponent<MeshRenderer>().material.color = Color.green;

        if(Input.GetMouseButtonDown(0))
        {
          hitGroundTile.attachedBuilding = Instantiate(
            building,
            hitObject.transform.position + hitGroundTile.buildingOffset,
            hitObject.transform.rotation);
          powerHead.availablePower -= powerCost;
        }
      }
      else
      {
        hitObject.GetComponent<MeshRenderer>().material.color = Color.red;

        if(hitGroundTile.attachedBuilding != null)
          hitGroundTile.attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.red;
      }
    }
  }
	
	void Update()
  {
    PlaceBuildingUpdate();
	}

  public void ResetAnimationBools()
  {
    animator.SetBool("isHit", false);
  }

  public void GetHit(float damage)
  {
    health -= damage;
    animator.SetBool("isHit", true);
    healthHUD.GetComponent<Animator>().SetInteger("health", (int)health);
  }

  private void FixedUpdate()
  {
    Vector2 leftJoystickInput;
    leftJoystickInput.x = Input.GetAxisRaw("Horizontal");
    leftJoystickInput.y = Input.GetAxisRaw("Vertical");

    float speedMultiplier = 1f;
    if(animator.GetBool("isHit"))
      speedMultiplier = 0.3f;

    switch(player_controlMode)
    {
      case Player_ControlMode.Move:
      {
        Vector3 movement = new Vector3(
          leftJoystickInput.x,
          0f,
          leftJoystickInput.y);

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(
          movement * speed * speedMultiplier * Time.fixedDeltaTime,
          ForceMode.VelocityChange);

      } break;
    }
  }
  
}
