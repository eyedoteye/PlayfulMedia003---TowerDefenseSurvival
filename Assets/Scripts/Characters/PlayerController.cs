using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
PlayerController : MonoBehaviour
{
  public float speed = 1f;
  public int health = 10;
  public int gobs = 0;

  public GameObject building;
  public Animator animator;
  public PowerHeadController powerHead;
  public SpriteRenderer spriteRenderer;
  public GameObject gameoverScreen;
  public MouseController mouseController;
  public WorldController worldController;

  new private Rigidbody rigidbody;
  private GameObject lastHitTile;

	private void
  Start()
  {
    rigidbody = GetComponent<Rigidbody>();
	}

	private void
  Update()
  {
    if(rigidbody.velocity.x < 0)
      spriteRenderer.flipX = true;
    else if(rigidbody.velocity.x > 0)
      spriteRenderer.flipX = false;
	}

  private void
  FixedUpdate()
  {
    Vector2 leftJoystickInput;
    leftJoystickInput.x = Input.GetAxisRaw("Horizontal");
    leftJoystickInput.y = Input.GetAxisRaw("Vertical");

    float speedMultiplier = 1f;
    if(animator.GetBool("isHit"))
      speedMultiplier = 0.3f;

    Vector3 movement = new Vector3(
      leftJoystickInput.x,
      0f,
      leftJoystickInput.y);

    rigidbody.velocity = Vector3.zero;

    rigidbody.AddForce(
      movement * speed * speedMultiplier * Time.fixedDeltaTime,
      ForceMode.VelocityChange);

    HandleBuildingPlacement();
  }

  private void
  HandleBuildingPlacement()
  {
    if(lastHitTile != null)
    {
      lastHitTile.GetComponent<MeshRenderer>().material.color = Color.white;
      GameObject attachedBuilding = lastHitTile.GetComponent<TileController>().attachedBuilding;
      if(attachedBuilding != null)
        attachedBuilding.GetComponent<MeshRenderer>().material.color =
          attachedBuilding.GetComponent<TowerController>().color;
    }

    if(!mouseController.mouseInUse)
    {
      TileController tileController = worldController.GetGroundTile(
        Camera.main,
        Input.mousePosition);       

      if(tileController != null)
      {
        GameObject hitTile = tileController.gameObject;
        lastHitTile = hitTile;

        TowerController hitTowerController = building.GetComponent<TowerController>();
        if(
          gobs >= hitTowerController.gobCost
          && powerHead.availablePower >= hitTowerController.powerCost
          && tileController.attachedBuilding == null
          && tileController.blocked == false)
        {
          lastHitTile.GetComponent<MeshRenderer>().material.color = Color.green;
          if(Input.GetMouseButtonDown(0))
          {
            worldController.CreateTower(
              building,
              tileController);

            powerHead.availablePower -= hitTowerController.powerCost;
            gobs -= hitTowerController.gobCost;
          }
        }
        else
        {
          hitTile.GetComponent<MeshRenderer>().material.color = Color.red;

          if(tileController.attachedBuilding != null)
          {
            tileController.attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.red;

            if(Input.GetMouseButtonDown(1))
            {
              Destroy(tileController.attachedBuilding);
              tileController.attachedBuilding = null;
              powerHead.availablePower += hitTowerController.powerCost;
            }
          }
        }
      }
    }
  }
	
  public void
  ResetAnimationBools()
  {
    animator.SetBool("isHit", false);
  }

  public void
  GetHit(int damage)
  {
    health -= damage;
    animator.SetBool("isHit", true);

    if(health <= 0)
      gameoverScreen.SetActive(true);
  }

}
