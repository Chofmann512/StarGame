using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Enter main game loop
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

}
