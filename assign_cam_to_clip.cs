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
    private Dictionary<string, CinemachineVirtualCamera> cam_dict;
    private TimelineAsset timeline;

    public List<CinemachineVirtualCamera> prim_cams;
    public List<GameObject> special_shots;
    public string story_xml_path;

    private GameObject main_camera_object;
    string p2 = "D://Unity projects//ShootingWorld//ShootingWorld//storyPlans//test_world.xml";

    void createTrack(string track_name, bool control_track)
    {

        if (control_track)
        {
            TrackAsset track = timeline.CreateTrack<CinemachineTrack>(null, "trackname");
        }

    }

    void addClip(PlayableDirector director, string expName, GameObject gameobject)
    {
        director.SetReferenceValue(expName, gameobject);
    }

    List<KeyValuePair<string, GameObject>> extractClips()
    {
        XmlTextReader xml_doc = new XmlTextReader(story_xml_path);
        List<KeyValuePair<string, GameObject>> clips = new List<KeyValuePair<string, GameObject>>();
        // cycle through each child noed 
        while(xml_doc.Read()){
            switch (xml_doc.NodeType)
            {
                case XmlNodeType.Element: // if the node is an element
                    Console.Write("<" + xml_doc.Name);
                    Console.WriteLine(">");
                    break;
                case XmlNodeType.Text: //Display the text in each element.
                    Console.WriteLine(xml_doc.Value);
                    break;
                case XmlNodeType.EndElement: //Display the end of the element.
                    Console.Write("</" + xml_doc.Name);
                    Console.WriteLine(">");
                    break;
            }
            Console.WriteLine(xml_doc.Name);
        }
   


        return clips;
    }

    
    List<List<string>> extract2()
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

                    if(clipItems.Count() > 0)
                    {
                        clipList.Add(clipItems);
                    }
                    break;
            }
        }
    return clipList;
    }

    void extract3()
    {
        var xml_doc = XDocument.Load(p2);
        // cycle through each child noed 
        var query = from c in xml_doc.Root.Descendants("clips")
                    select c.Element("clip").Value;

        foreach (string name in query)
        {
            Debug.Log("THe clip: " + name);
        }
    }

    // Use this for initialization
    void Awake() {
        List<List<string>> clipList = extract2();
        foreach(List<string> clipItem in clipList)
        {
            Debug.Log("\nClip:");
            foreach(string item in clipItem)
            {
                Debug.Log(item);
            }
        }
        main_camera_object = GameObject.Find("Main Camera");
        cam_dict = new Dictionary<string, CinemachineVirtualCamera>();

        // Load dictionary from public list
        foreach(CinemachineVirtualCamera Cam in prim_cams)
        {
            cam_dict.Add(Cam.Name, Cam);
        }

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
                CinemachineVirtualCamera new_cam = GameObject.Find(gameobj_name).GetComponent<CinemachineVirtualCamera>();
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