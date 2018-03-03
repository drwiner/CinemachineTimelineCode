using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine.Timeline;
using Cinemachine;
using SimpleJSON;

namespace ClipNamespace
{
    public class Clip
    {
    
        public PlayableDirector director;
        public TimelineAsset timeline;

        public string Name;
        public string Type;
        public string step_id;
        public int step_num;
        public float start = 1f;
        public float duration = 1f;

        protected string step_name;

        public Clip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director)
        {
            director = p_director;
            timeline = p_timeline;

            Name = json["name"];
            Type = json["type"];
            start = json["start"].AsFloat;
            duration = json["duration"].AsFloat;


        }

        public void TeleportBind(TeleportObject tpObj, GameObject obj_to_move, Transform start_pos, Transform end_pos) 
        {
            tpObj.ThingToMove.exposedName = UnityEditor.GUID.Generate().ToString();
            tpObj.StartTransform.exposedName = UnityEditor.GUID.Generate().ToString();
            tpObj.EndTransform.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(tpObj.ThingToMove.exposedName, obj_to_move);
            director.SetReferenceValue(tpObj.StartTransform.exposedName, start_pos);
            director.SetReferenceValue(tpObj.EndTransform.exposedName, end_pos);
        }

        public void TextBind(TextSwitcherClip tsc, string message, int fontSize, Color c)
        {
            tsc.template.text = message;
            tsc.template.fontSize = fontSize;
            tsc.template.color = c;
        }

        public void TransformBind(LerpMoveObjectAsset tpObj, GameObject obj_to_move, Transform start_pos, Transform end_pos)
        {
            tpObj.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
            tpObj.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
            tpObj.LerpMoveFrom.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(tpObj.ObjectToMove.exposedName, obj_to_move);
            director.SetReferenceValue(tpObj.LerpMoveTo.exposedName, end_pos);
            director.SetReferenceValue(tpObj.LerpMoveFrom.exposedName, start_pos);
        }

        public void TransformToBind(LerpMoveObjectAsset1 tpObj, GameObject obj_to_move, Transform end_pos)
        {
            tpObj.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
            tpObj.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(tpObj.ObjectToMove.exposedName, obj_to_move);
            director.SetReferenceValue(tpObj.LerpMoveTo.exposedName, end_pos);
        }

        public void AnimateBind(ControlPlayableAsset cpa, GameObject ato)
        {
            cpa.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(cpa.sourceGameObject.exposedName, ato);
        }

        public GameObject makeCustomizedTransform(Vector3 pos, float orientation)
        {
            GameObject t = new GameObject();
            t.transform.position = pos;
            t.transform.rotation = Quaternion.Euler(0f, orientation, 0f);
            return t;
        }

        public static void assignClipAttributes(TimelineClip origin, TimelineClip new_clip, string Name)
        {
            new_clip.start = origin.start;
            new_clip.duration = origin.duration;
            new_clip.displayName = Name;
        }

    }
}
