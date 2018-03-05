using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	public float originalSpeed;
	[SerializeField]
	private float speed;
	private bool isInitialized = false;

	Rigidbody rb;

	void Start(){
		rb = gameObject.GetComponent<Rigidbody> ();
	}


	void Update(){
		if (!isInitialized) {
			return;
		}

		gameObject.GetComponent<Rigidbody> ().velocity = gameObject.GetComponent<Rigidbody> ().velocity.normalized * originalSpeed;
		speed = rb.velocity.magnitude;

	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name != "AsteroidEnterTrigger"){
			return;
		}

		originalSpeed = rb.velocity.magnitude;
		isInitialized = true;
		gameObject.GetComponent<SphereCollider> ().isTrigger = false;
	}
		
}
