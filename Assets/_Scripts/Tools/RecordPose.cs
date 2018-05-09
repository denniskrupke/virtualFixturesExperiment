using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StampedPose{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 euler;
	public long timeInMillis;
}

public class RecordPose : MonoBehaviour {
//	List<Vector3> positions;
//	List<Quaternion> rotations;
//	List<Vector3> eulers;
//	List<long> timeInMillis;

	List<StampedPose> poseList;

	bool recording = false;


	// Use this for initialization
	void Start () {
//		positions = new List<Vector3> ();
//		rotations = new List<Quaternion> ();
//		eulers = new List<Vector3> ();
//		timeInMillis = new List<long> ();

		poseList = new List<StampedPose> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (recording) {
//			positions.Add (transform.position);
//			rotations.Add (transform.rotation);
//			eulers.Add (transform.rotation.eulerAngles);
//			timeInMillis.Add (System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000 + System.DateTime.Now.Minute * 60 * 1000 + System.DateTime.Now.Hour * 60 * 60 * 1000);

			StampedPose pose = new StampedPose ();
			pose.position = transform.position;
			pose.rotation = transform.rotation;
			pose.euler = transform.rotation.eulerAngles;
			pose.timeInMillis = System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000 + System.DateTime.Now.Minute * 60 * 1000 + System.DateTime.Now.Hour * 60 * 60 * 1000;

			poseList.Add (pose);
		}
	}

	public void StartRecording(){
		recording = true;
	}

	public void StopRecording(){
		recording = false;
	}

	public void ClearData(){
//		positions.Clear ();
//		rotations.Clear ();
//		eulers.Clear ();
//		timeInMillis.Clear ();

		poseList.Clear ();
	}

//	public List<Vector3> GetPositions(){
//		return positions;
//	}
//
//	public List<Quaternion> GetRotations(){
//		return rotations;
//	}
//
//	public List<Vector3> GetEulers(){
//		return eulers;
//	}
//
//	public List<long> GetTimeInMillis(){
//		return timeInMillis;
//	}

	public List<StampedPose> GetPoseList(){
		return poseList;
	}
}
