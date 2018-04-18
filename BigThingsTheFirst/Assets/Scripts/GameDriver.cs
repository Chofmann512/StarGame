using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GooglePlayGames;

public class GameDriver : MonoBehaviour {

	[SerializeField]
	private int score;
	[SerializeField]
	private GameObject starCharacter;
	[SerializeField]
	private float minSpawnTimerBound;
	[SerializeField]
	private float maxSpawnTimerBound;

	public List<GameObject> activeAsteroids = new List<GameObject>();
	public List<GameObject> asteroids;
	public GameObject poolingPosition;
	public int multiplierNum;

	private GameObject startPanel;
	private GameObject gameOverPanel;
	private GameObject scoreText;
	private GPGSDriver gpgsDriver;
	private bool isGameOver = true;//Flag used to tell whether or not the game is playing
	private bool multiplier;
	private int asteroidsCaught;//Used to track and report achievement progress

	// Use this for initialization
	void Start () {
		multiplierNum = 1;
		score = 0;
		startPanel = GameObject.Find ("/Canvas/StartPanel");
		gameOverPanel = GameObject.Find ("/Canvas/GameOverPanel");
		gameOverPanel.SetActive (false);//This panel needs to start out hidden
		scoreText = GameObject.Find ("/Canvas/Score");
		scoreText.GetComponent<Text> ().text = "Score : " + score.ToString ();
		scoreText.SetActive (false);//Score starts out hidden while on main menu

		gpgsDriver = GetComponent<GPGSDriver> ();
		asteroids = gameObject.GetComponent<AsteroidSpawner> ().asteroids;
		if(poolingPosition == null){
			Debug.LogError ("Missing a reference to a pooling position, please create an empty GO called 'Pooling Position' and place it at (20, 0, 20).");
		}
	}

	//An event called by the StarDriver when it detects a collision with an asteroid
	public void AsteroidCollision(GameObject star, GameObject asteroid){
		if(star.GetComponent<StarDriver>().color == asteroid.GetComponent<Asteroid>().color){
			//Report Achievement, TODO: Possibly flag this to compress calls
			gpgsDriver.ReportAchievement("TheBeginner");

			//Caught an asteroid set it under the star so the particles can flow into the star
			asteroid.GetComponent<SphereCollider> ().enabled = false;
			asteroid.GetComponent<Asteroid>().originalSpeed = 0;
			asteroid.GetComponent<Rigidbody> ().isKinematic = true;
			asteroid.transform.position = star.transform.position;
			asteroid.transform.parent = star.transform;
			asteroidsCaught++;
			//Increment score
			//LerpScore(100);//TODO: add checking for multiple asteroid catches
			if (!multiplier) {
				StartCoroutine (lerpScore (100));
				multiplierNum = 2;
			} else {
				StartCoroutine (lerpScore (100 * multiplierNum));
				if (multiplierNum < 4) {
					multiplierNum++;
				}
			}
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

	//Recursively spawn asteroids until game is over
	IEnumerator AsteroidTimer(float time){
		yield return new WaitForSeconds (time);

		if (!isGameOver) {
			//Game is still going spawn an asteroid and calculate, time to spawn next asteroid
			gameObject.GetComponent<AsteroidSpawner>().SpawnAsteroid ();
			//Roll for a chance to spawn a second asteroid at the same time
			//Between 0 and 11, max exclusive
			if(Random.Range(0, 11) > 8){
				gameObject.GetComponent<AsteroidSpawner>().SpawnAsteroid ();
			}
			//Recurse to the next timer
			StartCoroutine(AsteroidTimer(Random.Range (minSpawnTimerBound, maxSpawnTimerBound)));
		} 
		else{
			//Game has ended, stop recursing
			yield return null;
		}
	
	}
		

	public void StartGame(){
		Debug.Log ("Starting Game! :)");
		//Report Achievement
		gpgsDriver.ReportAchievement("OneSmallStep");
		//Increment Achievement
		gpgsDriver.ReportAchievement("AvidSwiper", 1);
		asteroidsCaught = 0;//reset asteroid tracker

		//If there are asteroids leftover on the screen, repool them instantly
		while(activeAsteroids.Count != 0){
			activeAsteroids.ElementAt(0).GetComponent<SphereCollider> ().enabled = false;
			activeAsteroids.ElementAt(0).GetComponent<Asteroid>().originalSpeed = 0;
			activeAsteroids.ElementAt(0).GetComponent<Rigidbody> ().isKinematic = true;
			activeAsteroids.ElementAt(0).transform.position = starCharacter.transform.position;
			activeAsteroids.ElementAt(0).transform.parent = starCharacter.transform;

			StartCoroutine (Repool (activeAsteroids.ElementAt(0), poolingPosition, 0f));
		}
		//Reset Star's position and velocity
		starCharacter.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		starCharacter.transform.position = new Vector3(0,1,0);
		//Set timescale back to normal
		Time.timeScale = 1f;
		//If this is the first game loop since program has opened.
		startPanel.SetActive (false);
		//If this is a restart set the game over panel inactive as well
		gameOverPanel.SetActive(false);
		starCharacter.SetActive (true);
		scoreText.SetActive (true);
		score = 0;
		multiplierNum = 1;
		scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
		gameObject.GetComponent<AsteroidSpawner> ().maxThrust = 20;//Reset base asteroid speed

		//Game is starting
		isGameOver = false;
		StartCoroutine (AsteroidTimer(3.0f));
	}

	void GameOver(){
		//End game
		isGameOver = true;
		Time.timeScale = 0f;
		scoreText.SetActive (false);
		gameOverPanel.SetActive (true);
		GameObject.Find ("/Canvas/GameOverPanel/EndScoreText").GetComponent<Text> ().text = "Score : " + score.ToString();

		StopAllCoroutines ();
		//Submit the score to the Google Play leaderboards
		gpgsDriver.ReportScore(score);
		//Report progress for achievements
		gpgsDriver.ReportAchievement("TheNovice", asteroidsCaught);
		gpgsDriver.ReportAchievement("TheApprentice", asteroidsCaught);
		gpgsDriver.ReportAchievement ("TheJourneyman", asteroidsCaught);
		gpgsDriver.ReportAchievement("TheMaster", asteroidsCaught);

		//TODO:
		//Load best personal high score and display it
		//GameObject.Find("/Canvas/GameOverPanel/BestScoreText").GetComponent<Text>().text = ???

	}

	//Returns the current score of the game
	public int GetScore(){
		return(score);
	}


	public IEnumerator lerpScore(int x){
		int countingUp = 0;
		while (countingUp<x) {
			score += 1;
			scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
			countingUp += 1;
			multiplier = true;
			yield return new WaitForFixedUpdate ();
		}
		multiplier = false;
		scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
	}
}
