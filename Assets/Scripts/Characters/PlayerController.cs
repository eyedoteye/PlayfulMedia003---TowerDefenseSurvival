﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  public float speed = 1f;

  new private Rigidbody rigidbody;
	// Use this for initialization
	void Start()
  {
    rigidbody = GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void Update() {
	}

  private void FixedUpdate()
  {
    Vector2 leftJoystickInput;
    leftJoystickInput.x = Input.GetAxisRaw("Horizontal");
    leftJoystickInput.y = Input.GetAxisRaw("Vertical");
    Vector3 movement = new Vector3(
      leftJoystickInput.x,
      leftJoystickInput.y,
      0f);

    rigidbody.velocity = Vector3.zero;
    rigidbody.AddForce(movement * speed * Time.deltaTime, ForceMode.VelocityChange);
  }

}
