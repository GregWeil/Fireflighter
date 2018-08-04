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

	private Rigidbody2D body;

	private float horizontal;
	private float vertical;
	private bool jump;
	private bool hasJumped;

	private Vector2 velocity;
	private float grounded;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		velocity = Vector2.zero;
		grounded = 0f;
		horizontal = 0f;
		vertical = 0f;
		jump = false;
		hasJumped = false;
	}

	private static float GetAxis(string name) {
		const float min = 0.2f;
		var value = Input.GetAxis(name);
		if (Mathf.Abs(value) < min) return 0;
		return Mathf.Sign(value) * (Mathf.Abs(value) - min) * (1.0f / (1.0f - min));
	}
	
	// Update is called once per frame
	void Update () {
		horizontal = GetAxis("Horizontal");
		vertical = GetAxis("Vertical");
		jump = Input.GetButton("Jump");
		if (Input.GetButtonUp("Jump")) hasJumped = false;
	}

	void FixedUpdate() {
		var position = transform.position;

		if (grounded > 0) {
			var target = maxSpeed * horizontal;
			var accel = acceleration;
			if (target == 0f) {
				accel = coastAcceleration;
			} else if (velocity.x * target < 0f) {
				accel = skidAcceleration;
				target = 0f;
			}
			velocity.x = Mathf.MoveTowards(velocity.x, target, accel * Time.fixedDeltaTime);
			if (jump && !hasJumped) {
				grounded = 0f;
				velocity.y = jumpSpeed + jumpSpeedBonus * Mathf.Abs(velocity.x) / maxSpeed;
				hasJumped = true;
			}
		} else {
			velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * horizontal, airAcceleration * Time.fixedDeltaTime);
			if (jump && velocity.y >= 0) {
				velocity.y += jumpFloatAcceleration * Time.fixedDeltaTime;
			}
			if (vertical < -0.5) {
				velocity.y += fastFallAcceleration * vertical * Time.fixedDeltaTime;
			}
		}
		velocity.y -= gravityAcceleration * Time.fixedDeltaTime;

		position += (Vector3)(velocity * Time.fixedDeltaTime);

		grounded -= Time.fixedDeltaTime;
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

		body.MovePosition(position);
	}
}
