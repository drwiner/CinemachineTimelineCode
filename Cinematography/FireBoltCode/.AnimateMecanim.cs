using UnityEngine;
using System.Collections;
using CM = CinematicModel;

namespace Assets.scripts
{
    public class AnimateMecanim : FireBoltAction
    {
        private string actorName;
        private GameObject actor;
        private string animName;
        private string stateName;
        private Animator animator;
        private AnimationClip animation;
        private AnimationClip state;
        AnimatorOverrideController overrideController;
        private int stopTriggerHash;
        private bool loop;
        private static readonly string animationToOverride = "_87_a_U1_M_P_idle_Neutral__Fb_p0_No_1";
        private static readonly string stateToOverride = "state";
        bool assignEndState = false;

        public static bool ValidForConstruction(string actorName, CM.Animation animation)
        {
            if (string.IsNullOrEmpty(actorName) || animation == null || string.IsNullOrEmpty(animation.FileName))
                return false;
            return true;
        }


        public AnimateMecanim(float startTick, float endTick, string actorName, string animName, bool loop, string endingName) :
            base(startTick, endTick)
        {
            this.actorName = actorName;
            this.animName = animName;
			this.loop = loop;
            this.assignEndState = !string.IsNullOrEmpty(endingName);
            this.stateName = endingName; 
            stopTriggerHash = Animator.StringToHash("stop");
        }

        public override bool Init()
        {
            //short circuit if this has clearly been initialized before
            if(animator && overrideController && animation && 
                (!assignEndState ||(assignEndState && state)))
            {
                assignAnimations();
                animator.runtimeAnimatorController = overrideController;
                actor.SetActive(true);
                return true;
            }

            if (!findAnimations()) return false;
            //get the actor this animate action is supposed to affect
            if(actor == null &&
               !getActorByName(actorName, out actor))
            {
                Debug.LogError("actor[" + actorName + "] not found.  cannot animate");
                return false;
            }

            //get the actor's current animator if it exists
            animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                animator = actor.AddComponent<Animator>();
            }
            animator.applyRootMotion = false;

            //find or make an override controller
            if (animator.runtimeAnimatorController is AnimatorOverrideController)
            {
                overrideController = (AnimatorOverrideController) animator.runtimeAnimatorController;
            }
            else
            {
                overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/Generic");
                animator.runtimeAnimatorController = overrideController;
            }

            assignAnimations();
            return true;
        }

        private void assignAnimations()
        {
            overrideController[animationToOverride] = animation;
            if (assignEndState)
                overrideController[stateToOverride] = state;
        }

        private bool findAnimations()
        {
            //find animations
            if (ElPresidente.Instance.GetActiveAssetBundle().Contains(animName))
            {
                animation = ElPresidente.Instance.GetActiveAssetBundle().LoadAsset<AnimationClip>(animName);

                if (animation == null)
                {
                    Debug.LogError(string.Format("unable to find animation [{0}] in asset bundle[{1}]", animName, ElPresidente.Instance.GetActiveAssetBundle().name));
                    return false;
                }
                animation.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;                
            }
            else
            {
                Debug.Log(string.Format("asset bundle [{0}] does not contain animation[{1}]", ElPresidente.Instance.GetActiveAssetBundle().name, animName));
                return false;
            }

            if (!string.IsNullOrEmpty(stateName) && ElPresidente.Instance.GetActiveAssetBundle().Contains(stateName))
            {
                assignEndState = true;
                state = ElPresidente.Instance.GetActiveAssetBundle().LoadAsset<AnimationClip>(stateName);
                if (state == null)
                {
                    Debug.LogError(string.Format("unable to find animation [{0}] in asset bundle[{1}]", stateName, ElPresidente.Instance.GetActiveAssetBundle().name));
                    if (state == null) return false;
                }
            }
            else if (!string.IsNullOrEmpty(stateName) && !ElPresidente.Instance.GetActiveAssetBundle().Contains(stateName))
            {
                Debug.Log(string.Format("should have looked up a state animation[{0}] and failed ", stateName));
                return false;
            }
            return true;
        }

        public override void Undo()
		{
		}

        public override void Skip()
        {
            animator.SetTrigger(stopTriggerHash);
        }

        public override void Execute(float currentTime) 
        {
		    //let it roll          
            float at = Mathf.Repeat ((currentTime - startTick)/1000, animation.length);
            animator.CrossFade( "animating", 0, 0, at/animation.length);
	    }

        public override void Stop()
        {
            animator.SetTrigger(stopTriggerHash);
        }
    }
}