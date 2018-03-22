﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starJitter : MonoBehaviour {
	public float MaxJitter;
	public float timer;
	public float timeAmount;
	public float finalTime;

	public int curColor;
	public int tarColor;

	public List<Material> StarColors = new List<Material>();

	public List<GameObject> Trails = new List<GameObject> ();
	public List<ParticleSystem> BuildUps = new List<ParticleSystem> ();
	public List<ParticleSystem> ExplodeShells = new List<ParticleSystem> ();
	public List<ParticleSystem> ExplodeOut = new List<ParticleSystem> ();
	// Use this for initialization
	void Start () {
		curColor = Random.Range (0, 4);
		this.GetComponent<Renderer> ().material = StarColors [curColor];

		tarColor = Random.Range (0, 4);
		if (tarColor == curColor) {
			while (tarColor == curColor) {
				tarColor = Random.Range (0, 4);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > timeAmount	) {
			transform.localPosition = new Vector3 (Random.Range (-MaxJitter, MaxJitter)*(timer-timeAmount), 0, Random.Range (-MaxJitter, MaxJitter)*(timer-timeAmount));
			if (finalTime - timer > .7f)
				BuildUps[tarColor].Play ();
			if (timer > finalTime) {
				timer = 0;

				ExplodeShells[tarColor].Play ();
				ExplodeOut[tarColor].Play ();
				transform.localPosition = new Vector3 (0,0,0);
				curColor = tarColor;
				if (tarColor == curColor) {
					while (tarColor == curColor) {
						tarColor = Random.Range (0, 4);
					}
				}
			}
		}

	}
}