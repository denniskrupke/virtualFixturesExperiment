using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class hapticFeedback_targetObject : MonoBehaviour {

    public Vector3 startingPosition = new Vector3();

    // array for all active _environmentVirtualFixture
    private GameObject[] virtualFixtureList;
    // array for all active _environmentObstacles

    // haptic pulse variables
    private ushort maxVibration = 3500;
	private ushort minVibration = 500;

	void Start () {
        this.transform.position = startingPosition;
	}


	void Update () {
		float minimalDistance = GameObject.FindGameObjectWithTag ("course").GetComponent<FindClosestObstacle> ().GetClosestDistance ();

        // change controller vibration strength depending on the (scaled) minimal surfaceDistance to any _environmentVirtualFixture
        // the higher the scaling factor the closer you have to be to the fixtures for a vibration effect
        
        ushort vibrationLerp = (ushort)Mathf.LerpUnclamped (maxVibration, minVibration, minimalDistance);
        Debug.Log(vibrationLerp);
        if (minimalDistance < 0.01f)
        {
            SteamVR_Controller.Input(1).TriggerHapticPulse(vibrationLerp); //LeftController index: 1
            SteamVR_Controller.Input(2).TriggerHapticPulse(vibrationLerp); //RightController index: 2
            SteamVR_Controller.Input(3).TriggerHapticPulse(vibrationLerp); //LeftController index: 3
            SteamVR_Controller.Input(4).TriggerHapticPulse(vibrationLerp); //RightController index: 4
        }


//		// due to loading and unloading scenes, fixtures need to found every frame
//		virtualFixtureList = GameObject.FindGameObjectsWithTag ("fixture");
//
//        if (virtualFixtureList != null && virtualFixtureList.Length != 0) {
//			HapticFeedback ();
//		}
	}


//	private void HapticFeedback () {	
//		// list of Vector3 surfacePoints from all _environmentVirtualFixtures
//		List<Vector3> surfacePointsList = new List<Vector3>();
//		// list of all float surfaceDistances from fixture-to-targetObject
//		List<float> surfaceDistanceList = new List<float> ();
//
//		// get targetObjectCenter position
//		Vector3 targetObjectCenter = GetComponent<Collider>().transform.position;
//
//
//		foreach (GameObject fixture in virtualFixtureList) {
//			// get all closestSurfacePoints from _environmentVirtualFixtures to targetObjectCenter
//			Vector3 temp_surfacePoint = fixture.GetComponent<Collider>().ClosestPointOnBounds(targetObjectCenter);
//			// get all surfaceDistances from _environmentVirtualFixtures to targetObjectCenter
//			float temp_surfaceDistance = Vector3.Distance(targetObjectCenter, temp_surfacePoint);
//
//			surfacePointsList.Add(temp_surfacePoint);
//			surfaceDistanceList.Add (temp_surfaceDistance);
//		}
//
//		// change controller vibration strength depending on the (scaled) minimal surfaceDistance to any _environmentVirtualFixture
//        // the higher the scaling factor the closer you have to be to the fixtures for a vibration effect
//		ushort vibrationLerp = (ushort)Mathf.LerpUnclamped (maxVibration, minVibration, surfaceDistanceList.Min() * 17);
//		SteamVR_Controller.Input (1).TriggerHapticPulse (vibrationLerp); //LeftController index: 1
//		SteamVR_Controller.Input (2).TriggerHapticPulse (vibrationLerp); //RightController index: 2
//		SteamVR_Controller.Input (3).TriggerHapticPulse (vibrationLerp); //LeftController index: 3
//		SteamVR_Controller.Input (4).TriggerHapticPulse (vibrationLerp); //RightController index: 4
//	}

    public void resetPosition(){
        this.transform.position = startingPosition;
    }
}