using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name != "AsteroidEnterTrigger"){
			return;
		}

		gameObject.GetComponent<SphereCollider> ().isTrigger = false;
		Destroy (this);
	}
}
