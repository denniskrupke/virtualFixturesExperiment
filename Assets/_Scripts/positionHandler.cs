using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionHandler : MonoBehaviour {

    public Vector3 startingPosition;
    private Vector3 startingPosition_1;
    private Vector3 startingPosition_2;
    private Vector3 startingPosition_3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // gets called on successful drop and resets the robots position.
    public void resetPosition()
    {
        this.transform.position = startingPosition;
    }

}
