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
	public int objectCollisionCount; //number of collisions
	public int gripperCollisionCount;
	public long duration; //time from pickup to drop at targetarea			

	public void UpdateStartTime(){
		timeStamp_start = ExperimentDataLogger.CalculateCurrentTimeStamp();
	}

	public void UpdateStopTime(){
		timeStamp_stop = ExperimentDataLogger.CalculateCurrentTimeStamp();	
	}
}


// ExperimentDataLogger Behaviour can be attached to GameObject to execute data collection
public class ExperimentDataLogger : MonoBehaviour {
    [Header("General public varibales")]
    public ExperimentFileWriter experimentFileWriter = null;
    public ExperimentSceneManager experimentSceneManager = null;
    public Transform ur5Target = null;
    public Transform targetObject = null;    

    /// public long timeStamp_start; //when the target appears in system time expressed in milliseconds
    [Header("Trial specific variables")]
    public int id_participant; // insterted manually
    public int trial = 0; // number of trial executed (1,2,3)
    public int handedness; // 0 = left handedness, 1 = right handedness - inserted manually    
    public int objectCollisionCount; // public variable gets updated from visualFeedback_obstacle
    public int gripperCollisionCount;
    public List<float> minimum_distance_euklid_targetObject2obstacle_list; // public variable gets updated from visualFeedback_obstacle    

    bool isColliding; // public variable gets updated from visualFeedback_obstacle
    bool isGrabbed; // if the gripper grasped an object

    public ExperimentDataFrame experimentData;
    
	private RecordPose targetPoseRecorder;
	private RecordPose tcpPoseRecorder;
	private RecordPose headPoseRecorder;
	private RecordCollisions collisionRecorder;
	// todo: record time of collisions and position



    // Use this for initialization
    void Start () {
		experimentData = new ExperimentDataFrame();
		targetPoseRecorder = GameObject.FindGameObjectWithTag ("targetObject").GetComponent<RecordPose> ();
		tcpPoseRecorder = GameObject.Find("TCP").GetComponent<RecordPose>() ;
		headPoseRecorder = GameObject.Find("Camera (eye)").GetComponent<RecordPose>() ;
		collisionRecorder = GetComponent<RecordCollisions>();

        isColliding = false;
        isGrabbed = false;
    }
		
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
        experimentData.objectCollisionCount = objectCollisionCount;
        experimentData.gripperCollisionCount = gripperCollisionCount;
        experimentData.duration = experimentData.timeStamp_stop - experimentData.timeStamp_start;

        // set individual file name for every participant and course
        //experimentFileWriter.filename = "id_" + id_participant + "_" + experimentData.course + "_method_" + experimentData.method + ".csv";
        experimentFileWriter.AppendLineToFile(experimentFileWriter.ExperimentDataFrame2String(experimentData));

        // write trajectories
        WriteTrajectories();     
    }

    private void WriteTrajectories()
    {
        Debug.Log("writing object collisions");
        experimentFileWriter.WriteCollisionList("graspedObject",
            collisionRecorder.GetCollisionList_graspedObject(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);

        experimentFileWriter.WriteCollisionList("gripper",
            collisionRecorder.GetCollisionList_gripper(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);
           

        experimentFileWriter.WriteTargetTrajectory("graspedObject",
            targetPoseRecorder.GetPoseList(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);        

        experimentFileWriter.WriteTargetTrajectory("tcp",
            tcpPoseRecorder.GetPoseList(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);        

        experimentFileWriter.WriteTargetTrajectory("head",
            headPoseRecorder.GetPoseList(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);        


        experimentFileWriter.WritePrecisionList(GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().GetAccuracyList(),
            experimentData.id_participant,
            experimentData.method,
            experimentData.course,
            experimentData.trial);        
    }

    public void StartTrial(){
        //print("StartTrial called");
        experimentData.UpdateStartTime();

		if(targetPoseRecorder!=null) targetPoseRecorder.StartRecording ();
		if(tcpPoseRecorder!=null) tcpPoseRecorder.StartRecording ();
        if(headPoseRecorder != null) headPoseRecorder.StartRecording();
        GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().StartRecording();
    }

	public void StopTrial(){
        //print("StopTrial called");
        experimentData.UpdateStopTime();    

		if(targetPoseRecorder!=null) targetPoseRecorder.StopRecording ();
		if(tcpPoseRecorder!=null) tcpPoseRecorder.StopRecording ();
        if(headPoseRecorder != null) headPoseRecorder.StopRecording();
        GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().StopRecording();
    }

    public void SetColliding(bool colliding)
    {
        isColliding = colliding;
    }

    public bool IsColliding()
    {
        return isColliding;
    }

    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    public void ResetData(){
        Debug.Log("resetting data");
        experimentData.timeStamp_stop = 0;
        objectCollisionCount = 0;
        isColliding = false;
        isGrabbed = false;
        objectCollisionCount = 0;
        gripperCollisionCount = 0;

        experimentData.objectCollisionCount = 0;
        experimentData.gripperCollisionCount = 0;

        collisionRecorder.ClearData();
        targetPoseRecorder.ClearData();
        tcpPoseRecorder.ClearData();
        headPoseRecorder.ClearData();
        GameObject.FindGameObjectWithTag("course").GetComponent<FindClosestObstacle>().ClearData();
    }

    public static long CalculateCurrentTimeStamp(){
		return System.DateTime.Now.Millisecond + System.DateTime.Now.Second*1000 + System.DateTime.Now.Minute*60*1000 + System.DateTime.Now.Hour*60*60*1000;
	}

//    private float RadianToDegree(float radian) {
//        return radian * (180.0f / Mathf.PI);
//    }
}
