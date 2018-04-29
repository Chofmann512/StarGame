using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpriteScroll : MonoBehaviour {
	public float ScrollX = .5f;
	public float ScrollY = .5f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		float OffsetX = Time.time * ScrollX;
		float OffsetY = Time.time * ScrollY;
		GetComponent<RawImage>().material.mainTextureOffset= new Vector2 (OffsetX, OffsetY);
	}
}
