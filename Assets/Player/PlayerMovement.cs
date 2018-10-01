using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerPhysicsInput {

	public float maxSpeed = 10f;
	public float acceleration = 12f;
	public float coastAcceleration = 20f;
	public float skidAcceleration = 35f;
	public float airAcceleration = 20f;

	public float jumpSpeed = 14f;
	public float jumpSpeedBonus = 3f;
	public float jumpFloatAcceleration = 30f;
	public float fastFallAcceleration = 10f;

	private float inputHorizontal;
	private float inputVertical;
	private bool inputJump;
	private bool hasJumped;

	// Use this for initialization
	void Start () {
		inputHorizontal = 0f;
		inputVertical = 0f;
		inputJump = false;
		hasJumped = false;
	}

	// Do some math to rescale from 0.2-1 to 0-1
	private static float GetAxis(string name) {
		const float min = 0.2f;
		var value = Input.GetAxis(name);
		if (Mathf.Abs(value) < min) return 0.0f;
		return Mathf.Sign(value) * (Mathf.Abs(value) - min) / (1.0f - min);
	}
	
	// Update is called once per frame
	void Update () {
		inputHorizontal = GetAxis("Horizontal");
		inputVertical = GetAxis("Vertical");
		inputJump = Input.GetButton("Jump");
		if (Input.GetButtonDown("Jump")) hasJumped = false;
	}

	override public void Apply(PlayerPhysics physics, float dt) {
		if (physics.grounded > 0) {
			// Three accelerations for running, coasting to a stop, and countering movement
			var target = maxSpeed * inputHorizontal;
			var accel = acceleration;
			if (Mathf.Abs(inputHorizontal) < 0.1f) {
				accel = coastAcceleration;
			} else if (inputHorizontal * physics.velocity.x < 0f) {
				accel = skidAcceleration;
				target = 0f;
			}
			physics.velocity.x = Mathf.MoveTowards(physics.velocity.x, target, accel * dt);
			
			// Jumping gets extra height if you are moving quickly
			if (inputJump && !hasJumped) {
				physics.grounded = 0f;
				physics.velocity.y = jumpSpeed + jumpSpeedBonus * Mathf.Abs(physics.velocity.x) / maxSpeed;
				hasJumped = true;
			}
		} else {
			// Air movement has slow h control, push down for slightly faster fall
			if (Mathf.Abs(inputHorizontal) > 0.2f) {
				var target = maxSpeed * Mathf.Sign(inputHorizontal);
				var accel = airAcceleration * Mathf.Abs(inputHorizontal) * dt;
				if (Mathf.Sign(target - physics.velocity.x) == Mathf.Sign(target)) {
					physics.velocity.x = Mathf.MoveTowards(physics.velocity.x, target, accel);
				}
			}
			if (inputJump && physics.velocity.y >= 0) {
				physics.velocity.y += jumpFloatAcceleration * dt;
			}
			if (inputVertical < -0.5) {
				physics.velocity.y += inputVertical * fastFallAcceleration * dt;
			}
		}
	}
}
