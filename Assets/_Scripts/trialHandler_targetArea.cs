using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trialHandler_targetArea : MonoBehaviour {

    private GameObject targetObject;
    private GameObject UR5_Target;
    private bool isColliding;

    private GameObject experimentController;
    private Fader fader;
    private ExperimentDataLogger dataLogger;


	// Use this for initialization
	void Start () {
        experimentController = GameObject.Find("ExperimentController");
        fader = experimentController.GetComponent<Fader>();
        dataLogger = experimentController.GetComponent<ExperimentDataLogger>();
        targetObject = GameObject.FindGameObjectWithTag("targetObject");
        UR5_Target = GameObject.Find("UR5-Target");       
    }
	
	// Update is called once per frame
	void Update () {
		isColliding = false;        
    }   


    // main function for updating trialcount and resetting positions
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("targetObject"))
        {
            if(isColliding) return;
         	isColliding = true;

            //Fading and back
            //Debug.Log("fading");
            fader.Fade();
                
            // update trial count on drop to change scene after trial 3 (0,1,2)
            dataLogger.StopTrial();
            dataLogger.UpdateDataFrame(); //dumps data to files
    		dataLogger.trial++;
            // reset stuff
            targetObject.GetComponent<hapticFeedback_targetObject>().resetPosition();
            UR5_Target.transform.DetachChildren();
            dataLogger.ResetData();            
        }
    }
}
