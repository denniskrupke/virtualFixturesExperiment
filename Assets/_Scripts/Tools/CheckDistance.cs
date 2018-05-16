using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Needs to be attached to an obstacle element. Calculates the shortest distance between the surfaces of a movable sphere and an obstacle.
 * TODO: Distance calculation for other movable/trackable objects than a sphere
 * 
 */
public class CheckDistance : MonoBehaviour {

	public bool visualDebugging = false;

	//float distanceFromCenter = 100.0f;
	Transform movingObject; // the tracked/moving object for measuring the distance between the surfaces

	float distanceApproximatedBetweenSurfaces = 100.0f;
	Vector3 closestPoint;
	Vector3 closestPoint2;


	// Use this for initialization
	void Start () {
		movingObject = GameObject.FindGameObjectWithTag ("targetObject").transform;
	}
	
	// Update is called once per frame
	void Update () {		
		var collider = GetComponent<Collider>();
		if (!collider)
			return; // nothing to do without a collider
		closestPoint = collider.ClosestPoint(movingObject.position);

		var collider2 = movingObject.GetComponent<Collider> ();
		if (!collider2)
			return; // nothing to do without a collider
		closestPoint2 = collider2.ClosestPoint(closestPoint);

		//distanceFromCenter = Vector3.Distance (closestPoint, movingObject.position);//- 0.0568f;
		distanceApproximatedBetweenSurfaces = Vector3.Distance (closestPoint, closestPoint2);
	}
		

	public void OnDrawGizmos()
	{		
		if (visualDebugging) {
			Gizmos.DrawSphere (movingObject.position, 0.01f);
			Gizmos.color = new Color (1, 0, 0);
			Gizmos.DrawSphere (closestPoint, 0.01f);
			Gizmos.color = new Color (1, 1, 1);
			Gizmos.DrawWireSphere(closestPoint2, 0.01f);
		}			
	}

//	public float GetDistanceFromCenter(){
//		return distanceFromCenter;
//	}

	public float GetDistanceApproximatedBetweenSurfaces(){
		return distanceApproximatedBetweenSurfaces;
	}

//	public Vector3 GetClosestPoint(){
//		return closestPoint;
//	}
}
