using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public float speed = 1f;
  public GameObject building;

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
    rigidbody = GetComponentInChildren<Rigidbody>();	
    groundTile_LayerMask = LayerMask.GetMask("Ground Tile");
	}
	
	void Update()
  {
    if(cached_LastHitTile != null)
      cached_LastHitTile.GetComponent<MeshRenderer>().material.color = Color.white;

    RaycastHit hit;
    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 300f, groundTile_LayerMask))
    {
      GameObject hitObject = hit.transform.gameObject;
      cached_LastHitTile = hitObject;

      hitObject.GetComponent<MeshRenderer>().material.color = Color.green;
      if(Input.GetMouseButtonDown(0))
      {
        GroundTileProperties hitGroundTile = hitObject.GetComponent<GroundTileProperties>();
        hitGroundTile.attachedBuilding = Instantiate(
          building,
          hitObject.transform.position + hitGroundTile.buildingOffset,
          hitObject.transform.rotation,
          hitObject.transform);
      }
    }
	}

  private void FixedUpdate()
  {
    Vector2 leftJoystickInput;
    leftJoystickInput.x = Input.GetAxisRaw("Horizontal");
    leftJoystickInput.y = Input.GetAxisRaw("Vertical");
    
    switch(player_controlMode)
    {
      case Player_ControlMode.Move:
      {
        Vector3 movement = new Vector3(
          leftJoystickInput.x,
          0f,
          leftJoystickInput.y);

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

      } break;
    }
  }
}
