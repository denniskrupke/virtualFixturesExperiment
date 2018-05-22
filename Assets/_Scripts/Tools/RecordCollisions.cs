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
	
	void Start () {
		collisionList_graspedObject = new List<StampedCollision> ();
		collisionList_gripper = new List<StampedCollision> ();
	}

	public void AddCollision_graspedObject(Vector3 pos, string other){
		StampedCollision collision = new StampedCollision ();
		collision.position = transform.position;		
		collision.other = other;	
		collision.timeInMillis = ExperimentDataLogger.CalculateCurrentTimeStamp();
		collisionList_graspedObject.Add (collision);
	}

	public void AddCollision_gripper(Vector3 pos, string other){
		StampedCollision collision = new StampedCollision ();
		collision.position = transform.position;		
		collision.other = other;	
		collision.timeInMillis = ExperimentDataLogger.CalculateCurrentTimeStamp();
		collisionList_gripper.Add (collision);
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
