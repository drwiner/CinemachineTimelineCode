using UnityEngine;
using System.IO;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using ClipNamespace;

public class ReadClips : MonoBehaviour {
    private string disc_json_file = @"Scripts//CinemachineTimelineCode//FileIO_json//discourse.json";
    private string fab_json_file = @"Scripts//CinemachineTimelineCode//FileIO_json//fabula.json";

    private PlayableDirector fab_director;
    private PlayableDirector disc_director;
    private TimelineAsset fab_timeline;
    private TimelineAsset disc_timeline;

    // Use this for initialization
    void Awake () {
        // read clips
        Debug.Log("Reading Clips");

        // prep fabula
        string fab_file_path = Path.Combine(Application.dataPath, fab_json_file);
        string fab_clips_as_json = File.ReadAllText(fab_file_path);

        fab_timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");
        fab_director = GetComponent<PlayableDirector>();

        // prep discourse
        string disc_file_path = Path.Combine(Application.dataPath, disc_json_file);
        string disc_clips_as_json = File.ReadAllText(disc_file_path);

        disc_timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");
        disc_director = GetComponent<PlayableDirector>();


        fab_director.Play(fab_timeline);
        disc_director.Play(disc_timeline);
    }
	
    public void ReadFabClipList(string clips_as_json)
    {
        Debug.Log("Reading Fabula");
        var C = JSON.Parse(clips_as_json);
        foreach (JSONNode clip in C)
        {
            Debug.Log(clip.ToString());
            if (clip["type"] == "navigate")
            {
                new NavigateFabulaClip(clip, fab_timeline, fab_director);
                // create a navigate action
            }
            else if (clip["type"] == "stationary")
                new StationaryFabulaClip(clip, fab_timeline, fab_director);
            
        }

    }

    public void ReadDiscClipList(string clips_as_json)
    {
        Debug.Log("Reading Discourse");
        var C = JSON.Parse(clips_as_json);
        foreach (JSONNode clip in C)
        {
            Debug.Log(clip.ToString());
            if (clip["type"] == "nav_cam")
            {
                new NavCustomDiscourseClip(clip, disc_timeline, disc_director);
            }
            else if (clip["type"] == "nav_virtual")
            {
                new NavVirtualDiscourseClip(clip, disc_timeline, disc_director);
            }
        }

    }

}
