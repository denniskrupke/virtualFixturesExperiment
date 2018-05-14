using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour {

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;            
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Fade()
    {
        SteamVR_Fade.Start(new Color(.4f,.4f,.4f,1), 1);
        this.Invoke("FadeBack", 1);
    }

    private void FadeBack()
    {
        SteamVR_Fade.Start(Color.clear, 1);
    }

    public void FadeInstantBack()
    {
        SteamVR_Fade.Start(new Color(.4f, .4f, .4f, 1), 0.1f);
        this.Invoke("FadeBack", 1);
    }
}
