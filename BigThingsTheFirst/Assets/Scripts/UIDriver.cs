using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDriver : MonoBehaviour {

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
		
	}
}
