using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO; 

using UnityEngine;

public class RenderTrajectory : MonoBehaviour {
	public string file = "";
	public LineRenderer lineRenderer;
	private string line = "";
	private List<Vector3> points;
	private bool done = false;

	// Use this for initialization
	void Start () {
		points = new List<Vector3>();
		Load(file);
	}
	
	// Update is called once per frame
	void Update () {
		if(done) {
			lineRenderer.positionCount = points.Count;
			lineRenderer.SetPositions(points.ToArray());
			done = false;
		}
	}

	
	private void Load(string fileName)
	{
	 //     // Handle any problems that might arise when reading the text
	 //     //try
	 //     //{
			         
		StreamReader theReader = new StreamReader(fileName, Encoding.Default);
	         
			
		line = theReader.ReadLine();
		// bool firstLineDone = false;
		while(line != null){
			string[] entries = line.Split(',');               
            // do stuff
        	// if(firstLineDone){
            	Vector3 point = new Vector3(float.Parse(entries[1]),float.Parse(entries[2]),float.Parse(entries[3]));             	
             	Debug.Log(""+point);
             	points.Add(point);
            // }
			line = theReader.ReadLine();
			// firstLineDone = true;				
		}
		// if(line != null){
  //           do
  //           {
  //           	string[] entries = line.Split(',');
  //               if (entries.Length > 0){
  //                	// do stuff
  //               	Vector3 point = new Vector3(float.Parse(entries[1]),float.Parse(entries[2]),float.Parse(entries[3]))
  //                	//Debug.Log(point);
		// 			line = theReader.ReadLine();
  //                }		                     
  //           }		             
  //           while (line != null);
  //       }
 //             // Done reading, close the reader and return true to broadcast success    
        theReader.Close();		
		done = true;
	}	   
}