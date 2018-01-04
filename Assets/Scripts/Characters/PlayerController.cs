using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
PlayerController : MonoBehaviour
{
  public float speed = 1f;
  public int health = 10;
  public int gobs = 0;

  public TowerController baseTower;
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
    Debug.Log(leftJoystickInput);

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
    worldController.UnhilightAllTowers();

    if(!mouseController.mouseInUse)
    {
      TileController centerTileController = worldController.GetTileFromMousePosition(
        Camera.main,
        Input.mousePosition);
      TileController[] tileControllers = worldController.GetTilesForBuilding(
        centerTileController,
        baseTower);

      if(
        worldController.TilesAreValidForBuilding(tileControllers)
        && gobs >= baseTower.gobCost
        && powerHead.availablePower >= baseTower.powerCost)
      {
        for(
          int tileIndex = 0;
          tileIndex < tileControllers.Length;
          ++tileIndex)
        {
          TileController hitTile = tileControllers[tileIndex];

          hitTile.GetComponent<MeshRenderer>().material.color = Color.green;
        }

        if(Input.GetMouseButtonDown(0))
        {
          worldController.CreateTower(
          baseTower.gameObject,
          centerTileController,
          tileControllers);

          powerHead.availablePower -= baseTower.powerCost;
          gobs -= baseTower.gobCost;
        }

      }
      else if(tileControllers != null)
      {
        for(
          int tileIndex = 0;
          tileIndex < tileControllers.Length;
          ++tileIndex)
        {
          TileController hitTile = tileControllers[tileIndex];
          if(hitTile == null)
            continue;

          hitTile.GetComponent<MeshRenderer>().material.color = Color.red;
          if(hitTile.attachedBuilding != null)
            hitTile.attachedBuilding.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if(centerTileController.attachedBuilding != null && Input.GetMouseButtonDown(1))
        {
          worldController.RemoveTower(
            centerTileController,
            tileControllers);
          powerHead.availablePower += baseTower.powerCost;
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
