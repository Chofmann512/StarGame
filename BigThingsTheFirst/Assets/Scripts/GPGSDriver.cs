using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class GPGSDriver : MonoBehaviour {

	[SerializeField]
	private Text signInButtonText;
	[SerializeField]
	private Text authStatus;

	// Use this for initialization
	void Start () {
		GameObject startButton = GameObject.Find ("startButton");
		EventSystem.current.firstSelectedGameObject = startButton;

		//Create GPG client
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

		//Set debugging output to true
		PlayGamesPlatform.DebugLogEnabled = true;

		//Initialize and activate the platform
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate ();

		// Try silent sign-in (second parameter is isSilent)
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
	}

	public void SignInCallback(bool success){
		if (success) {
			//login succeeded
			Debug.Log ("(Star Swipe) Signed in!");
			signInButtonText.text = "Sign out";

			authStatus.text = "Signed in as: " + Social.localUser.userName;
		} 
		else {
			//login failed
			signInButtonText.text = "Sign in";
			authStatus.text = "Sign-in failed";
		}
	}
	
	public  void SignIn(){
		if(!PlayGamesPlatform.Instance.localUser.authenticated){
			// TODO:
			// Sign in with Play Game Services, showing the consent dialog
			// by setting the second parameter to isSilent=false.
			PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
		}
		else{
			// Sign out of play games
			PlayGamesPlatform.Instance.SignOut();

			// Reset UI
			signInButtonText.text = "Sign In";
			authStatus.text = "";
		}

	}

	public void ShowAchievements(){
		if(PlayGamesPlatform.Instance.localUser.authenticated){
			PlayGamesPlatform.Instance.ShowAchievementsUI ();
		}
		else{
			Debug.Log ("Unable to show Achievements, not logged in to Google Play");
		}
	}

	public void ShowLeaderboards(){
		if(PlayGamesPlatform.Instance.localUser.authenticated){
			PlayGamesPlatform.Instance.ShowLeaderboardUI ();
		}
		else{
			Debug.Log ("Unable to show Leaderboard, not logged in to Google Play");
		}
	}
}
