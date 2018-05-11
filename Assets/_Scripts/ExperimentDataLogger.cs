using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ExperimentDataFrame{
	public int id_participant;
	public int method; // true = VF in first trial, false = VF in second trial
	public string course; // 1 = Course1, 2 = Course2, 3 = Course3
	public int trial; // [0...3]
	public int handedness; // 0 = left handedness, 1 = right handedness
	public long timeStamp_start; //when the target appears in system time expressed in milliseconds
	public long timeStamp_stop; //when the goal position was selected by the user in system time expressed in milliseconds
//	public Vector3 position_targetObject; //position of the current cylinder in world coordinates
//	public Vector3 position_targetArea; //position of the current target in world coordinated
//	public Vector3 position_user_eye; //position of the user when the goal position was selected
//	public Vector3 position_user_controller; // position of the user controller/hand
//	public float ur5_shoulder_pan_joint; // current motor position x value
//	public float ur5_shoulder_lift_joint; // current motor position x value
//	public float ur5_elbow_joint; // current motor position x value
//	public float ur5_wrist_1_joint; // current motor position x value
//	public float ur5_wrist_2_joint; // current motor position x value
//	public float ur5_wrist_3_joint; // current motor position x value
//	public float minimum_distance_euklid_targetObject2obstacle; //how far is the user away from the cylinder in meter when spawning
//	public float minimum_distance_euklid_targetObject2targetArea; //how far is the user away from the cylinder in meter at selection time
	//public bool error; //true, if obstacle is hit
	public int errorCount; //number of collisions
	public long time; //time from pickup to drop at targetarea


//	// constructor for initialization
//	public ExperimentDataFrame(){
//
//	}
		
	private long CalculateCurrentTimeStamp(){
		return System.DateTime.Now.Millisecond + System.DateTime.Now.Second*1000 + System.DateTime.Now.Minute*60*1000 + System.DateTime.Now.Hour*60*60*1000;
	}

	public void UpdateStartTime(){
		timeStamp_start = CalculateCurrentTimeStamp();
	}

	public void UpdateStopTime(){
		timeStamp_stop = CalculateCurrentTimeStamp();	
	}
}

// ExperimentDataLogger Behaviour can be attached to GameObject to execute data collection
public class ExperimentDataLogger : MonoBehaviour {
    [Header("General public varibales")]
    public ExperimentFileWriter experimentFileWriter = null;
    public ExperimentSceneManager experimentSceneManager = null;
    public Transform ur5Target = null;
    public Transform targetObject = null;

    /// Static fields that need manual input for each trial/participant

    /// public long timeStamp_start; //when the target appears in system time expressed in milliseconds
    [Header("Trial specific variables")]
    public int id_participant; // insterted manually
    public int trial = 0; // number of trial executed (1,2,3)
    public int handedness; // 0 = left handedness, 1 = right handedness - inserted manually
    public bool error; // public variable gets updated from visualFeedback_obstacle
    public int errorCount; // public variable gets updated from visualFeedback_obstacle
    public List<float> minimum_distance_euklid_targetObject2obstacle_list; // public variable gets updated from visualFeedback_obstacle


    public ExperimentDataFrame experimentData;
    private bool startArgument = false;

	private RecordPose targetPoseRecorder;


    // Use this for initialization
    void Start () {
		experimentData = new ExperimentDataFrame();
		targetPoseRecorder = GameObject.FindGameObjectWithTag ("targetObject").GetComponent<RecordPose> ();        
    }
    
//	// Update is called once per frame
//	void FixedUpdate () {
//         float gripperOnTargetDistance = Vector3.Distance(targetObject.position, ur5Target.position);
//
//        //
//        if (gripperOnTargetDistance < 0.1f)
//        {
//            startArgument = true;
//            Debug.Log("<color=green>Data Collection Started</color>");   
//        }
//        else {
//            startArgument = false;
//            Debug.Log("<color=red>Data Collection Stopped</color>");
//        }
//
//        // update DataFrame variables
//        if (startArgument) {
//            UpdateDataFrame();
//        }
//    }
		
