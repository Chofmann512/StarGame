using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Class used on the GameManager gameobject to help control
//scene flow/actions from UI elements such as buttons
public class UIDriver : MonoBehaviour {

	//Serialize drop down?
	public GameObject settingsPanel;
	public GameObject startPanel;
	public GameObject gameOverPanel;
	public GameObject instructionsPanel;
	public Button activeMusicButton;
	public Button activeSoundFXButton;
	public Button mutedMusicButton;
	public Button mutedSoundFXButton;
	public Button googleSignInButton;
	public Button googleSignOutButton;

	[SerializeField]
	private string facebookURL;
	[SerializeField]
	private string twitterURL;
	private enum UI {Menu, Settings, Instructions, GameOver};
	private UI curUI;

	public void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			HandleEscapeInput ();
		}
	}

	public void Start(){

		//If this is the very first time playing, show instructions
		if(PlayerPrefs.GetInt("Tutorial") != 1){
			//Pop-up instructions
			ToggleInstructionsPanel();
			curUI = UI.Instructions;
			//flag that the user has seen the instructions for the first time
			PlayerPrefs.SetInt("Tutorial", 1);
			return;
		}
		curUI = UI.Menu;
	}

	public void ReplayGame(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void LoadFacebookPage(){
		if (facebookURL == "") {
			Debug.LogError ("Missing a reference to the URL...");
		} 
		else {
			Application.OpenURL (facebookURL);
		}


	}

	public void LoadTwitterPage(){
		if (twitterURL == "") {
			Debug.LogError ("Missing a reference to the URL...");
		} 
		else {
			Application.OpenURL (twitterURL);
		}
	}

	public void LoadMainMenu(){
		settingsPanel.SetActive (false);
		gameOverPanel.SetActive (false);

		Replay.isReplay = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ToggleSignInButtons(){
		if (googleSignInButton.IsActive ()) {
			googleSignInButton.gameObject.SetActive (false);
			googleSignOutButton.gameObject.SetActive (true);
		} else {
			googleSignInButton.gameObject.SetActive (true);
			googleSignOutButton.gameObject.SetActive (false);
		}
	}

	public void ToggleGameOverPanel(){
		gameOverPanel.SetActive (true);
		curUI = UI.GameOver;
	}

	public void ToggleSettingsPanel(){
		settingsPanel.SetActive (!settingsPanel.activeSelf);
		if (settingsPanel.activeSelf) {
			curUI = UI.Settings;
		} else {
			curUI = UI.Menu;
		}
	}

	public void ToggleInstructionsPanel(){
		instructionsPanel.SetActive (!instructionsPanel.activeSelf);
		if (instructionsPanel.activeSelf) {
			curUI = UI.Instructions;
		} else {
			curUI = UI.Menu;
		}
	}

	public void ToggleSoundFX(){
		if(activeSoundFXButton.IsActive()){
			
			activeSoundFXButton.gameObject.SetActive (false);
			mutedSoundFXButton.gameObject.SetActive (true);

			//TODO: Mute the SoundFX sound channel
		}
		else if(mutedSoundFXButton.IsActive()){
			activeSoundFXButton.gameObject.SetActive (true);
			mutedSoundFXButton.gameObject.SetActive (false);

			//TODO: Unmute the SoundFX sound channel
		}
	}

	public void ToggleMusic(){
		if(activeMusicButton.IsActive()){
			activeMusicButton.gameObject.SetActive (false);
			mutedMusicButton.gameObject.SetActive (true);

			//TODO: Mute the music sound channel
		}
		else if(mutedMusicButton.IsActive()){
			activeMusicButton.gameObject.SetActive (true);
			mutedMusicButton.gameObject.SetActive (false);

			//TODO: Unmute the music sound channel
		}
	}

	public void HandleEscapeInput(){
		switch(curUI)
		{
		case UI.Menu:
			Application.Quit();
			break;

		case UI.Settings:
			ToggleSettingsPanel ();
			break;
		
		case UI.Instructions:
			ToggleInstructionsPanel ();
			break;

		case UI.GameOver:
			LoadMainMenu ();
			break;
		default:
			Debug.LogError ("Found a new case for handling escape key input.");
			break;
		}
	}
}
