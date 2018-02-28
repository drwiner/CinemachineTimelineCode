using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.Timeline;

public class SetFabulaTimeline : MonoBehaviour {

    public GameObject fab_timeline;
    public float fab_time;

    private PlayableDirector pd;

    void Awake()
    {
        fab_timeline = GameObject.FindGameObjectWithTag("FabulaTimeline");
    }

    void Start()
    {
        pd = fab_timeline.GetComponent<PlayableDirector>();
    }

    public void Rewind(float ft)
    {
        while (ft < pd.time)
        {
            pd.time -= .1f;
            pd.Evaluate();
        }
    }

    public void FastForward(float ft)
    {
        while (ft > pd.time)
        {
            pd.time += .1f;
            pd.Evaluate();
        }
    }

    public void setFabTime(float new_time)
    {
        //pd.playableGraph.Stop();
        pd.Pause();
        if (new_time > pd.time)
        {
            FastForward(new_time);
        }
        else
        {
            Rewind(new_time);
        }

        Debug.Log("setFabTime: " + pd.time);
        fab_time = (float)pd.time;
        if (pd.state != PlayState.Playing)
        {
            pd.Evaluate();
            pd.Play();
        }
        
    }



}
