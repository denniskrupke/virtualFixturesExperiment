using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ExperimentFileWriter : MonoBehaviour
{
    public string filename = "default.csv";
    public string win_path = "D:\\User\\krupke";

	// Use this for initialization
    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        /*
        using (StreamWriter writer = File.CreateText(path))
        {
            writer.WriteLine("100");
            writer.Flush();
            writer.Dispose();
        }
        */

        print(Application.persistentDataPath);
    }


	
	public void WriteTargetTrajectory(string objectName, List<StampedPose> poses, int participant, int method, string course, int trial){
        if (poses.Count == 0) {
            Debug.Log("no poses in the list");
            return;
        }

		//string path = Path.Combine(Application.persistentDataPath, "targetObject_" + participant + "_" + method + "_" + course + "_" + trial + ".csv");
        string path = Path.Combine(win_path, "targetObject_" + objectName + "_" + participant + "_" + method + "_" + course + "_" + trial + ".csv");
        using (StreamWriter writer = File.CreateText(path))
		{         
            string line = "";
            line += "timestamp";
            line += ",";
            line += "x_pos";
            line += ",";
            line += "y_pos";
            line += ",";
            line += "z_pos";

            line += ",";
            line += "quat_x";
            line += ",";
            line += "quat_y";
            line += ",";
            line += "quat_z";
            line += ",";
            line += "quat_w";

            line += ",";
            line += "roll";
            line += ",";
            line += "yaw";
            line += ",";
            line += "pitch";

            line += ",";
            line += "forward_x";
            line += ",";
            line += "forward_y";
            line += ",";
            line += "forward_z";
            writer.WriteLine (line);   
			foreach (StampedPose pose in poses) {
				line = "";
				line += pose.timeInMillis;
				line += ",";
				line += pose.position.x.ToString("R");
				line += ",";
				line += pose.position.y.ToString("R");
				line += ",";
				line += pose.position.z.ToString("R");

                line += ",";
                line += pose.rotation.x.ToString("R");
                line += ",";
                line += pose.rotation.y.ToString("R");
                line += ",";
                line += pose.rotation.z.ToString("R");
                line += ",";
                line += pose.rotation.w.ToString("R");

                line += ",";
                line += pose.euler.x.ToString("R");
                line += ",";
                line += pose.euler.y.ToString("R");
                line += ",";
                line += pose.euler.z.ToString("R");

                line += ",";
                line += pose.forward.x.ToString("R");
                line += ",";
                line += pose.forward.y.ToString("R");
                line += ",";
                line += pose.forward.z.ToString("R");                
				writer.WriteLine (line);
			}
			writer.Flush();
			writer.Dispose();
		}
	}

	// TODO probably adding a timestamp
	public void WritePrecisionList(List<KeyValuePair<float,bool> > precisionList, int participant, int method, string course, int trial){
        if (precisionList.Count == 0) {
            Debug.Log("no precision recordings in the list");
            return;
        }

		//string path = Path.Combine(Application.persistentDataPath, "precision_" + participant + "_" + method + "_" + course + "_" + trial + ".csv");
        string path = Path.Combine(win_path, "precision_" + participant + "_" + method + "_" + course + "_" + trial + ".csv");
        using (StreamWriter writer = File.CreateText(path))
		{        
            string line = "";
            line += "shortestDistance";
            line += ",";
            line += "didCollide";
            writer.WriteLine (line);    
            foreach (KeyValuePair<float,bool> el in precisionList) {
				line = "";
				line += el.Key.ToString("R");
				line += ",";
				line += el.Value;
				writer.WriteLine (line);
			}
			writer.Flush();
			writer.Dispose();
		}
	}


    // TODO write the duration of the collision as well
    public void WriteCollisionList(string type, List<StampedCollision> collisionList, int participant, int method, string course, int trial){
        if (collisionList.Count == 0) {
            Debug.Log("no collision recordings in the list");
            return;
        }
        
        string path = Path.Combine(win_path, "collision_" + type + "_" + participant + "_" + method + "_" + course + "_" + trial + ".csv");
        using (StreamWriter writer = File.CreateText(path))
        {            
            string line = "";
            line += "TimeOfCollision";
            line += ",";
            line += "x";
            line += ",";
            line += "y";
            line += ",";
            line += "z";
            line += ",";                
            line += "other";
            writer.WriteLine (line);
            foreach (StampedCollision col in collisionList) {
                line = "";
                line += col.timeInMillis;
                line += ",";
                line += col.position.x.ToString("R");
                line += ",";
                line += col.position.y.ToString("R");
                line += ",";
                line += col.position.z.ToString("R");
                line += ",";                
                line += col.other;
                writer.WriteLine (line);
            }
            writer.Flush();
            writer.Dispose();
        }   
    }


    public void AppendLineToFile(string line)
    {        
        string path = Path.Combine(win_path, this.filename);

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
        data += frame.objectCollisionCount;
        data += ",";
        data += frame.gripperCollisionCount;
        data += ",";
        data += frame.duration;

        return data;
    }
}
