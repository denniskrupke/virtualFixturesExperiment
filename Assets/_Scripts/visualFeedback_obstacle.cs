using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualFeedback_obstacle : MonoBehaviour {

    private Renderer rend;

    // gameObject targetObject to measure the distance to
    private GameObject targetObject;
    private Color originalColor;

    private int gripperCollisionCount;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        gripperCollisionCount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        targetObject = GameObject.FindGameObjectWithTag("targetObject");

        // calculating obstacle-targetObject distance
        //Append2GlobalObstacleDistance(MinimumObstacleDistance());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("targetObject"))
        {         
            UpdateGlobalErrorCount();
            rend.material.color = Color.black;
        }
        else if (other.gameObject.CompareTag("gripper")) {
            if(gripperCollisionCount++ == 0){
                Debug.Log("collision on the gripper");
                UpdateGripperCollisionCount();  
            }                
        }        
    }    
    
    private void OnTriggerExit(Collider other)
    {
        rend.material.color = originalColor; 
        if (other.gameObject.CompareTag("gripper")) {
            gripperCollisionCount--;
        }
    }

    private void UpdateGlobalErrorCount() {
        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().errorCount++;
        Debug.Log(GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().errorCount);
    }

    private void UpdateGripperCollisionCount(){
        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().gripperCollisionCount++;
        Debug.Log(GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().gripperCollisionCount);   
    }

//    private void UpdateGlobalErrorState(bool boolean) {
//        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().error = boolean;
//    }

//    private void Append2GlobalObstacleDistance(float distance) {
//        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().minimum_distance_euklid_targetObject2obstacle_list.Add(distance);
//    }

    // calculates the euclidian distance from targetObject to obstacle
//    private float MinimumObstacleDistance()
//    {
//        // get targetObjectCenter position
//        Vector3 targetObjectCenter = targetObject.GetComponent<Collider>().transform.position;
//
//        // get obstacle surface point
//        Vector3 temp_surfacePoint = GetComponent<Collider>().ClosestPointOnBounds(targetObjectCenter);
//
//        // calculate distance
//        float temp_surfaceDistance = Vector3.Distance(targetObjectCenter, temp_surfacePoint);
//
//        // visualize distance between targetObjectCenter and obstacleNearestSurface (debug)
//        Debug.DrawLine(temp_surfacePoint, targetObjectCenter, Color.green);
//
//        return temp_surfaceDistance;
//    }
}
