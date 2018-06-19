using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GooglePlayGames;
using UnityEngine.SceneManagement;

public class GameDriver : MonoBehaviour {

	[SerializeField]
	private int score;
	[SerializeField]
	public GameObject starCharacter;
	[SerializeField]
	private float minSpawnTimerBound;
	[SerializeField]
	private float maxSpawnTimerBound;

	public List<GameObject> activeAsteroids = new List<GameObject>();
	public List<GameObject> asteroids;
	public GameObject poolingPosition;
	public static int multiplierNum;
	public AudioSource scoreCount;
    public AudioSource menuMusic;
    public AudioSource gameMusic;
    public AudioSource asteroidCapture;
    public AudioSource asteroidSpawn;
    public GameObject musicManager;
    public GameObject soundEffectManager;
    public UIDriver uiDriver;
    public int remainingScore;

	private GameObject startPanel;
	private GameObject gameOverPanel;
	private GameObject scoreText;
	private GameObject bestScoreText;
	private GPGSDriver gpgsDriver;
	private bool isGameOver = true;//Flag used to tell whether or not the game is playing
	private bool multiplier;
	private int asteroidsCaught;
	private int totalCurrency;//Used to track the total amount of in-game currency
	private int highestScore;//Used to load locally, the highest score achieved
	//Used to track and report achievement progress
	//public float TimeToLerp;

	public void SetTotalCurrency(int x){
		totalCurrency = x;
	}
	public void SetHighestScore(int x){
		highestScore = x;
	}

	public int GetTotalCurrency(){
        //Backklogged
		return(totalCurrency);
	}
	public int GetHighestScore(){
		return(highestScore);
	}

	//Returns the current score of the game
	public int GetScore(){
		return(score);
	}

    private void OnEnable()
    {
        if (musicManager.activeInHierarchy)
        {
            menuMusic.Play();
        }
    }

    void Start () {
		Time.timeScale = 1.0f;//If the scene was reloaded, set timescale back to normal




		multiplierNum = 1;
		score = 0;
		startPanel = GameObject.Find ("/Canvas/StartPanel");
		bestScoreText = GameObject.Find ("/Canvas/GameOverPanel/BestScoreText");
		gameOverPanel = GameObject.Find ("/Canvas/GameOverPanel");
		gameOverPanel.SetActive (false);//This panel needs to start out hidden
		scoreText = GameObject.Find ("/Canvas/Score");
		scoreText.GetComponent<Text> ().text = "Score : " + score.ToString ();
		scoreText.SetActive (false);//Score starts out hidden while on main menu
		LoadHighestScoreLocal();

		gpgsDriver = GetComponent<GPGSDriver> ();
		asteroids = gameObject.GetComponent<AsteroidSpawner> ().asteroids;
		if(poolingPosition == null){
			Debug.LogError ("Missing a reference to a pooling position, please create an empty GO called 'Pooling Position' and place it at (20, 0, 20).");
		}

		//Is this the first StartGame()?
		if(Replay.isReplay == 1){
			StartGame ();
		}
	}

