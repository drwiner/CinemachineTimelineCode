// Work originally by Brandon Thorne (brthorne@ncsu.edu)

using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cinematography
{

    public class CinematographyAttributes : MonoBehaviour
    {

        public static List<ProCamsLensDataTable.FOVData> lensFovData;
        public static NoiseSettings standardNoise;
        public static TextSwitcherTrack discTextTrack;
        public static TextSwitcherTrack fabTextTrack;
        public static Text discText;
        public static Text fabText;


        public static Dictionary<string, ushort> lenses = new Dictionary<string, ushort>()
        {
            {"12mm",0}, {"14mm",1}, {"16mm",2}, {"18mm",3}, {"21mm",4}, {"25mm",5}, {"27mm",6},
            {"32mm",7}, {"35mm",8}, {"40mm",9}, {"50mm",10}, {"65mm",11}, {"75mm",12}, {"100mm",13},
            {"135mm",14}, {"150mm",15}, {"180mm",16}
        };

        public static Dictionary<string, ushort> fStops = new Dictionary<string, ushort>()
        {
            {"1.4",0}, {"2",1}, {"2.8",2}, {"4",3}, {"5.6",4}, {"8",5}, {"11",6}, {"16",7}, {"22",8}
        };


        public static int GetLensIndex(float targetVerticalFov)
        {
            int lensIndex = 0;
            //iterates over the entire array of lenses always.  it has 15 elements, so it doesn't make much difference
            for (int i = 0; i < lensFovData.Count; i++)
            {
                if (lensFovData[i]._unityVFOV > targetVerticalFov)
                {
                    lensIndex = i;
                }
            }
            return lensIndex;
        }


        public static float CalcCameraDistance(GameObject framingTarget, FramingType ft)
        {
            // pick out the framing parameters from the table
            FramingParameters framingParameters = FramingParameters.FramingTable[ft];

            // get the apprpriate lens based on the focal length for the framing parameters
            ushort? tempLensIndex = lenses[framingParameters.DefaultFocalLength];

            // calculate the area of the target
            Bounds targetBounds = framingTarget.GetComponent<BoxCollider>().bounds;

            // calculate the vertical field of view
            float vFov = lensFovData[tempLensIndex.Value]._unityVFOV * Mathf.Deg2Rad;

            float frustumHeight = (1 / framingParameters.TargetPercent) * (targetBounds.max.y - targetBounds.min.y);

            float distanceToCamera = frustumHeight / Mathf.Tan(vFov / 2);

            return distanceToCamera;
        }

    }

    public enum FramingType
    {
        None,
        ExtremeCloseUp,
        CloseUp,
        Waist,
        Full,
        Long,
        ExtremeLong,
        ExtremeLongLong,
        Angle
    }

    public class Framing
    {
        public FramingType FramingType { get; set; }
        public string FramingTarget { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1})", FramingTarget, FramingType.ToString());
        }
    }

    public class FramingParameters
    {
        public static Dictionary<FramingType, FramingParameters> FramingTable = new Dictionary<FramingType, FramingParameters>()
    {
        {FramingType.ExtremeCloseUp, new FramingParameters()
        {
            Type=FramingType.ExtremeCloseUp,
            MaxPercent=20.25f,
            MinPercent=15.01f,
            TargetPercent=18.0f,
            DefaultFocalLength="150mm",
            DefaultFStop="2.8"
        }},
        {FramingType.CloseUp, new FramingParameters()
        {
            Type=FramingType.CloseUp,
            MaxPercent=10.25f,
            MinPercent=8.01f,
            TargetPercent=9.0f,
            DefaultFocalLength="100mm",
            DefaultFStop="2.8"
        }},
        {FramingType.Waist, new FramingParameters()
        {
            Type=FramingType.Waist,
            MaxPercent=2.25f,
            MinPercent=1.75f,
            TargetPercent=2.0f,
            DefaultFocalLength="50mm",
            DefaultFStop="5.6"
        }},
        {FramingType.Full,new FramingParameters()
        {
            Type=FramingType.Full,
            MaxPercent=1.0f,
            MinPercent=0.9f,
            TargetPercent=0.95f,
            DefaultFocalLength="35mm",
            DefaultFStop="8"
        }},
        {FramingType.Long,new FramingParameters()
        {
            Type=FramingType.Long,
            MaxPercent=0.75f,
            MinPercent=0.35f,
            TargetPercent=0.5f,
            DefaultFocalLength="27mm",
            DefaultFStop="16"
        }},
        {FramingType.ExtremeLong, new FramingParameters()
        {
            Type=FramingType.ExtremeLong,
            MaxPercent=0.25f,
            MinPercent=0.01f,
            TargetPercent=0.2f,
            DefaultFocalLength="27mm",
            DefaultFStop="22"
        }},
    };

        public FramingType Type { get; set; }
        public float MaxPercent { get; set; }
        public float MinPercent { get; set; }
        public float TargetPercent { get; set; }
        public string DefaultFocalLength { get; set; }
        public string DefaultFStop { get; set; }
    }

}