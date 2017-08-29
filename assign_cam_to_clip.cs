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

    
    void extract2()
    {
        XmlTextReader xml_doc = new XmlTextReader(p2);
        // cycle through each child noed 
        while (xml_doc.Read())
        {
            switch (xml_doc.NodeType)
            {
                case XmlNodeType.Element: // if the node is an element
                    Debug.Log("<" + xml_doc.Name + "_");
                    Debug.Log("VALUE: " + xml_doc.Value);
                    while (xml_doc.MoveToNextAttribute())
                        Debug.Log("- " + xml_doc.Name + "='" + xml_doc.Value + "'");

                    Debug.Log(">");
                    break;
                case XmlNodeType.Text: //Display the text in each element.
                    Debug.Log("Text: " + xml_doc.Value);
                    break;
                //case XmlNodeType.EndElement: //Display the end of the element.
                //    Debug.Log("</" + xml_doc.Name + ">");
                //    break;
            }
            Debug.Log("DOC_NAME: " +xml_doc.Name);
        }
    }

    void extract3()
    {
        var xml_doc = XDocument.Load(p2);
        // cycle through each child noed 
        var query = from c in xml_doc.Root.Descendants("clips")
                    select c.Element("00").Value;

        foreach (string name in query)
        {
            Debug.Log("THe clip: " + name);
        }
    }

    // Use this for initialization
    void Awake() {
        extract3();
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


      
        var clip = track.CreateDefaultClip();
        clip.start = 0.0;
        clip.duration = 4;
        clip.displayName = "justcreated";


        // Camera 1
        CinemachineVirtualCamera left_angle = (CinemachineVirtualCamera)prim_cams[0];
        var cinemachineShot = clip.asset as CinemachineShot;
        cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        


        // Camera 2
        CinemachineVirtualCamera right_angle = (CinemachineVirtualCamera)prim_cams[1];
        var clip2 = track.CreateDefaultClip();
        clip2.start = 4;
        clip2.duration = 4;
        var cinemachineShot2 = clip2.asset as CinemachineShot;
        cinemachineShot2.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();

        

        // Camera 3
        GameObject overhead = (GameObject) special_shots[0];
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        var clip3 = ctrack.CreateDefaultClip();
        clip3.start = 8;
        clip3.duration = 8;
        var controlshot =  clip3.asset as ControlPlayableAsset;
        controlshot.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();

        //foreach (KeyValuePair<string, TrackAsset>(exp_name, GameObject) in ClipDict){
        //    director.SetReferenceValue(exp_name, GameObject);
        //}
        director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, left_angle);
        director.SetReferenceValue(cinemachineShot2.VirtualCamera.exposedName, right_angle);
        director.SetReferenceValue(controlshot.sourceGameObject.exposedName, overhead);
        // Set cinemachine brain as track's game object
        director.SetGenericBinding(track, main_camera_object);

        // Set it to play when ready.
        director.Play(timeline);
    }

    // Update is called once per frame
    void Update() {

    }
}