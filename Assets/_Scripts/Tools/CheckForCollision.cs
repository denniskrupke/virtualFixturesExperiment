using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Checks if a collision with any obstacle occurs
 */

public class CheckForCollision : MonoBehaviour {

	bool colliding;

	// Use this for initialization
	void Start () {
		colliding = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other){
		if (other.gameObject.CompareTag("obstacle"))
			colliding = true;		
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.CompareTag("obstacle"))
			colliding = false;
	}

	public bool IsColliding(){
		return colliding;
	}
}
