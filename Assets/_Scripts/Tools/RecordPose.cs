using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StampedPose{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 euler;
	public Vector3 forward;
	public long timeInMillis;
}


public class RecordPose : MonoBehaviour {
	List<StampedPose> poseList;
	bool recording = false;
	
	void Start () {
		poseList = new List<StampedPose> ();
	}
	
	void Update () {
		if (recording) {
			StampedPose pose = new StampedPose ();

			pose.position = transform.position;
			pose.rotation = transform.rotation;
			pose.euler = transform.rotation.eulerAngles;
			pose.forward = transform.forward;
			pose.timeInMillis = ExperimentDataLogger.CalculateCurrentTimeStamp();

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
		poseList.Clear ();
	}

	public List<StampedPose> GetPoseList(){
		return poseList;
	}
}