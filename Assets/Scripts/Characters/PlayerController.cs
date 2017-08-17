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
  public GameObject gameoverScreen;
  public MouseController mouseController;
  public WorldController worldController;

  new private Rigidbody rigidbody;

  private GameObject last_hit_tile;

  public enum Player_ControlMode 
  {
    Move,
    Build
  } private Player_ControlMode player_controlMode =
    Player_ControlMode.Move;

	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
	}

  void PlaceBuildingUpdate()
  {
    if(last_hit_tile != null)
    {
      last_hit_tile.GetComponent<MeshRenderer>().material.color = Color.white;
      GameObject attachedBuilding = last_hit_tile.GetComponent<TileController>().attachedBuilding;
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
        GameObject hit_tile = tileController.gameObject;
        last_hit_tile = hit_tile;

        TowerController hit_towerController = building.GetComponent<TowerController>();
        if(
          gobs >= hit_towerController.gobCost
          && powerHead.availablePower >= hit_towerController.powerCost
          && tileController.attachedBuilding == null
          && tileController.blocked == false)
        {
          last_hit_tile.GetComponent<MeshRenderer>().material.color = Color.green;
          if(Input.GetMouseButtonDown(0))
          {
            worldController.CreateTower(
              building,
              tileController);

            powerHead.availablePower -= hit_towerController.powerCost;
            gobs -= hit_towerController.gobCost;
          }
        }
        else
        {
          hit_tile.GetComponent<MeshRenderer>().material.color = Color.red;

          if(tileController.attachedBuilding != null)
            tileController.attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.red;

          if(Input.GetMouseButtonDown(1))
          {
            Destroy(tileController.attachedBuilding);
            tileController.attachedBuilding = null;
            powerHead.availablePower += hit_towerController.powerCost;
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

    if(health <= 0)
      gameoverScreen.SetActive(true);
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
