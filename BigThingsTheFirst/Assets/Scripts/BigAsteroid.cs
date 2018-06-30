using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAsteroid : Asteroid {

    public float health;

    private GameDriver gameDriver;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        this.gameObject.SetActive(true);
        gameDriver = GameObject.Find("GameDriver").GetComponent<GameDriver>();
    }

    void Start () {
        //Set color in here

	}

    public void Hit(float dmg) {
        Debug.Log("BigAsteroid took 1 damage.");
        health -= dmg;

        if (health < 1.0f) {
            Death();
        }
    }

    public void Death() {
        Debug.Log("BigAsteroid has been destroyed!");
        Destroy(this.gameObject);
        //Add flat amount to score
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name != "AsteroidEnterTrigger")
        {
            return;
        }

        originalSpeed = rb.velocity.magnitude;
        StartCoroutine(SetInitialized());

        rb.mass = 80.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Asteroid") {
            gameDriver.AsteroidCollision(this.gameObject, collision.collider.gameObject);
        }
    }

    IEnumerator SetInitialized() {
        yield return new WaitForSeconds(2.1f);

        isInitialized = true;
        gameObject.GetComponent<SphereCollider>().isTrigger = false;
        originalSpeed = 3.0f;
    }
}
