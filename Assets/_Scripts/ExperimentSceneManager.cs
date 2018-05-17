using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentSceneManager : MonoBehaviour {

    [Header("Course Rotation Settings")]
    public bool VF_first = false;
    public string currentScene;
	public short repetitions = 5;

    private string[] sceneNames;
    private int modulator;
    private int sceneIndex = 0;


    // gameObject targetObject to reset its position
    private GameObject targetObject;

	// Use this for initialization
	void Start () {

        if (VF_first)
        {
            // VF first
            sceneNames = new string[] { "PauseVF", "Course1_VF", "Course2_VF", "Course3_VF", "Pause", "Course1", "Course2", "Course3"};
        } else
        {   // VF second
            sceneNames = new string[] {"Pause", "Course1","Course2", "Course3","PauseVF", "Course1_VF", "Course2_VF", "Course3_VF"};
        }
			
		SceneManager.LoadScene (sceneNames[sceneIndex], LoadSceneMode.Additive);

        // provide inital sceneName for ExperimentData in DataLogger
        currentScene = sceneNames[sceneIndex];
        modulator = sceneNames.Length;
    }
	
	// Update is called once per frame
	void Update () {

        // if trial 0,1,2 are completed, load next scene in rotation (or manual scene change with spacebar)
		if (GetComponent<ExperimentDataLogger>().trial > repetitions-1 || Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.UnloadSceneAsync (sceneNames [(int)nfmod(sceneIndex, modulator)]);            

			if (++sceneIndex >= sceneNames.Length)
				UnityEditor.EditorApplication.isPlaying = false;
			else {                
				SceneManager.LoadSceneAsync (sceneNames [(int)nfmod (sceneIndex, modulator)], LoadSceneMode.Additive);
                Debug.Log("fading after new scene");
                GameObject.Find("ExperimentController").GetComponent<Fader>().FadeInstantBack();
                // reset course specific variables (new course name, trial and errorCount reset
                currentScene = sceneNames [(int)nfmod (sceneIndex, modulator)];
				GetComponent<ExperimentDataLogger> ().trial = 0;
                if(sceneIndex == 4) UnityEditor.EditorApplication.isPaused = true;
            }
        }
	}

	// modulo function
	private float nfmod(float a,float b)
	{
		return a - b * (float)Math.Floor(a / b);
	}
  
}
