using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class visualFeedback_fixture : MonoBehaviour {

	private GameObject targetObject;
	private Renderer rend;

	Vector3 virtualFixtureSurfacePoint;
	Vector3 targetObjectCenter;
	float surfaceDistance;

	private Color redColor = Color.red;
	private Color greenColor = Color.green;


	void Start()
	{	
		// lerping transparent colors causes glitches
		greenColor.a = 0.5f;
		redColor.a = 1.0f;

		rend = GetComponent<Renderer> ();
		targetObject = GameObject.FindGameObjectWithTag ("targetObject");
	}    
		
	void Update()
	{
		// get targetObjectCenter position
		targetObjectCenter = targetObject.GetComponent<Collider>().transform.position;

		// get closest virtualFixtureSurfacePoint to targetObjectCenter position
		virtualFixtureSurfacePoint = GetComponent<Collider>().ClosestPointOnBounds(targetObjectCenter);

		// get surfaceDistance from virtualFixtureSurfacePoint to targetObjectCenter
		surfaceDistance = Vector3.Distance(virtualFixtureSurfacePoint, targetObjectCenter); 

		// visualize the different distances (debug)
		Debug.DrawLine(transform.position, targetObjectCenter, Color.yellow); //yellow center to center
		Debug.DrawLine(virtualFixtureSurfacePoint, targetObjectCenter, Color.magenta); // magenta surface to surface

		// change _environmentVirtualFixture Color depending on the (scaled) distance between virtualFixtureSurfacePoint and targetObjectCenter
		rend.material.color = Color.LerpUnclamped (redColor, greenColor, capDistance(surfaceDistance * 6));
	}

    private float capDistance(float x) {
        if (x > 1) {
            return 1;
        } else
        {
            return x;
        }
    }
		
}