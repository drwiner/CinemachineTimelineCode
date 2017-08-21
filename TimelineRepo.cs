using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
public class TimelineRepo : MonoBehaviour {
    // A container class to hold references to timelines
    // Use this for initialization
    public List<TimelineAsset> timeline_assets = new List<TimelineAsset>();
    public List<PlayableDirector> playable_directors = new List<PlayableDirector>();


    
}
