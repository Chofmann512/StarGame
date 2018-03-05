﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flick : MonoBehaviour {
	public float flickForce;
	public Transform startFlick;
	public Transform endFlick;
	public float flickTimer;
	private bool flickBool;
	private bool startMove;
	public float maxDistanceGrab;
	// Use this for initialization
	void Start () {
		flickBool = false;
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (Vector3.Distance(v3, startFlick.position) < maxDistanceGrab) {
				startMove = true;
				flickBool = true;
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
		gameObject.GetComponent<Rigidbody> ().AddForce ((startFlick.forward * flickForce /(flickTimer*4))* Vector3.Distance(startFlick.position,endFlick.position)); 

		flickBool = false;
		flickTimer = 0;
	}
}
