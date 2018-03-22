using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDriver : MonoBehaviour {

	[SerializeField]
	private int score;
	[SerializeField]
	private GameObject starCharacter;

	public List<GameObject> activeAsteroids = new List<GameObject>();
	public List<GameObject> asteroids;
	public GameObject poolingPosition;


	// Use this for initialization
	void Start () {
		score = 0;

		asteroids = gameObject.GetComponent<AsteroidSpawner> ().asteroids;
		if(poolingPosition == null){
			Debug.LogError ("Missing a reference to a pooling position, please create an empty GO called 'Pooling Position' and place it at (20, 0, 20).");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			gameObject.GetComponent<AsteroidSpawner>().SpawnAsteroid ();
		}
	}

	//An event called by the StarDriver when it detects a collision with an asteroid
	public void AsteroidCollision(GameObject star, GameObject asteroid){
		if(star.GetComponent<StarDriver>().color == asteroid.GetComponent<Asteroid>().color){
			//Caught an asteroid set it under the star so the particles can flow into the star
			asteroid.GetComponent<SphereCollider> ().enabled = false;
			asteroid.GetComponent<Asteroid>().originalSpeed = 0;
			asteroid.GetComponent<Rigidbody> ().isKinematic = true;
			asteroid.transform.position = star.transform.position;
			asteroid.transform.parent = star.transform;
			//After the particles have flowed in reset the asteroid
			StartCoroutine(Repool(asteroid, poolingPosition));

		}
		else{
			//
			//
			//Game Over
			Debug.Log("GAME OVER! :(");
			//
			//
		}
	}

	//Repools a game object to how it started as when the game opened
	//TODO: possibly pass in a parameter to use in WaitForSeconds
	IEnumerator Repool(GameObject go, GameObject origin){
		yield return new WaitForSeconds(0.75f);

		activeAsteroids.Remove (go);
		go.transform.parent = null;
		go.transform.position = origin.transform.position;
		go.SetActive (false);

		if(go.tag == "Asteroid"){
			go.GetComponent<Rigidbody> ().isKinematic = false;
			go.GetComponent<Rigidbody> ().mass = 1;
			go.GetComponent<SphereCollider> ().enabled = true;
			go.GetComponent<SphereCollider> ().isTrigger = true;
			go.GetComponent<Asteroid>().Reset();
			asteroids.Add (go);
		}
	}

	public void StartGame(){
		
	}

	void GameOver(){
		
	}

	//Returns the current score of the game
	public int GetScore(){
		return(score);
	}
}
