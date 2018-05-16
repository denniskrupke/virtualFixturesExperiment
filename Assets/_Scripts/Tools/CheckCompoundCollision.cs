using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCompoundCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.CompareTag("targetObject"))
        // {
        //     //            UpdateGlobalErrorState(true);            
        //     UpdateGlobalErrorCount();
        //     rend.material.color = Color.black;
        // }
        // else if (other.gameObject.CompareTag("gripper")) {
        //     Debug.Log("collision on the gripper");
        //     UpdateGripperCollisionCount();
        // }
         Debug.Log("collision on the gripper");
    }
}
