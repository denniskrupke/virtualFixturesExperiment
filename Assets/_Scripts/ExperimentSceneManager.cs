﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentSceneManager : MonoBehaviour {

    [Header("Course Rotation Settings")]
    public bool VF_first_rotation = false;
    public string currentScene;

    private string[] sceneNames;
    private int modulator;
    private int sceneIndex = 0;


    // gameObject targetObject to reset its position
    private GameObject targetObject;

	// Use this for initialization
	void Start () {

        if (VF_first_rotation)
        {
            // VF first
            sceneNames = new string[] { "Pause", "Course1_VF", "Course2_VF", "Course3_VF", "Pause", "Course1", "Course2", "Course3"};
        } else
        {   // VF second
            sceneNames = new string[] {"Pause", "Course1","Course2", "Course3","Pause", "Course1_VF", "Course2_VF", "Course3_VF"};
        }

		SceneManager.LoadScene (sceneNames[sceneIndex], LoadSceneMode.Additive);
        // provide inital sceneName for ExperimentData in DataLogger
        currentScene = sceneNames[sceneIndex];
        modulator = sceneNames.Length;
    }
	
	// Update is called once per frame
	void Update () {

        // if trial 0,1,2 are completed, load next scene in rotation (or manual scene change with spacebar)
        if (GetComponent<ExperimentDataLogger>().trial > 2 || Input.GetKeyDown(KeyCode.Space)) {

            SceneManager.UnloadSceneAsync (sceneNames [(int)nfmod(sceneIndex, modulator)]);
            sceneIndex++;
            SceneManager.LoadSceneAsync (sceneNames [(int)nfmod(sceneIndex, modulator)], LoadSceneMode.Additive);
            
            // reset course specific variables (new course name, trial and errorCount reset
            currentScene = sceneNames[(int)nfmod(sceneIndex, modulator)];
            GetComponent<ExperimentDataLogger>().trial = 0;
            GetComponent<ExperimentDataLogger>().errorCount = 0;
        }
	}

	// modulo function
	private float nfmod(float a,float b)
	{
		return a - b * (float)Math.Floor(a / b);
	}
}