using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJet : PlayerPhysicsInput {
	public float waterAcceleration = 45f;

	Vector2 aim;

	// Use this for initialization
	void Start () {
		aim = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		aim = new Vector2(Input.GetAxis("AimHorizontal"), -Input.GetAxis("AimVertical"));
	}

	override public void Apply(PlayerPhysics physics, float dt) {
		if (physics.grounded <= 0f && aim.magnitude > 0.5f) {
			physics.velocity -= waterAcceleration * aim * dt;
		}
	}
}
