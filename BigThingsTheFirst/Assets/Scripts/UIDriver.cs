using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDriver : MonoBehaviour {

	//Serialize drop down?
	public GameObject settingsPanel;
	public GameObject startPanel;

	[SerializeField]
	private string facebookURL;
	[SerializeField]
	private string twitterURL;

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


		startPanel.SetActive (true);
	}

	public void ToggleSettingsPanel(){
		settingsPanel.SetActive (!settingsPanel.activeSelf);
	}
}
