using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameDriver : MonoBehaviour {

	[SerializeField]
	private int score;
	[SerializeField]
	private GameObject starCharacter;

	public List<GameObject> activeAsteroids = new List<GameObject>();
	public List<GameObject> asteroids;
	public GameObject poolingPosition;


	private GameObject startPanel;
	private GameObject gameOverPanel;
	private GameObject scoreText;

	// Use this for initialization
	void Start () {
		score = 0;
		startPanel = GameObject.Find ("/Canvas/StartPanel");
		gameOverPanel = GameObject.Find ("/Canvas/GameOverPanel");
		gameOverPanel.SetActive (false);//This panel needs to start out hidden
		scoreText = GameObject.Find ("/Canvas/Score");
		scoreText.GetComponent<Text> ().text = "Score : " + score.ToString ();
		scoreText.SetActive (false);//Score starts out hidden while on main menu


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
			StartCoroutine(Repool(asteroid, poolingPosition, 0.75f));

		}
		else{
			//Game Over
			Debug.Log("GAME OVER! :(");
			GameOver ();

		}
	}

	//Repools a game object to how it started as when the game opened
	//TODO: possibly pass in a parameter to use in WaitForSeconds
	IEnumerator Repool(GameObject go, GameObject origin, float delay){
		activeAsteroids.Remove (go);
		yield return new WaitForSeconds(delay);

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
		Debug.Log ("Starting Game! :)");
		//If there are asteroids leftover on the screen, repool them instantly
		while(activeAsteroids.Count != 0){
			activeAsteroids.ElementAt(0).GetComponent<SphereCollider> ().enabled = false;
			activeAsteroids.ElementAt(0).GetComponent<Asteroid>().originalSpeed = 0;
			activeAsteroids.ElementAt(0).GetComponent<Rigidbody> ().isKinematic = true;
			activeAsteroids.ElementAt(0).transform.position = starCharacter.transform.position;
			activeAsteroids.ElementAt(0).transform.parent = starCharacter.transform;

			StartCoroutine (Repool (activeAsteroids.ElementAt(0), poolingPosition, 0f));
		}

		Time.timeScale = 1f;
		//If this is the first game loop since program has opened.
		startPanel.SetActive (false);
		//If this is a restart set the game over panel inactive as well
		gameOverPanel.SetActive(false);
		starCharacter.SetActive (true);
		scoreText.SetActive (true);
		score = 0;
		gameObject.GetComponent<AsteroidSpawner> ().maxThrust = 20;//Reset base asteroid speed

		//TODO
		//
		// Reset the Star's position to the center of the screen
		//
		//


	}

	void GameOver(){
		Time.timeScale = 0f;
		scoreText.SetActive (false);
		gameOverPanel.SetActive (true);
		GameObject.Find ("/Canvas/GameOverPanel/EndScoreText").GetComponent<Text> ().text = "Score : " + score.ToString();
		//TODO
		//Load best personal high score and display it
		//GameObject.Find("/Canvas/GameOverPanel/BestScoreText").GetComponent<Text>().text = ???
	}

	//Returns the current score of the game
	public int GetScore(){
		return(score);
	}
}
