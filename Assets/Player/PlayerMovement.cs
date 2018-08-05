using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float maxSpeed = 10f;
	public float acceleration = 12f;
	public float coastAcceleration = 20f;
	public float skidAcceleration = 35f;
	public float airAcceleration = 20f;

	public float jumpSpeed = 14f;
	public float jumpSpeedBonus = 3f;
	public float jumpFloatAcceleration = 30f;
	public float fastFallAcceleration = 10f;
	public float gravityAcceleration = 50f;

	// Only used for position interpolation
	private Rigidbody2D body;

	private float inputHorizontal;
	private float inputVertical;
	private bool inputJump;
	private bool hasJumped;

	private Vector2 velocity;
	private float grounded;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		velocity = Vector2.zero;
		grounded = 0f;
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
		return Mathf.Sign(value) * (Mathf.Abs(value) - min) * (1.0f / (1.0f - min));
	}
	
	// Update is called once per frame
	void Update () {
		inputHorizontal = GetAxis("Horizontal");
		inputVertical = GetAxis("Vertical");
		inputJump = Input.GetButton("Jump");
		if (Input.GetButtonDown("Jump")) hasJumped = false;
	}

	void FixedUpdate() {
		var position = transform.position;

		if (grounded > 0) {
			// Three accelerations for running, coasting to a stop, and countering movement
			var target = maxSpeed * inputHorizontal;
			var accel = acceleration;
			if (Mathf.Abs(inputHorizontal) < 0.1f) {
				accel = coastAcceleration;
			} else if (inputHorizontal * velocity.x < 0f) {
				accel = skidAcceleration;
				target = 0f;
			}
			velocity.x = Mathf.MoveTowards(velocity.x, target, accel * Time.fixedDeltaTime);
			
			// Jumping gets extra height if you are moving quickly
			if (inputJump && !hasJumped) {
				grounded = 0f;
				velocity.y = jumpSpeed + jumpSpeedBonus * Mathf.Abs(velocity.x) / maxSpeed;
				hasJumped = true;
			}
		} else {
			// Air movement has slow h control, push down for slightly faster fall
			var target = maxSpeed * inputHorizontal;
			velocity.x = Mathf.MoveTowards(velocity.x, target, airAcceleration * Time.fixedDeltaTime);
			if (inputJump && velocity.y >= 0) {
				velocity.y += jumpFloatAcceleration * Time.fixedDeltaTime;
			}
			if (inputVertical < -0.5) {
				velocity.y += inputVertical * fastFallAcceleration * Time.fixedDeltaTime;
			}
		}

		// Apply gravity and motion
		velocity.y -= gravityAcceleration * Time.fixedDeltaTime;
		position += (Vector3)(velocity * Time.fixedDeltaTime);
		grounded -= Time.fixedDeltaTime;

		// Check for collisions along each axis
		var hitDown = Physics2D.BoxCast(position, new Vector2(0.5f, 0.1f), 0f, Vector2.down, 0.6f);
		if (hitDown && velocity.y <= 0 && (hitDown.distance <= 0.45f || grounded > 0f)) {
			position += new Vector3(0, 0.45f - hitDown.distance, 0);
			velocity.y = 0f;
			grounded = 0.1f;
		}
		var hitLeft = Physics2D.Raycast(position, Vector2.left, 0.45f);
		if (hitLeft && velocity.x <= 0) {
			position += new Vector3(0.45f - hitLeft.distance, 0, 0);
			velocity.x = 0;
		}
		var hitRight = Physics2D.Raycast(position, Vector2.right, 0.45f);
		if (hitRight && velocity.x >= 0) {
			position -= new Vector3(0.45f - hitRight.distance, 0, 0);
			velocity.x = 0;
		}
		var hitTop = Physics2D.Raycast(position, Vector2.up, 0.45f);
		if (hitTop && velocity.y >= 0) {
			position -= new Vector3(0, 0.45f - hitTop.distance, 0);
			velocity.y = 0;
		}

		// MovePosition will do interpolation at high framerates for extra smooth rendering
		body.MovePosition(position);
	}
}
