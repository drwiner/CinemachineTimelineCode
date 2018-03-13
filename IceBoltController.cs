using IceBoltNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBoltController : MonoBehaviour {

    public bool ReadInDiscourseOps;
    public bool ReadInStoryPlan;
    public bool ExtractVirtualWorldInformation;
    public bool PopulateTimelines;
    public bool PlayTimelines;

    private IceBolt IB;
    private bool timelinesPopulated = false;
    private bool timelinesPlaying = false;
    public bool restartPipeline = false;

	// Use this for initialization
	void Awake () {
        IB = GetComponent<IceBolt>();
        Pipeline();


        // Read In Discourse Ops (and possibly a story plan)
        // Extract relevant information from virtual world (like pairwise distance)
        // Randomly Generate Discourse, or use existing discourse plan
    }

    void Pipeline()
    {

        if (ReadInDiscourseOps)
        {

        }
        if (ReadInStoryPlan)
        {

        }
        if (ExtractVirtualWorldInformation)
        {

        }

        //if (ActivateIceBolt)
        //{
        //    IB.enabled = true;
        //}

        //if (!ActivateIceBolt)
        //{
        //    IB.enabled = false;
        //}

        if (PopulateTimelines)
        {
            timelinesPopulated = IB.PopulateTimelines();
        }

        if (PlayTimelines)
        {
            if (timelinesPopulated) {
                IB.PlayTimelines();
                timelinesPlaying = true;
            }
        }

    }
	// Update is called once per frame
	void Update () {
        if (restartPipeline)
        {
            Pipeline();
            restartPipeline = false;
        }

        if (PlayTimelines && !timelinesPlaying)
        {
            if (timelinesPopulated)
            {
                IB.PlayTimelines();
                timelinesPlaying = true;
            }
        }
    }
}
