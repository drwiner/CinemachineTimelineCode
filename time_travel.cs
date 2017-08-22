using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class time_travel : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        // Grab fabula timeline
        GameObject fabula_timeline_obj = GameObject.Find("FabulaTimeline");
        TimelineAsset fabula_timeline = fabula_timeline_obj.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;

        // 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
