﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gripController : MonoBehaviour {

    public SteamVR_TrackedController _controller;
    public GameObject _UR5_target;
	public GameObject _targetObject;
    public GameObject _finger1, _finger2, _finger3;

    private Vector3 targetObjectCenter;
    private Vector3 UR5_targetCenter;
    private float distance;

    // Use this for initialization
    void Start () {
        _controller = GetComponent<SteamVR_TrackedController>();
        _UR5_target = GameObject.FindWithTag ("UR5-Target");
		_targetObject = GameObject.FindWithTag ("targetObject");
        _finger1 = GameObject.Find("s_model_finger_1_joint_1");
        _finger2 = GameObject.Find("s_model_finger_2_joint_1");
        _finger3 = GameObject.Find("s_model_finger_middle_joint_1");
    }
	
	// Update is called once per frame
	void Update () {

        targetObjectCenter = _targetObject.GetComponent<Collider>().transform.position;
        UR5_targetCenter = _UR5_target.GetComponent<Collider>().transform.position;
        distance = Vector3.Distance(targetObjectCenter, UR5_targetCenter);

        //print(distance);

        // targetOject can only be grabbed on close distance
        if (distance < 0.07f)
        {
            _controller.TriggerClicked += HandleTriggerClicked;
            _controller.TriggerUnclicked += HandleTriggerUnclicked;
        }
        else
        {
            _controller.TriggerClicked += HandleTriggerUnclicked;
            _controller.TriggerUnclicked += HandleTriggerUnclicked;
        }
	}

    // attach targetObject for movement
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        _targetObject.transform.SetParent(parent: _UR5_target.transform);
        CloseGrippers();
    }

    // detach targetObject
    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        _UR5_target.transform.DetachChildren();
        OpenGrippers();
    }


    // visual effect gripper closing
    private void CloseGrippers()
    {
        _finger1.GetComponent<BioIK.KinematicJoint>().SetTargetValues(1, 0, 0);
        _finger2.GetComponent<BioIK.KinematicJoint>().SetTargetValues(1, 0, 0);
        _finger3.GetComponent<BioIK.KinematicJoint>().SetTargetValues(1, 0, 0);
    }

    // visual effect gripper opening
    private void OpenGrippers()
    {
        _finger1.GetComponent<BioIK.KinematicJoint>().SetTargetValues(0, 0, 0);
        _finger2.GetComponent<BioIK.KinematicJoint>().SetTargetValues(0, 0, 0);
        _finger3.GetComponent<BioIK.KinematicJoint>().SetTargetValues(0, 0, 0);
    }
}
