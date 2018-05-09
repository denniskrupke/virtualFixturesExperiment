using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour {
	public bool selected = false;
	public Color highlightColor = new Color(1,1,1);

	Color originalColor;

	// Use this for initialization
	void Start () {
		originalColor = (GetComponent<Renderer> ()).material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (selected)
			(GetComponent<Renderer> ()).material.color = highlightColor;
		else 
			(GetComponent<Renderer> ()).material.color = originalColor;
	}

	public void SetHighlighted(bool highlight){
		selected = highlight;
	}
}
