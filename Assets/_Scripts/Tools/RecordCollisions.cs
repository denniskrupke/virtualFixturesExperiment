using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampedCollision{
	public Vector3 position;// if available	
	public string other;// the other object
	public long timeInMillis;
}


public class RecordCollisions : MonoBehaviour {
	List<StampedCollision> collisionList_graspedObject;
	List<StampedCollision> collisionList_gripper;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;

            collisionList_graspedObject = new List<StampedCollision>();
            collisionList_gripper = new List<StampedCollision>();
        }
    }

    void Start () {
		
	}

	public void AddCollision_graspedObject(Vector3 pos, string other){
        //Debug.Log("adding object collision");
		StampedCollision collision = new StampedCollision ();
		collision.position = transform.position;		
		collision.other = other;	
		collision.timeInMillis = ExperimentDataLogger.CalculateCurrentTimeStamp();
		collisionList_graspedObject.Add (collision);
        //Debug.Log(collisionList_graspedObject.Count);
    }

	public void AddCollision_gripper(Vector3 pos, string other){
        //Debug.Log("adding gripper collision");
        StampedCollision collision = new StampedCollision ();
		collision.position = transform.position;		
		collision.other = other;	
		collision.timeInMillis = ExperimentDataLogger.CalculateCurrentTimeStamp();
		collisionList_gripper.Add (collision);
        //Debug.Log(collisionList_gripper.Count);
	}

	public void ClearData(){
		collisionList_graspedObject.Clear ();
		collisionList_gripper.Clear ();
	}

	public List<StampedCollision> GetCollisionList_graspedObject(){
		return collisionList_graspedObject;
	}

	public List<StampedCollision> GetCollisionList_gripper(){
		return collisionList_gripper;
	}
}
