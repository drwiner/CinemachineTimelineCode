// Work originally by Brandon Thorne (brthorne@ncsu.edu)

using Oshmirto;
using System.Collections.Generic;
namespace Assets.scripts
{
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