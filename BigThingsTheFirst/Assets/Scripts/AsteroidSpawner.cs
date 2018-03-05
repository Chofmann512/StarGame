//**************************************************************************************************************************************************
//
//
//TODO: Add algorithm to increase chance of more force being added to each asteroid as the game progresses,
//		also add algorithm of spawning different asteroids (color/shape).
//
//
//**************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

	//[Debug]
	public GameObject testSpawnPoint;

	private GameObject curAsteroid;//The most recently spawned asteroid
	private Vector3 upperLeftCorner;
	private Vector3 upperRightCorner;
	private Vector3 bottomLeftCorner;
	private Vector3 bottomRightCorner;
	private Vector3 dirUpperLeftCorner;
	private Vector3 dirUpperRightCorner;
	private Vector3 dirBottomLeftCorner;
	private Vector3 dirBottomRightCorner;

	private Vector3 spawnPosition;

    private int side;//1 = left, 2 = right, 3 = top, 4 = bottom

	// Use this for initialization
	void Start () {
		upperLeftCorner = GameObject.Find("/SpawnBoundaries/UpperLeftCorner").transform.position;
		upperRightCorner = GameObject.Find("/SpawnBoundaries/UpperRightCorner").transform.position;
		bottomLeftCorner = GameObject.Find("/SpawnBoundaries/BottomLeftCorner").transform.position;
		bottomRightCorner = GameObject.Find("/SpawnBoundaries/BottomRightCorner").transform.position;

		dirUpperLeftCorner = GameObject.Find("/SpawnBoundaries/DirectionalUpperLeftCorner").transform.position;
		dirUpperRightCorner = GameObject.Find("/SpawnBoundaries/DirectionalUpperRightCorner").transform.position;
		dirBottomLeftCorner = GameObject.Find("/SpawnBoundaries/DirectionalBottomLeftCorner").transform.position;
		dirBottomRightCorner = GameObject.Find("/SpawnBoundaries/DirectionalBottomRightCorner").transform.position;
		Debug.Log ("Initialized upperLeftCorner at : " + GameObject.Find("/SpawnBoundaries/UpperLeftCorner").transform.position);
	}

    public void SpawnAsteroid() {
		//Compute which side to spawn an asteroid on
        side = Random.Range(1, 5);//Max bound for ints is exclusive so this will pick either 1, 2, 3, or 4.

		//Compute an origin spawn point on the given side
        switch (side) {
		case 1://left side
			spawnPosition = new Vector3 (upperLeftCorner.x, upperLeftCorner.y, Random.Range (bottomLeftCorner.z, upperLeftCorner.z));
                break;

            case 2://right side
			spawnPosition = new Vector3(upperRightCorner.x, upperRightCorner.y, Random.Range(bottomRightCorner.z, upperRightCorner.z));
                break;

            case 3://top side
			spawnPosition = new Vector3(Random.Range(upperLeftCorner.x, upperRightCorner.x), upperLeftCorner.y, upperLeftCorner.z);
                break;

            case 4://bottom side
			spawnPosition = new Vector3(Random.Range(bottomLeftCorner.x, bottomRightCorner.x), bottomLeftCorner.y, bottomLeftCorner.z);
                break;

            default:
                Debug.LogError("Could not compute the correct side to spawn an asteroid on.");
                break;
        }

		curAsteroid = Instantiate (testSpawnPoint, spawnPosition, testSpawnPoint.transform.rotation) as GameObject;
		ApplyForce(curAsteroid);

		Debug.Log ("Spawn position of an asteroid : " + spawnPosition);
    }

	private void ApplyForce(GameObject asteroid){
		//Calculate direction
		Vector3 dir = new Vector3(Random.Range(dirUpperLeftCorner.x, dirUpperRightCorner.x), dirUpperLeftCorner.y, Random.Range(dirUpperLeftCorner.z, dirBottomLeftCorner.z));
		dir = dir - asteroid.transform.position;

		asteroid.GetComponent<Rigidbody> ().AddForce (dir * 20);
	}
}
