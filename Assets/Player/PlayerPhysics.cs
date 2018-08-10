using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPhysicsInput : MonoBehaviour {
	public abstract void Apply(PlayerPhysics physics, float dt);
}

public class PlayerPhysics : MonoBehaviour {
	public float gravityAcceleration = 50f;

	// Only used for position interpolation
	private Rigidbody2D body;

	// Current movement state
	public Vector2 velocity;
	public float grounded;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
		velocity = Vector2.zero;
		grounded = 0f;
	}
	
	// FixedUpdate is called on a regular interval
	void FixedUpdate () {
		var position = transform.position;
		foreach (var input in GetComponents<PlayerPhysicsInput>()) {
			input.Apply(this, Time.fixedDeltaTime);
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
