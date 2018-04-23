using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDriver : MonoBehaviour {
	public float flickForce;
	public Transform startFlick;
	public Transform endFlick;
	public float flickTimer;
	private bool flickBool;
	private bool startMove;
	public float maxDistanceGrab;
	public GameDriver gameDriver;
	public string color;
	private Vector3 startV3;
	private Vector3 direction;
	public static int bankShot;


	// Use this for initialization
	void Start () {
		flickBool = false;

		if(gameDriver == null){
			Debug.LogError ("StarDriver is missing a reference to GameDriver, please drag a reference in.");
		}
	}
	// Update is called once per frame
	void Update () {
		if(!startMove)
		startFlick.position = new Vector3 (transform.position.x, 3, transform.position.z);
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (Vector3.Distance(v3, startFlick.position) < maxDistanceGrab) {
				gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				CancelInvoke ("cancelBankShot");
				bankShot = 1;
				startMove = true;
				flickBool = true;
				startFlick.position = v3;
				startV3 = startFlick.position;
			} else {
				flickBool = false;
				startMove = false;
			}
		}
		if (Input.GetButtonUp ("Fire1")&& startMove) {
			
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			endFlick.transform.position = v3;
			startFlick.LookAt (endFlick);
			forceAdd ();
		}
		if (flickBool)
			flickTimer += Time.fixedDeltaTime;
	}
	void forceAdd(){

		direction = (startV3 - endFlick.position).normalized;
		startFlick.position = new Vector3 (transform.position.x, 3, transform.position.z);
		float dist = Vector3.Distance (startV3, endFlick.position);
		if (flickTimer < .02)
			flickTimer = .02f;
		if (dist > 10)
			dist = 10;
		gameObject.GetComponent<Rigidbody> ().AddForce ((-direction * flickForce /(flickTimer*80))* dist,ForceMode.Impulse); 
		//gameObject.GetComponent<Rigidbody> ().velocity.magnitude
		flickBool = false;
		flickTimer = 0;
		startMove = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Asteroid") {
			gameDriver.AsteroidCollision (this.gameObject, other.gameObject);
		}
		else if(other.tag == "Boundary"){
			BankShot ();
		}
		else {
			return;
		}
	}

	public void BankShot(){
		bankShot = 2;
		CancelInvoke ("cancelBankShot");
		Invoke ("cancelBankShot", 1f);
	}
	public void cancelBankShot(){
		
		bankShot = 1;

	}

}
