using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private float horizontal;
	private float vertical;
	private bool jump;
	private bool hasJumped;

	Vector2 velocity;
	float grounded;

	// Use this for initialization
	void Start () {
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
		if (grounded > 0) {
			var target = 10f * horizontal;
			var acceleration = 10f * Time.fixedDeltaTime;
			if (Mathf.Abs(target - velocity.x) > 4f) {
				acceleration *= 5f;
			}
			velocity.x = Mathf.MoveTowards(velocity.x, target, acceleration);
			if (jump && !hasJumped) {
				grounded = 0f;
				velocity.y += 15f;
				hasJumped = true;
			}
		} else {
			velocity.x = Mathf.MoveTowards(velocity.x, 10f * horizontal, 20f * Time.fixedDeltaTime);
			if (jump) {
				velocity.y += 30f * Time.fixedDeltaTime;
			}
			if (vertical < 0) {
				velocity.y += 5f * vertical * Time.fixedDeltaTime;
			}
		}
		velocity.y -= 50f * Time.fixedDeltaTime;

		transform.position += (Vector3)(velocity * Time.fixedDeltaTime);

		grounded -= Time.fixedDeltaTime;
		var hitDownLeft = Physics2D.Raycast(transform.position + 0.3f * Vector3.left, Vector2.down, 0.5f);
		var hitDownRight = Physics2D.Raycast(transform.position + 0.3f * Vector3.right, Vector2.down, 0.5f);
		if ((hitDownLeft || hitDownRight) && velocity.y <= 0) {
			var distance = 0f;
			if (hitDownLeft && hitDownRight) {
				distance = (hitDownLeft.distance + hitDownRight.distance) / 2f;
			} else if (hitDownLeft) {
				distance = hitDownLeft.distance;
			} else if (hitDownRight) {
				distance = hitDownRight.distance;
			}
			transform.position += new Vector3(0, 0.5f - distance, 0);
			velocity.y = 0;
			grounded = 0.1f;
		}
		var hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.45f);
		if (hitLeft && velocity.x <= 0) {
			transform.position += new Vector3(0.45f - hitLeft.distance, 0, 0);
			velocity.x = 0;
		}
		var hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.45f);
		if (hitRight && velocity.x >= 0) {
			transform.position -= new Vector3(0.45f - hitRight.distance, 0, 0);
			velocity.x = 0;
		}
		var hitTop = Physics2D.Raycast(transform.position, Vector2.up, 0.45f);
		if (hitTop && velocity.y >= 0) {
			transform.position -= new Vector3(0, 0.45f - hitTop.distance, 0);
			velocity.y = 0;
		}
	}
}
