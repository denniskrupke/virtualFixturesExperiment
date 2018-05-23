using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosestObstacle : MonoBehaviour {

	GameObject closestObject;
	float closestDistance;
	bool recording = false;
	List<KeyValuePair<float,bool>> accuracyList;
	//List<float> distanceList;
	//List<bool> collisionList;
	ExperimentDataLogger dataLogger;


	// Use this for initialization
	void Start () {		
		closestDistance = float.MaxValue;
		closestObject = this.transform.gameObject;
		accuracyList = new List<KeyValuePair<float, bool> >();
	//	distanceList = new List<float>();
	//	collisionList = new List<bool>();
		dataLogger = GameObject.FindGameObject("ExperiementController").GetComponent<ExperimentDataLogger>();
	}
	
	// Update is called once per frame
	void Update () {
		CheckDistance[] distances = GetComponentsInChildren<CheckDistance> ();
		foreach (CheckDistance dist in distances) {
			if (dist.GetDistanceApproximatedBetweenSurfaces() < closestDistance) {		
				if (dist.transform.gameObject != closestObject) {
					if (closestObject.GetComponent<HighlightObject> () != null)	closestObject.GetComponent<HighlightObject> ().SetHighlighted (false);												
					closestObject = dist.transform.gameObject;
					//closestObject.GetComponent<HighlightObject> ().SetHighlighted (true);
					//Debug.Log ("found closer object: "+closestObject.name);
				}
				closestDistance = dist.GetDistanceApproximatedBetweenSurfaces();
			}
			closestDistance = closestObject.GetComponent<CheckDistance> ().GetDistanceApproximatedBetweenSurfaces ();
			//Debug.Log (closestDistance);
		}
		if (recording) {
			//bool colliding = GameObject.FindGameObjectWithTag ("targetObject").GetComponent<CheckForCollision> ().IsColliding ();
			//accuracyList.Add(new KeyValuePair<float, bool>(closestDistance, colliding));
			accuracyList.Add(new KeyValuePair<float, bool>(closestDistance, dataLogger.IsColliding()));
			//distanceList.Add (closestDistance);
			//collisionList.Add (closestObject.GetComponent<Collider>().)
			// TODO: mark collision
		}
	}

	public GameObject GetClosestObstacle(){
		return closestObject;
	}

	public float GetClosestDistance(){
		return closestDistance;
	}

	public void StartRecording(){
		recording = true;
	}

	public void StopRecording(){
		recording = false;
	}

	public void ClearData(){
//		distanceList.Clear ();
//		collisionList.Clear ();
		accuracyList.Clear ();
		closestDistance = float.MaxValue;
		closestObject = this.transform.gameObject;
	}

	public List<KeyValuePair<float,bool>> GetAccuracyList(){
		return accuracyList;
	}

}
