using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public float speed = 1f;
  public int health = 10;
  public int gobs = 0;
  public GameObject building;
  public Animator animator;
  public PowerHeadController powerHead;
  public SpriteRenderer spriteRenderer;
  public MouseController mouseController;
  public GameObject gameoverScreen;

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
	}

  void PlaceBuildingUpdate()
  {
    if(cached_LastHitTile != null)
    {
      cached_LastHitTile.GetComponent<MeshRenderer>().material.color = Color.white;
      GameObject attachedBuilding = cached_LastHitTile.GetComponent<GroundTileProperties>().attachedBuilding;
      if(attachedBuilding != null)
        attachedBuilding.GetComponent<MeshRenderer>().material.color =
          attachedBuilding.GetComponent<BasicTowerController>().color;
    }

    if(!mouseController.mouseInUse)
    {
      RaycastHit hit;
      if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 300f, groundTile_LayerMask))
      {
        GameObject hitObject = hit.transform.gameObject;
        cached_LastHitTile = hitObject;

        GroundTileProperties hitGroundTile = hitObject.GetComponent<GroundTileProperties>();
        BasicTowerController basicTowerController = building.GetComponent<BasicTowerController>();
        if(
          gobs >= basicTowerController.gobCost
          && powerHead.availablePower >= basicTowerController.powerCost
          && hitGroundTile.attachedBuilding == null)
        {
          hitObject.GetComponent<MeshRenderer>().material.color = Color.green;
          if(Input.GetMouseButtonDown(0))
          {
            hitGroundTile.attachedBuilding = Instantiate(
              building,
              hitObject.transform.position + hitGroundTile.buildingOffset,
              hitObject.transform.rotation);
            hitGroundTile.attachedBuilding.SetActive(true);
            powerHead.availablePower -= basicTowerController.powerCost;
            gobs -= basicTowerController.gobCost;
          }
        }
        else
        {
          hitObject.GetComponent<MeshRenderer>().material.color = Color.red;

          if(hitGroundTile.attachedBuilding != null)
            hitGroundTile.attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.red;

          if(Input.GetMouseButtonDown(1))
          {
            Destroy(hitGroundTile.attachedBuilding);
            hitGroundTile.attachedBuilding = null;
            powerHead.availablePower += basicTowerController.powerCost;
          }
        }
      }
    }
  }
	
	void Update()
  {
    PlaceBuildingUpdate();
    if(rigidbody.velocity.x < 0)
      spriteRenderer.flipX = true;
    else if(rigidbody.velocity.x > 0)
      spriteRenderer.flipX = false;
	}

  public void ResetAnimationBools()
  {
    animator.SetBool("isHit", false);
  }

  public void GetHit(int damage)
  {
    health -= damage;
    animator.SetBool("isHit", true);
    //Todo: Replace with superior transition
    if(health <= 0)
    {
      gameoverScreen.SetActive(true);
      gameObject.SetActive(false);
    }
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
