using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteScroll : MonoBehaviour {
	public float ScrollX = .5f;
	public float ScrollY = .5f;

	public float OffsetX;
	public float OffsetY;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		OffsetX = Time.time * ScrollX;
		OffsetY = Time.time * ScrollY;
		GetComponent<RawImage>().uvRect= new Rect (OffsetX, OffsetY,1,1);
	}
}
