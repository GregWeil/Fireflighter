using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJet : PlayerPhysicsInput {
	public Vector2 waterAcceleration = new Vector2(60f, 40f);
	public float maxSpeed = 12f;

	Vector2 aim;

	// Use this for initialization
	void Start () {
		aim = Vector2.zero;
	}

	// Do some math to rescale from x-y to 0-1
	private static float Rescale(float value, float min, float max) {
		if (Mathf.Abs(value) < min) return 0.0f;
		if (Mathf.Abs(value) > max) return Mathf.Sign(value);
		min = 0f;
		return Mathf.Sign(value) * (Mathf.Abs(value) - min) / (max - min);
	}
	
	// Update is called once per frame
	void Update () {
		aim = new Vector2(
			Input.GetAxis("AimHorizontal"),
			-Input.GetAxis("AimVertical")
		);
	}

	override public void Apply(PlayerPhysics physics, float dt) {
		if (physics.grounded <= 0f) {
			var target = maxSpeed * Mathf.Sign(-aim.x);
			var accel = waterAcceleration.x * Mathf.Abs(Rescale(aim.x, 0.6f, 0.8f)) * dt;
			if (Mathf.Sign(target - physics.velocity.x) == Mathf.Sign(target)) {
				physics.velocity.x = Mathf.MoveTowards(physics.velocity.x, target, accel);
			}
		Debug.Log(waterAcceleration.y * Rescale(aim.y, 0.2f, 0.6f));
			physics.velocity.y -= waterAcceleration.y * Rescale(aim.y, 0.2f, 0.6f) * dt;
		}
	}
}
