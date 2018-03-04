﻿using UnityEngine;
using System.IO;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using ClipNamespace;
using Cinemachine.Timeline;
using System.Collections.Generic;
using Cinematography;
using Cinemachine;
using UnityEngine.UI;

namespace IceBoltNamespace
{
    public class IceBolt : MonoBehaviour
    {
        public string disc_json_file = @"Scripts//CinemachineTimelineCode//FileIO_json//discourse.json";
        public string fab_json_file = @"Scripts//CinemachineTimelineCode//FileIO_json//fabula.json";

        private PlayableDirector fab_director;
        private PlayableDirector disc_director;
        private TimelineAsset fab_timeline;
        private TimelineAsset disc_timeline;

        private GameObject main_camera_object;
        private GameObject fabTimelineChild;
        private GameObject discTimelineChild;


        // Use this for initialization
        void Awake()
        {
            // read clips
            Debug.Log("Reading Clips");

            fabTimelineChild = GameObject.FindGameObjectWithTag("FabulaTimeline");
            discTimelineChild = GameObject.FindGameObjectWithTag("DiscourseTimeline");

            // prep fabula
            string fab_file_path = Path.Combine(Application.dataPath, fab_json_file);
            string fab_clips_as_json = File.ReadAllText(fab_file_path);

            fab_timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");
            fab_director = fabTimelineChild.GetComponent<PlayableDirector>();

            // prep discourse
            string disc_file_path = Path.Combine(Application.dataPath, disc_json_file);
            string disc_clips_as_json = File.ReadAllText(disc_file_path);

            disc_timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");
            disc_director = discTimelineChild.GetComponent<PlayableDirector>();

            // Load Cinematography Attributes
            ProCamsLensDataTable.Instance.LoadData();
            CinematographyAttributes.lensFovData = ProCamsLensDataTable.Instance.GetFilmFormat("35mm 16:9 Aperture (1.78:1)").GetLensKitData(0)._fovDataset;
            CinematographyAttributes.standardNoise = Instantiate(Resources.Load("Handheld_tele_mild", typeof(NoiseSettings))) as NoiseSettings;

            // Load Film Track Manager
            TrackAttributes.FilmTrackManager = new CinemachineTrackManager(disc_timeline);
            main_camera_object = GameObject.FindGameObjectWithTag("MainCamera");
            disc_director.SetGenericBinding(TrackAttributes.FilmTrackManager.currentTrack, main_camera_object);

            // Load other Track Attributes
            TrackAttributes.TimeTrackManager = new TrackManager(disc_timeline, "timeTravel");
            TrackAttributes.discTextTrack = disc_timeline.CreateTrack<TextSwitcherTrack>(null, "text_track");
            TrackAttributes.fabTextTrack = fab_timeline.CreateTrack<TextSwitcherTrack>(null, "text_track");
            disc_director.SetGenericBinding(TrackAttributes.discTextTrack, GameObject.Find("DiscourseText").GetComponent<Text>());
            fab_director.SetGenericBinding(TrackAttributes.fabTextTrack, GameObject.Find("FabulaText").GetComponent<Text>());

            // read clips
            ReadFabClipList(fab_clips_as_json);
            ReadDiscClipList(disc_clips_as_json);
        }

        void Start()
        {

            disc_director.Play(disc_timeline);
            fab_director.Play(fab_timeline);

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
}