using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LN.Utilities;
using Impulse.v_1_336;
using UintT = Impulse.v_1_336.Intervals.Interval<Impulse.v_1_336.Constants.ValueConstant<uint>, uint>;
using UintV = Impulse.v_1_336.Constants.ValueConstant<uint>;
using CM = CinematicModel;
using Oshmirto;

namespace Assets.scripts
{
    public class CameraActionFactory
    {
        private static readonly string cameraName = "Pro Cam";
        private static readonly string cameraRig = "Rig";
        public static uint endDiscourseTime = 0;
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

        public static void CreateActions(Story<UintV, UintT, IIntervalSet<UintV, UintT>> story, CM.CinematicModel cinematicModel, string cameraPlanPath, 
                                                     out CameraActionList cameraActionList, out FireBoltActionList discourseActionList)
        {
            cameraActionList = new CameraActionList();
            discourseActionList = new FireBoltActionList();
            CameraPlan cameraPlan = Oshmirto.Parser.Parse(cameraPlanPath);

            if(cinematicModel.MillisPerTick != 1)
            {
                scaleTime(cinematicModel.MillisPerTick, cameraPlan);
            }
            

            uint currentDiscourseTime = 0;
            float previousStoryTimeOffset = 0;
            foreach (Block block in cameraPlan.Blocks)
            {
                float blockStartTime = Single.MaxValue;
                float blockEndTime = Single.MinValue;
                foreach (var fragment in block.ShotFragments)
                {
                    uint fragmentStartTime = currentDiscourseTime++;
                    uint fragmentEndTime = fragmentStartTime + fragment.Duration;
                    
                    if (fragmentStartTime < blockStartTime) //assumes same time scale for discourse and story
                        blockStartTime = fragmentStartTime;                                     
                    if (fragmentEndTime > blockEndTime)
                        blockEndTime = fragmentEndTime;

                    cameraActionList.Add(new ShotFragmentInit(fragmentStartTime, cameraRig, fragment.Anchor, fragment.Height, fragment.Pan,
                        fragment.Lens, fragment.FStop, fragment.Framings, fragment.Direction, fragment.Angle, fragment.FocusPosition));

                    var movementStartTime = fragmentStartTime + 1; //force moves to sort after inits
                    RotateRelative rotateWith = null; //collect all the relative rotations together for the camera so we can avoid representational nightmares
                    foreach (var movement in fragment.CameraMovements)
                    {
                        switch (movement.Type) 
                        {
                            case CameraMovementType.Dolly :
                                switch (movement.Directive)
                                {
                                    case(CameraMovementDirective.With):
                                        cameraActionList.Add(new TranslateRelative(movement.Subject, movementStartTime, fragmentEndTime, cameraRig, false, true, false));
                                        break;
                                    case(CameraMovementDirective.To):
                                        Vector2 destination;
                                        if (movement.Subject.TryParsePlanarCoords(out destination))
                                        {
                                            cameraActionList.Add(new Translate(movementStartTime, fragmentEndTime, cameraRig,
                                                                                null, new Vector3Nullable(destination.x,null,destination.y)));
                                        }
                                        break;
                                }                               
                                break;
                            case CameraMovementType.Crane :
                                switch (movement.Directive)
                                {
                                    case CameraMovementDirective.With:
                                        break;
                                    case CameraMovementDirective.To:
                                        cameraActionList.Add(new Translate(movementStartTime, fragmentEndTime, cameraRig,
                                                                            null, new Vector3Nullable(null, float.Parse(movement.Subject), null)));
                                        break;
                                }
                                break;
                            case CameraMovementType.Pan:
                                switch (movement.Directive)
                                {
                                    case CameraMovementDirective.With:
                                        if (rotateWith != null)
                                        {
                                            rotateWith.AppendAxis(movement.Subject, cameraRig, true, false);
                                        }
                                        else
                                        {
                                            rotateWith = new RotateRelative(movement.Subject, movementStartTime, fragmentEndTime, cameraRig,
                                                                               true, false);
                                        }
                                       
                                        break;
                                    case CameraMovementDirective.To:
                                        cameraActionList.Add(new Rotate(movementStartTime, fragmentEndTime, cameraRig, new Vector3Nullable(null, float.Parse(movement.Subject), null),null));
                                        break;
                                }
                                break;
                            case CameraMovementType.Tilt:
                                switch(movement.Directive)
                                {
                                    case CameraMovementDirective.With: // will this co-execute with pan-with? not currently
                                        if (rotateWith != null)
                                        {
                                            rotateWith.AppendAxis(movement.Subject, cameraRig, false, true);
                                        }
                                        else
                                        {
                                            rotateWith = new RotateRelative(movement.Subject, movementStartTime, fragmentEndTime, cameraRig,
                                                                               false, true);
                                        }
                                        break;
                                    case CameraMovementDirective.To:
                                        cameraActionList.Add(new Rotate(movementStartTime, fragmentEndTime, cameraRig, new Vector3Nullable(float.Parse(movement.Subject), null, null),null));
                                        break;
                                }
                                break;
                            case CameraMovementType.Focus:
                                switch (movement.Directive)
                                {
                                    case CameraMovementDirective.With:
                                        cameraActionList.Add(new Focus(movementStartTime, fragmentEndTime, cameraName, movement.Subject, true));
                                        break;
                                }
                                break;
                        }
                    }
                    if(rotateWith!=null)
                        cameraActionList.Add(rotateWith);
                    // Shake it off
                    if(fragment.Shake > float.Epsilon)
                        cameraActionList.Add(new Shake(movementStartTime, fragmentEndTime, cameraName, fragment.Shake));

                    currentDiscourseTime = fragmentEndTime;
                }
                if (block.StoryTime.HasValue)
                {
                    float currentStoryTimeOffset = block.StoryTime.Value - blockStartTime;
                    discourseActionList.Add(new SetStoryTime(currentStoryTimeOffset, previousStoryTimeOffset, blockStartTime, blockEndTime));
                    previousStoryTimeOffset = block.StoryTime.Value;
                }
            }
            cameraActionList.EndDiscourseTime = currentDiscourseTime;
        }

        private static void scaleTime(uint millisPerTick, CameraPlan cameraPlan)
        {
            
            foreach(var block in cameraPlan.Blocks)
            {
                if (block.StoryTime.HasValue)
                    block.StoryTime = block.StoryTime.Value * millisPerTick;
                foreach(var fragment in block.ShotFragments)
                {
                    fragment.Duration *= millisPerTick;
                }
            }
        }

    }
}
