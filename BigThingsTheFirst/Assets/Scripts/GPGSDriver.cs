using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class GPGSDriver : MonoBehaviour {

	[SerializeField]
	private Text signInButtonText;
	[SerializeField]
	private Text authStatus;

	// Use this for initialization
	void Start () {
		GameObject startButton = GameObject.Find ("startButton");//TODO: Remove this??
		EventSystem.current.firstSelectedGameObject = startButton;//TODO: Remove this??

		//Create GPG client
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();

		//Set debugging output to true
		PlayGamesPlatform.DebugLogEnabled = true;

		//Initialize and activate the platform
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate ();

		// Try silent sign-in (second parameter is isSilent)
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
	}

	//Callback function after an attempted sign-in 
	public void SignInCallback(bool success){
		if (success) {
			//login succeeded
			Debug.Log ("(Star Swipe) Signed in!");
			OpenSave (false);//Update player data from the google cloud

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

	#region
	//Leaderboard and Achievement functionality
	#endregion

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

	public void ReportScore(int reportedScore){
		if(PlayGamesPlatform.Instance.localUser.authenticated){
			PlayGamesPlatform.Instance.ReportScore(reportedScore, GPGSIds.leaderboard_score, 
				(bool success) => 
				{
					Debug.Log("(Star Swipe) Leaderboard update success: " + success);
				});
		}
	}

	//looooooooooool RIP
	public void ReportAchievement(string which){
		//First check if the user is logged in
		if(Social.localUser.authenticated){
			switch (which) {
			case "OneSmallStep":
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_one_small_step,
					100.0f, (bool success) => {
					Debug.Log ("(Star Swipe) One Small Step Unlock: " +
					success);
				});
				break;

			case "TheBeginner" :
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_the_beginner,
					100.0f, (bool success) => {
						Debug.Log ("(Star Swipe) The Beginner: " +
							success);
				});
				break;

			case "BankShot!" :
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_bank_shot,
					100.0f, (bool success) => {
						Debug.Log ("(Star Swipe) Bank Shot!: " +
							success);
				});
				break;

			case "Double!" :
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_double,
					100.0f, (bool success) => {
						Debug.Log ("(Star Swipe) Double!: " +
							success);
				});
				break;

			case "Tripel!" :
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_triple,
					100.0f, (bool success) => {
						Debug.Log ("(Star Swipe) Triple!: " +
							success);
				});
				break;

			case "Monochromatic" :
				PlayGamesPlatform.Instance.ReportProgress (
					GPGSIds.achievement_monochromatic,
					100.0f, (bool success) => {
						Debug.Log ("(Star Swipe) Monochromatic: " +
							success);
				});
				break;
			}
		}
		else{
			Debug.Log("User is not connected to Google Play Games Services.");
		}

	}

	//Overloaded function for ReportAchievement
	public void ReportAchievement(string which, int incrementAmt){
		//First check if the user is logged in
		if(Social.localUser.authenticated){
			switch (which) {
			case "TheNovice":
				PlayGamesPlatform.Instance.IncrementAchievement (
					GPGSIds.achievement_the_novice,
					incrementAmt, (bool success) => {
						Debug.Log ("(Star Swipe) The Novice Increment of " + incrementAmt + ": " +
							success);
				});
				break;

			case "TheApprentice" :
				PlayGamesPlatform.Instance.IncrementAchievement (
					GPGSIds.achievement_the_apprentice,
					incrementAmt, (bool success) => {
						Debug.Log ("(Star Swipe) The Apprentice Increment of " + incrementAmt + ": " +
							success);
				});
				break;

			case "TheJourneyman" :
				PlayGamesPlatform.Instance.IncrementAchievement (
					GPGSIds.achievement_the_journeyman,
					incrementAmt, (bool success) => {
						Debug.Log ("(Star Swipe) The Journeyman Increment of " + incrementAmt + ": " +
							success);
				});
				break;

			case "TheMaster" :
				PlayGamesPlatform.Instance.IncrementAchievement (
					GPGSIds.achievement_the_master,
					incrementAmt, (bool success) => {
						Debug.Log ("(Star Swipe) The Master Increment of " + incrementAmt + ": " +
							success);
				});
				break;

			//Special case, we want to only increment this by 1 every time. Hard coded for safety
			case "AvidSwiper" :
				PlayGamesPlatform.Instance.IncrementAchievement (
					GPGSIds.achievement_avid_swiper,
					incrementAmt, (bool success) => {
						Debug.Log ("(Star Swipe) Avid Swiper Increment of " + 1 + ": " +
							success);
				});
				break;

			}
		}
		else{
			Debug.Log("User is not connected to Google Play Games Services.");
		}
	}

	#region
	//---------------------------------------------------------------------------------------------------------------
	//Saving and Loading game data functionality
	//---------------------------------------------------------------------------------------------------------------

	private bool isSaving = false;

	//Packages the string to save as META data to the Google Cloud
	//Format is highscore|totalCurrency, for example 21367|22
	private string GetSaveString(){
		string s = "";
		s += PlayerPrefs.GetInt ("Hiscore").ToString ();
		s += "|";
		s += GetComponent<GameDriver> ().GetTotalCurrency().ToString ();

		return(s);
	}

	//Unpacks the loaded string and updates the respective data fields
	private void LoadSaveString(string save){
		string[] data = save.Split ('|');
		//[0] is highscore
		//[1] is currency
		PlayerPrefs.SetInt ("Hiscore", int.Parse (data [0]));
		GetComponent<GameDriver> ().SetTotalCurrency(int.Parse (data[1]));
	}

	public void OpenSave(bool saving){
		Debug.Log ("Open Save");

		if (Social.localUser.authenticated) {
			isSaving = saving;
			((PlayGamesPlatform)Social.Active).SavedGame
				.OpenWithAutomaticConflictResolution ("StarSwipe", GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpened);
			
		}
	}

	//Callback function
	private void SaveGameOpened (SavedGameRequestStatus status, ISavedGameMetadata meta){

		Debug.Log ("SaveGameOpened");
		if(status == SavedGameRequestStatus.Success){
			if(isSaving)//Writing
			{
				byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes (GetSaveString());
				SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder ().WithUpdatedDescription ("Saved at " + DateTime.Now.ToString()).Build();

				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate (meta, update, data, SaveUpdate);
			}
			else//Loading
			{
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(meta, SaveRead);
			}
		}
	}

	//Callback function for Load
	private void SaveRead(SavedGameRequestStatus status, byte[] data){
		if (status == SavedGameRequestStatus.Success) {
			string saveData = System.Text.ASCIIEncoding.ASCII.GetString (data);
			LoadSaveString (saveData);
		}
	}

	//Callback function for whether or not load was successful
	private void SaveUpdate(SavedGameRequestStatus status, ISavedGameMetadata meta){
		Debug.Log (status);
	}

	#endregion
}
