using Cinemachine.Timeline;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;
using System.Xml;
using System.Xml.Linq;

public class assign_cam_to_clip : MonoBehaviour {
    private TimelineAsset timeline;

    private GameObject main_camera_object;

    public string cam_script_name;
   // string p2 = "Assets//Scripts//CinemachineTimelineCode//xml_docs//test_world.xml";
  //  string p3 = "Assets//Scripts//xml_docs//block_world.xml";
    
    List<List<string>> readDiscourse()
    {
        List<List<string>> clipList = new List<List<string>>();
        List<string> clipItems;

        XmlTextReader xml_doc = new XmlTextReader(cam_script_name);
        // cycle through each child noed 
        while (xml_doc.Read())
        {
            switch (xml_doc.NodeType)
            {  
                case XmlNodeType.Element: // if the node is an element
                    clipItems = new List<string>();
                    while (xml_doc.MoveToNextAttribute())
                        clipItems.Add(xml_doc.Value);
                      //  Debug.Log("- " + xml_doc.Name + "='" + xml_doc.Value + "'");

                    if(clipItems.Count() > 0)
                    {
                        clipList.Add(clipItems);
                    }
                    break;
            }
        }
        return clipList;
    }


    // Use this for initialization
    void Awake() {
        List<List<string>> clipList = readDiscourse();
        foreach(List<string> clipItem in clipList)
        {
            Debug.Log("Clip:");
            foreach(string item in clipItem)
            {
                Debug.Log(item);
            }
        }
        main_camera_object = GameObject.Find("Main Camera");

        timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        PlayableDirector director = GetComponent<PlayableDirector>();
        
        TrackAsset track = timeline.CreateTrack<CinemachineTrack>(null, "trackname");
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");

        foreach (List<string> clipitem_list in clipList)
        {
            string name = clipitem_list[0];
            string type = clipitem_list[1];
            float start = float.Parse(clipitem_list[2]);
            float dur = float.Parse(clipitem_list[3]);
            string gameobj_name = clipitem_list[4];

            if (type.Equals("cam")){
                var clip = track.CreateDefaultClip();
                clip.start = start;
                clip.duration = dur;
                clip.displayName = name;

                CinemachineVirtualCamera new_cam = GameObject.Find(gameobj_name).GetComponent<CinemachineVirtualCamera>();
                var cinemachineShot = clip.asset as CinemachineShot;
                cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, new_cam);
            }
            else // assume control track
            {
                var clip = ctrack.CreateDefaultClip();
                clip.start = start;
                clip.duration = dur;
                clip.displayName = name;
                GameObject new_cam = GameObject.Find(gameobj_name);
                var controlshot = clip.asset as ControlPlayableAsset;
                controlshot.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
                director.SetReferenceValue(controlshot.sourceGameObject.exposedName, new_cam);
            }
        }

        // Set cinemachine brain as track's game object
        director.SetGenericBinding(track, main_camera_object);

        // Set it to play when ready.
        director.Play(timeline);
    }

    // Update is called once per frame
    void Update() {

    }
}