	//An event called by the StarDriver when it detects a collision with an asteroid
	public void AsteroidCollision(GameObject star, GameObject asteroid){
		if(star.GetComponent<StarDriver>().color == asteroid.GetComponent<Asteroid>().color){
			if(StarDriver.bankShot == 2){
				gpgsDriver.ReportAchievement ("BankShot!");
			}
			//TODO: Possibly compress this call below
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
            if(soundEffectManager.activeInHierarchy)
            asteroidCapture.Play();
			if (!multiplier) {
                remainingScore += 100* multiplierNum * StarDriver.bankShot;
				multiplierNum = 1;
                multiplier = true;
			} else {
				if (multiplierNum < 4) {
					multiplierNum++;
                    if (multiplierNum == 2) {
                        //Report Double!
                        gpgsDriver.ReportAchievement("Double!");
                    }
                    else if (multiplierNum == 3) {
                        //Report Triple!
                        gpgsDriver.ReportAchievement("Triple!");
                    }
                    
                    uiDriver.UpdateMultiplierText(multiplierNum*StarDriver.bankShot);
				}
                remainingScore += 100* multiplierNum * StarDriver.bankShot;

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
            asteroidSpawn.Play();
			//Roll for a chance to spawn a second asteroid at the same time
			//Between 0 and 11, max exclusive
			if(Random.Range(0, 11) > 8 && gameObject.GetComponent<AsteroidSpawner>().maxThrust > 20){
				gameObject.GetComponent<AsteroidSpawner>().SpawnAsteroid ();
                asteroidSpawn.Play();
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

        //Pre-processor directive to not call ads in the unity editor
#if UNITY_EDITOR
#else
			//AD//Start loading a new interstitial ad to play when the game ends
			AdDriver.Instance.RequestInterstitialAd();
			//Increment ad interval
			AdDriver.Instance.loadCount++;
#endif

        GameObject.Find("AdDriver").GetComponent<AdDriver>().RemoveBanner();

		//Change to true that a game has been played
		Replay.isReplay = 1;

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
        uiDriver.pauseButton.gameObject.SetActive(true);
		score = 0;
		multiplierNum = 1;
		scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
		gameObject.GetComponent<AsteroidSpawner> ().maxThrust = 6;//Reset base asteroid speed

        //Game is starting
        if (musicManager.activeInHierarchy)
        {
                menuMusic.Stop();
                gameMusic.Play();
        }
		isGameOver = false;
		StartCoroutine (AsteroidTimer(3.0f));
        StartCoroutine(lerpScore());
    }

	void GameOver(){
        //End game
        musicManager.SetActive(false);
        soundEffectManager.SetActive(false);

		isGameOver = true;
		SubmitNewScore (score);
		Time.timeScale = 0f;
		//TODO: Rewrite this using UIDriver for consistency
		scoreText.SetActive (false);
		//gameOverPanel.SetActive (true);
		GetComponent<UIDriver>().ToggleGameOverPanel();
		GameObject.Find ("/Canvas/GameOverPanel/EndScoreText").GetComponent<Text> ().text = "Score : " + score.ToString();//Show ending score for the current round
		bestScoreText.GetComponent<Text>().text = "Best Score : " + highestScore.ToString(); //Show highest score
		GameObject.Find("/Canvas/GameOverPanel").GetComponentInChildren<Animator>().Play("Text_Breathe");


		//Pre-processor directive to not call ads in the unity editor
		#if UNITY_EDITOR
		#else
		//Attempt to show interstitial
		if(AdDriver.Instance.loadCount % AdDriver.Instance.interstitialInterval == 0){
		AdDriver.Instance.loadCount = 0;//Reset the loadcount
		AdDriver.Instance.ShowInterstitial();
		}
		#endif

		StopAllCoroutines ();
		//Submit the score to the Google Play leaderboards
		gpgsDriver.ReportScore(score);

		//Report progress for achievements
		gpgsDriver.ReportAchievement("TheNovice", asteroidsCaught);
		gpgsDriver.ReportAchievement("TheApprentice", asteroidsCaught);
		gpgsDriver.ReportAchievement ("TheJourneyman", asteroidsCaught);
		gpgsDriver.ReportAchievement("TheMaster", asteroidsCaught);

		//Make a Call to Upload Player Data to Google Cloud
		gpgsDriver.OpenSave(true);
	}

	public IEnumerator lerpScore(){
       
        if(remainingScore == 0)
        {
            multiplier = false;
            multiplierNum = 1;
        }
        if (remainingScore > 0)
        {

            int bankShot = 1;
            if (StarDriver.bankShot == 2)
                bankShot = 2;
            else
            {
                bankShot = 1;
            }
            if ((int)(remainingScore / 75) == 0)
            {
                score += 1;
                remainingScore -= 1;
            }
            else { 
            score += (int)(remainingScore / 75);
            remainingScore -= (int)(remainingScore / 75);
             }
            scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
            if (soundEffectManager.activeInHierarchy)
            {
                if (!scoreCount.isPlaying)
                    scoreCount.Play();
            }
            scoreText.GetComponent<Text>().text = "Score : " + score.ToString();
            uiDriver.UpdateMultiplierText(multiplierNum * StarDriver.bankShot);
        }
        yield return new WaitForSeconds(.02f);
        
        StartCoroutine(lerpScore());
	}

    //Checks if the achieved score is a new all time best
    public void SubmitNewScore(int newScore){
		if(newScore > highestScore){
			highestScore = newScore;
			SaveHighestScore();
		}
	}
		
	private void LoadHighestScoreLocal(){
		if(PlayerPrefs.HasKey("Hiscore")){
			highestScore = PlayerPrefs.GetInt ("Hiscore");
		}
	}

	//Sets the locally saved high score to the value held in highestScore
	private void SaveHighestScore(){
		PlayerPrefs.SetInt ("Hiscore", highestScore);
		//TODO:
		//GetComponent<GPGSDriver>(). save game data
	}
}
