using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// speedを制御する
	public float speed = 10;

	void FixedUpdate ()
	{
		float x =  Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Rigidbody rigidbody = GetComponent<Rigidbody>();

		// xとyにspeedを掛ける
		rigidbody.AddForce(x * speed, 0, z * speed);
	}
}
