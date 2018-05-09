using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ExperimentFileWriter : MonoBehaviour
{
    public string filename = "default.csv";
    public string win_path = "D:/User/3slange/UnityProject/LabratorySetup";

	// Use this for initialization
    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        using (StreamWriter writer = File.CreateText(path))
        {
            writer.WriteLine("100");
            writer.Flush();
            writer.Dispose();
        }

        print(Application.persistentDataPath);
    }

    //Update is called once per frame
    void Update()
    {

    }


    public void WriteTestFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "TestFileMoop.txt");
        using (StreamWriter writer = File.CreateText(path))
        {
            writer.WriteLine("0");
            writer.Flush();
            writer.Dispose();
        }
    }

	// TODO add timestamps
	public void WriteTargetTrajectory(List<StampedPose> poses, int participant, int method, int course, int trial){
		string path = Path.Combine(Application.persistentDataPath, "targetObject_" + participant + "_" + method + "_" + course + "_" + trial);
		using (StreamWriter writer = File.CreateText(path))
		{
			foreach (StampedPose pose in poses) {
				string line = "";
				line += pose.timeInMillis;
				line += ",";
				line += pose.position.x;
				line += ",";
				line += pose.position.y;
				line += ",";
				line += pose.position.z;
				writer.WriteLine (line);
			}
			writer.Flush();
			writer.Dispose();
		}
	}

	// TODO probably adding a timestamp
	public void WritePrecisionList(List<KeyValuePair<float,bool> > precisionList, int participant, int method, int course, int trial){
		string path = Path.Combine(Application.persistentDataPath, "precision_" + participant + "_" + method + "_" + course + "_" + trial);
		using (StreamWriter writer = File.CreateText(path))
		{
			foreach (KeyValuePair<float,bool> el in precisionList) {
				string line = "";
				line += el.Key;
				line += ",";
				line += el.Value;
				writer.WriteLine (line);
			}
			writer.Flush();
			writer.Dispose();
		}
	}


    public void AppendLineToFile(string line)
    {
        string path = Path.Combine(Application.persistentDataPath, this.filename);
	
        using (StreamWriter writer = File.AppendText(path))
        {            
            writer.WriteLine(line);
            writer.Flush();
            writer.Dispose();
        }        
    }

    public string ExperimentDataFrame2String(ExperimentDataFrame frame)
    {
        string data = "";
        data += frame.id_participant;
        data += ",";
        data += frame.method;
        data += ",";
        data += frame.course;
        data += ",";
		data += frame.trial;
        data += ",";
        data += frame.handedness;
        data += ",";
        data += frame.timeStamp_start;
        data += ",";
        data += frame.timeStamp_stop;
        data += ",";
//        data += "[";
//        data += frame.position_targetObject.x;
//        data += ",";
//        data += frame.position_targetObject.y;
//        data += ",";
//        data += frame.position_targetObject.z;
//        data += "]";
//        data += ",";
//        data += "[";
//        data += frame.position_targetArea.x;
//        data += ",";
//        data += frame.position_targetArea.y;
//        data += ",";
//        data += frame.position_targetArea.z;
//        data += "]";
//        data += ",";
//        data += "[";
//        data += frame.position_user_eye.x;
//        data += ",";
//        data += frame.position_user_eye.y;
//        data += ",";
//        data += frame.position_user_eye.z;
//        data += "]";
//        data += ",";
//        data += "[";
//        data += frame.position_user_controller.x;
//        data += ",";
//        data += frame.position_user_controller.y;
//        data += ",";
//        data += frame.position_user_controller.z;
//        data += "]";
//        data += ",";
//        data += frame.ur5_shoulder_pan_joint;
//        data += ",";
//        data += frame.ur5_shoulder_lift_joint;
//        data += ",";
//        data += frame.ur5_elbow_joint;
//        data += ",";
//        data += frame.ur5_wrist_1_joint;
//        data += ",";
//		data += frame.ur5_wrist_2_joint;
//        data += ",";
//        data += frame.ur5_wrist_3_joint;
//        data += ",";
//        data += frame.minimum_distance_euklid_targetObject2obstacle;
//        data += ",";
//        data += frame.minimum_distance_euklid_targetObject2targetArea;
//        data += ",";
//        data += frame.error;
//        data += ",";
        data += frame.errorCount;
        data += ",";
        data += frame.time;

        return data;
    }
}
