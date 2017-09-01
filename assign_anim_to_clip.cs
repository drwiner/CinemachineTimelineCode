using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class assign_anim_to_clip : MonoBehaviour {
    private TimelineAsset timeline;

    string p2 = "Assets//Scripts//CinemachineTimelineCode//xml_docs//test_world.xml";
    string p3 = "Assets//Scripts//xml_docs//block_world.xml";

    List<List<string>> readFabula()
    {
        List<List<string>> clipList = new List<List<string>>();
        List<string> clipItems;

        XmlTextReader xml_doc = new XmlTextReader(p2);
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

                    if (clipItems.Count > 0)
                    {
                        clipList.Add(clipItems);
                    }
                    break;
            }
        }
        return clipList;
    }

    // Use this for initialization
    void Awake () {
        List<List<string>> clipList = readFabula();
        foreach (List<string> clipItem in clipList)
        {
            Debug.Log("Clip:");
            foreach (string item in clipItem)
            {
                Debug.Log(item);
            }
        }

        timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        PlayableDirector director = GetComponent<PlayableDirector>();

        TrackAsset track = timeline.CreateTrack<AnimationTrack>(null, "trackname");
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");

        foreach (List<string> clipitem_list in clipList)
        {
            string name = clipitem_list[0];
            string type = clipitem_list[1];
            float start = float.Parse(clipitem_list[2]);
            float dur = float.Parse(clipitem_list[3]);
            string gameobj_name = clipitem_list[4];
            string [] start_pos_str = clipitem_list[5].Split('(', ')');
            Vector3 start_pos;
            start_pos.x = float.Parse(start_pos_str[1]);
            start_pos.y = float.Parse(start_pos_str[2]);
            start_pos.z = float.Parse(start_pos_str[3]);

            string[] finish_pos_str = clipitem_list[5].Split('(', ')');
            Vector3 finish_pos;
            finish_pos.x = float.Parse(finish_pos_str[1]);
            finish_pos.y = float.Parse(finish_pos_str[2]);
            finish_pos.z = float.Parse(finish_pos_str[3]);


            if (type.Equals("anim"))
            {
                // Treat like animation
                var clip = track.CreateDefaultClip();
                clip.start = start;
                clip.duration = dur;
                clip.displayName = name;
                
                var anim = Resources.Load(gameobj_name) as AnimationClip;
                var anim_asset = clip.asset as AnimationPlayableAsset;
                
                anim_asset.ResetOffsets();
             //   anim_asset.position = 
               // director.SetReferenceValue(anim_asset.name, anim);
            }
            //AnimationClip anim_clip = GameObject.Find(gameobj_name).GetComponent<AnimationClip>();

                //anim_clip.
                //var animAsset = anim_clip.asset as AnimationPlayableAsset;
                //animAsset.clip.exposedName = UnityEditor.GUID.Generate().ToString();
                //director.SetReferenceValue(.exposedName, new_cam);
            //}
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
        //director.SetGenericBinding(track, main_camera_object);

        // Set it to play when ready.
        director.Play(timeline);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
