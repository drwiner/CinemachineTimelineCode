using ClipNamespace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;

namespace ClipNamespace
{
    public class TrackManager
    {
        // A manager for a track type that only adds new tracks when there is a conflict
        //public List<TrackAsset> Tracks;
        public TrackAsset currentTrack;
        public List<TimelineClip> TimelineClips;
        private int trackNum = 0;

        private TimelineAsset timeline;

        public TrackManager(TimelineAsset _timeline, string trackName)
        {
            timeline = _timeline;
            currentTrack = timeline.CreateTrack<ControlTrack>(null, "trackName" + trackNum.ToString());
        }

        public TimelineClip CreateClip(float start, float duration, string displayName)
        {
            TimelineClip tc;

            if (FreeInterval(start, start + duration))
            {
                tc = currentTrack.CreateDefaultClip();
            }
            else
            {
                trackNum++;
                currentTrack = timeline.CreateTrack<ControlTrack>(null, "trackName" + trackNum.ToString());
                TimelineClips = new List<TimelineClip>();
                tc = currentTrack.CreateDefaultClip();
            }

            tc.start = start;
            tc.duration = duration;
            tc.displayName = displayName;

            TimelineClips.Add(tc);
            return tc;
        }

        public bool FreeInterval(float start, float end)
        {
            foreach (var clip in TimelineClips)
            {
                if (clip.start >= start && clip.start <= end)
                {
                    return false;
                }
                if (clip.end <= end && clip.end > start)
                {
                    return false;
                }
                if (clip.start <= start && clip.end >= end)
                {
                    return false;
                }

            }
            return true;
        }
    }
}