	public ExperimentDataFrame GetExperimentData(){
		return experimentData;
	}

    // while the trial is running, update the dataframe with current values und append to file
    public void UpdateDataFrame() {
        experimentData.id_participant = id_participant;
        experimentData.method = GetComponent<ExperimentSceneManager>().VF_first == true ? 1 : 0;
        experimentData.course = experimentSceneManager.currentScene;
        experimentData.trial = trial;
        experimentData.handedness = handedness;
        //experimentData.UpdateStopTime();
//        experimentData.position_targetObject = GameObject.FindGameObjectWithTag("targetObject").transform.position;
//        experimentData.position_targetArea = GameObject.FindGameObjectWithTag("targetArea").transform.position;
//        experimentData.position_user_eye = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
//        experimentData.position_user_controller = GameObject.FindGameObjectWithTag("GameController").transform.position;
//
//        experimentData.ur5_shoulder_pan_joint = GameObject.Find("ur5_shoulder_pan_joint").GetComponent<BioIK.KinematicJoint>().GetXMotion().GetTargetValue();
//        experimentData.ur5_elbow_joint = GameObject.Find("ur5_elbow_joint").GetComponent<BioIK.KinematicJoint>().GetXMotion().GetTargetValue();
//        experimentData.ur5_wrist_1_joint = GameObject.Find("ur5_wrist_1_joint").GetComponent<BioIK.KinematicJoint>().GetXMotion().GetTargetValue();
//        experimentData.ur5_wrist_2_joint = GameObject.Find("ur5_wrist_2_joint").GetComponent<BioIK.KinematicJoint>().GetXMotion().GetTargetValue();
//        experimentData.ur5_wrist_3_joint = GameObject.Find("ur5_wrist_3_joint").GetComponent<BioIK.KinematicJoint>().GetXMotion().GetTargetValue();
//        
//        experimentData.minimum_distance_euklid_targetObject2obstacle = minimum_distance_euklid_targetObject2obstacle_list.Min();
//        experimentData.minimum_distance_euklid_targetObject2targetArea = Vector3.Distance(experimentData.position_targetObject, experimentData.position_targetArea);
//        experimentData.error = error;
        experimentData.errorCount = errorCount;
        experimentData.time = experimentData.timeStamp_stop - experimentData.timeStamp_start;

        // set individual file name for every participant and course
        //experimentFileWriter.filename = "id_" + id_participant + "_" + experimentData.course + "_method_" + experimentData.method + ".csv";
        experimentFileWriter.AppendLineToFile(experimentFileWriter.ExperimentDataFrame2String(experimentData));

		// write trajectories
		experimentFileWriter.WriteTargetTrajectory(targetPoseRecorder.GetPoseList(), 
			experimentData.id_participant,
			experimentData.method,
			experimentData.course,
			experimentData.trial);
		targetPoseRecorder.ClearData ();
		experimentFileWriter.WritePrecisionList(GameObject.FindGameObjectWithTag ("course").GetComponent<FindClosestObstacle> ().GetAccuracyList (), 
			experimentData.id_participant,
			experimentData.method,
			experimentData.course,
			experimentData.trial);
		GameObject.FindGameObjectWithTag ("course").GetComponent<FindClosestObstacle> ().ClearData ();
    }

	public void StartTrial(){
        print("StartTrial called");
        experimentData.UpdateStartTime();
		targetPoseRecorder.StartRecording ();
        GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().StartRecording();
    }

	public void StopTrial(){
        print("StopTrial called");
        experimentData.UpdateStopTime();    
		targetPoseRecorder.StopRecording ();
        GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().StopRecording();
    }

//    private float RadianToDegree(float radian) {
//        return radian * (180.0f / Mathf.PI);
//    }
}
