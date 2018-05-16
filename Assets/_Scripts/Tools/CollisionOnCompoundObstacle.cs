using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionOnCompoundObstacle : MonoBehaviour {

    bool isCollidingWithGraspedObject;
    int collisionsWithGripper;

	// Use this for initialization
	void Start () {
        isCollidingWithGraspedObject = false;
        collisionsWithGripper = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCollidingWithGraspedObject(bool colliding)
    {
        isCollidingWithGraspedObject = colliding;
    }

    public bool IsCollidingWithGraspedObject()
    {
        return isCollidingWithGraspedObject;
    }

    public void IncreaseCollisionsWithGripper()
    {
        collisionsWithGripper++;
    }

    public void DecreaseCollisionsWithGripper()
    {
        collisionsWithGripper--;
    }

    public int GetCollisionsWithGripper()
    {
        return collisionsWithGripper;
    }
}
