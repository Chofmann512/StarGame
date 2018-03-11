using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDriver : MonoBehaviour {

	[SerializeField]
	private int score;


	// Use this for initialization
	void Start () {
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			gameObject.GetComponent<AsteroidSpawner>().SpawnAsteroid ();
		}
	}



	public int GetScore(){
		return(score);
	}
}
