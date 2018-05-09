using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trialHandler_targetArea : MonoBehaviour {

    private GameObject targetObject;
    private GameObject UR5_Target;
    private bool isColliding;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		isColliding = false;
        targetObject = GameObject.FindGameObjectWithTag("targetObject");
        UR5_Target = GameObject.Find("UR5-Target");
    }


    // main function for updating trialcount and resetting positions
	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("targetObject"))
        {
        	if(isColliding) return;
     		isColliding = true;

            // reset targetObject to startingPosition
            targetObject.GetComponent<hapticFeedback_targetObject>().resetPosition();
            // detach targetObject from UR5
            UR5_Target.transform.DetachChildren();
            // update trial count on drop to change scene after trial 3 (0,1,2)
            GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().StopTrial();
            GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().UpdateDataFrame(); //dumps data to files
		

            // reset stopTimer to enable easier calculating numbers (trial times arer all positiv)s
            GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().experimentData.timeStamp_stop = 0;
            GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().trial++;
        }
    }
}
