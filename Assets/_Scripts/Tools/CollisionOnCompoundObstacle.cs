using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionOnCompoundObstacle : MonoBehaviour {

    int collisionsWithGraspedObject;
    int collisionsWithGripper;

	// Use this for initialization
	void Start () {
        collisionsWithGraspedObject = 0;
        collisionsWithGripper = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void IncreaseCollisionsWithGraspedObject()
    {
        collisionsWithGraspedObject++;
    }

    public void DecreaseCollisionsWithGraspedObject()
    {
        collisionsWithGraspedObject--;
    }

    public int GetCollisionsWithGraspedObject()
    {
        return collisionsWithGraspedObject;
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
