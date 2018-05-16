using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visualFeedback_obstacle : MonoBehaviour {
    
    private GameObject targetObject;
    private Renderer rend;
    private Color originalColor;

    private int gripperCollisionCount;
    private bool graspedObjectIsCollidingWithObstacle;

    CollisionOnCompoundObstacle compoundObstacleHandler;
    ExperimentDataLogger experimentLogger;

    public bool visualizeCollisions = false;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        gripperCollisionCount = 0;
        graspedObjectIsCollidingWithObstacle = false;
        compoundObstacleHandler = null;        
    }
	
	// Update is called once per frame
	void Update () {
        targetObject = GameObject.FindGameObjectWithTag("targetObject");

        // calculating obstacle-targetObject distance
        //Append2GlobalObstacleDistance(MinimumObstacleDistance());
    }

    void OnTriggerEnter(Collider other) {
        compoundObstacleHandler = gameObject.GetComponentInParent<CollisionOnCompoundObstacle>();
        experimentLogger = GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>();

        if (other.gameObject.CompareTag("targetObject")) //Collision with grasped object
        {
            if(compoundObstacleHandler != null) //compound obstacle
            {
                if(compoundObstacleHandler.GetCollisionsWithGraspedObject() == 0) {                    
                    UpdateGlobalErrorCount();                        
                    experimentLogger.SetColliding(true);        
                }
                compoundObstacleHandler.IncreaseCollisionsWithGraspedObject();                
            }
            else  {// simple obstacle
                graspedObjectIsCollidingWithObstacle = true;
                UpdateGlobalErrorCount();                
                experimentLogger.SetColliding(true);        
            }
            if(visualizeCollisions) rend.material.color = Color.red;
        }
        else if (other.gameObject.CompareTag("gripper")) { //Collision with robot           
                if(compoundObstacleHandler != null) //compound obstacle
                {
                    if(compoundObstacleHandler.GetCollisionsWithGripper() == 0)
                    {                        
                        if (experimentLogger.IsGrabbed()) UpdateGripperCollisionCount();
                        experimentLogger.SetColliding(true);
                    }
                    compoundObstacleHandler.IncreaseCollisionsWithGripper();
                }
                else {//simple obstacle
                {
                    if (gripperCollisionCount == 0 && experimentLogger.IsGrabbed()) UpdateGripperCollisionCount();    //causes counting collision
                    gripperCollisionCount++;
                    experimentLogger.SetColliding(true);    //causes haptic feedback
                }
            }                
        }        
    }    
    
    private void OnTriggerExit(Collider other) {
        compoundObstacleHandler = gameObject.GetComponentInParent<CollisionOnCompoundObstacle>();
        if (visualizeCollisions) rend.material.color = originalColor;

        if (other.gameObject.CompareTag("targetObject"))
        {
            if(compoundObstacleHandler != null) //compound obstacle
            {
                compoundObstacleHandler.DecreaseCollisionsWithGraspedObject();
                if(compoundObstacleHandler.GetCollisionsWithGripper() == 0 
                    && compoundObstacleHandler.GetCollisionsWithGraspedObject() == 0
                    && gripperCollisionCount == 0) experimentLogger.SetColliding(false);        
            }
            else {// simple obstacle
                graspedObjectIsCollidingWithObstacle = false;
                if(gripperCollisionCount == 0) experimentLogger.SetColliding(false);
            }            
        }
        else if (other.gameObject.CompareTag("gripper")) {
            if(compoundObstacleHandler != null) //compound obstacle
            {
                compoundObstacleHandler.DecreaseCollisionsWithGripper();
                if(compoundObstacleHandler.GetCollisionsWithGripper() == 0 
                    && compoundObstacleHandler.GetCollisionsWithGraspedObject() == 0
                    && gripperCollisionCount == 0) experimentLogger.SetColliding(false);                
            }
            else {
                gripperCollisionCount--;
                if(gripperCollisionCount == 0 
                    && !graspedObjectIsCollidingWithObstacle) experimentLogger.SetColliding(false);    
            }                
        }
    }

    private void UpdateGlobalErrorCount() {
        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().errorCount++;
        Debug.Log("objectCollisions: " + experimentLogger.errorCount);
    }

    private void UpdateGripperCollisionCount(){
        GameObject.Find("ExperimentController").GetComponent<ExperimentDataLogger>().gripperCollisionCount++;
        Debug.Log("gripperCollisions: " + experimentLogger.gripperCollisionCount);   
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
