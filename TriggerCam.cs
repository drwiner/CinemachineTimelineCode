using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class TriggerCam : MonoBehaviour {
    public UnityEngine.Playables.PlayableGraph this_graph;
	// Use this for initialization
	void Start () {
        this_graph = this.GetComponent<UnityEngine.Playables.PlayableDirector>().playableGraph;
       // this.GetComponent<UnityEngine.Playables.PlayableDirector>().SetReferenceValue();
        //this.GetComponent<UnityEngine.Playables.PlayableDirector>().SetGenericBinding();
      //  this.GetComponent<UnityEngine.Playables.PlayableAsset>().CreatePlayable(this_graph, this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {


    }

}
