using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SteeringNamespace
{

    public class DynoBehavior_TimelineControl : MonoBehaviour
    {
 
        // Get Componoents
        private SteeringParams SP;
        private PlayableDirector PD;

        // organization of tracks into hierarchical segments
        private List<List<SteeringPlayable>> SteerList;
        private List<bool> LockedList;

        //private DynoSteering ds_force;
        //private DynoSteering ds_torque;

        // bool switches
        public bool steering = false;
        public bool playingClip = false;
        private bool initiatedExternally = false;

        // clip attributes
        public Vector3 currentGoal;
        public Vector3 targetVelocity;

        // steering parameters
        public float slowRadius = 2.5f;
        public float goalRadius = 0.25f;
        public float angularSlowRadius = 1.2f;
        public float angularGoalRadius = 0.05f;
        public float arriveTime = .75f;
        public float alignTime = .25f;

        // tasks to perform every frame
        private Action seekTask;
        private Action alignTask;

        // KinematicBody reads force and torque and returns kso
        public Kinematic KinematicBody;
        // calculated each frame
        public Vector3 force;
        public float torque;
        private KinematicSteeringOutput kso;

        // animation parameters (calculated each frame)
        public float tiltAmountForward;
        public float tiltAmountSideways;

        // Use this for initialization
        //void Awake()
        //{
        //    PD = GameObject.FindGameObjectWithTag("FabulaTimeline").GetComponent<PlayableDirector>();
        //    SP = GetComponent<SteeringParams>();
        //    KinematicBody = GetComponent<Kinematic>();
        //    SteerList = new List<List<SteeringPlayable>>();
        //    //SteerList.Add(new List<SteeringPlayable>());
        //    LockedList = new List<bool>();
        //    //LockedList.Add(false);
        //}

        public void InitiateExternally()
        {
            PD = GameObject.FindGameObjectWithTag("FabulaTimeline").GetComponent<PlayableDirector>();
            SP = GetComponent<SteeringParams>();
            KinematicBody = GetComponent<Kinematic>();
            SteerList = new List<List<SteeringPlayable>>();
            //SteerList.Add(new List<SteeringPlayable>());
            LockedList = new List<bool>();
            initiatedExternally = true;
        }

        public bool IsDone()
        {
            return false;
        }

        public void Seek(bool arrive)
        {
            var direction = currentGoal - transform.position;
            var distance = direction.magnitude;

            if (distance < goalRadius)
            {
                force = Vector3.zero;
                tiltAmountForward = 0f;
                return;
            }

            float targetSpeed;
            if (distance > slowRadius || !arrive)
            {
                targetSpeed = SP.MAXSPEED;
            }
            else
            {
                targetSpeed = SP.MAXSPEED * distance / slowRadius;
                
            }

            tiltAmountForward = targetSpeed;

            //targetVelocity = direction.normalized;
            //targetVelocity.Normalize();
            targetVelocity = direction.normalized * targetSpeed;

            force = targetVelocity - KinematicBody.getVelocity();
            force = force / arriveTime;

            if (force.magnitude > SP.MAXACCEL)
            {
                force.Normalize();
                force = force * SP.MAXACCEL;

            }

        }

        public void Align()
        {

            var targetOrientation = KinematicBody.getNewOrientation(currentGoal - transform.position);
            //rotation = goal.eulerAngles;
            var rotation = targetOrientation - KinematicBody.getOrientation();
            rotation = Kinematic.mapToRange(rotation);
            var rotationSize = Mathf.Abs(rotation);

            if (rotationSize < angularGoalRadius)
            {
                torque = 0f;
                tiltAmountSideways = 0f;
                return;
            }

            float targetRotation;
            // if we're outside the slow Radius
            if (rotationSize > angularSlowRadius)
            {
                targetRotation = SP.MAXROTATION;
            }
            else
            {
                targetRotation = SP.MAXROTATION * rotationSize / slowRadius;
            }

            tiltAmountSideways = targetRotation;

            // Final target rotation combines speed (already in variable) with rotation direction
            targetRotation = targetRotation * rotation / rotationSize;

            torque = targetRotation - KinematicBody.getRotation();
            torque = torque / alignTime;

            var angularAcceleration = Mathf.Abs(torque);

            if (angularAcceleration > SP.MAXANGULAR)
            {
                torque = torque / angularAcceleration;
                torque = torque * SP.MAXANGULAR;
            }
        }

        private float positionRefx, positionRefz;
        public void Steer(Vector3 origin, Vector3 target, bool departed, bool arrive)
        {
            // assume position already set

            currentGoal = target;

            seekTask = () => Seek(arrive);
            alignTask = Align;

            Mathf.Lerp(transform.position.x, origin.x,  1f);
            Mathf.Lerp(transform.position.z, origin.z,  1f);

            //Mathf.SmoothDamp(transform.position.x, origin.x, ref positionRefx, 1f);
            //Mathf.SmoothDamp(transform.position.z, origin.z, ref positionRefz, 1f);
            //transform.position = origin;
            KinematicBody.position = transform.position;

            if (departed)
            {
                // current Velocity is the position + directino * max speed
                var direction = currentGoal - transform.position;
                var currentVelocity = direction.normalized * SP.MAXSPEED;
                // instantaneously set orientation and velocity
                KinematicBody.setVelocity(currentVelocity);
                KinematicBody.setOrientation(KinematicBody.getNewOrientation(currentVelocity));
                transform.position = new Vector3(KinematicBody.position.x, transform.position.y, KinematicBody.position.z);
                transform.rotation = Quaternion.Euler(0f, KinematicBody.getOrientation() * Mathf.Rad2Deg - 90f, 0f);
            }

            steering = true;
            playingClip = true;
        }

        public void InformMasterIsPlaying(int whichList, bool isPlaying)
        {
            LockedList[whichList] = isPlaying;
        }

        public bool CheckMasterIsPlaying(int whichList)
        {
            if (LockedList[whichList])
            {
                return true;
            }
            return false;
        }

        public int Register(SteeringPlayable P, bool isMaster) 
        {
            
            var whichList = SteerList.Count-1;
            if (whichList < 0)
            {
                whichList = 0;
            }

            if (isMaster)
            {
                var newList = new List<SteeringPlayable>();
                newList.Add(P);
                SteerList.Add(newList);
                LockedList.Add(false);
                if (SteerList.Count == 1)
                {
                    return 0;
                }
                return whichList + 1;
            }
            
            if (SteerList[whichList] == null)
            {
                Debug.Log("master should always get added first");
                throw new System.Exception();
            }
            SteerList[whichList].Add(P);
            return whichList;
        }

        //public void RegisterPlayableToList

        // Update is called once per frame
        void Update()
        {
            if (!initiatedExternally)
            {
                steering = false;
                //throw new System.Exception();
            }
            if (steering)
            {
                //Debug.Log(gameObject.name + " " + transform.position);
                
                //Debug.Log(transform.name + force.ToString());
                seekTask();

                // Strategy to prevent sudden re-allignment. 
                if (playingClip)
                {
                    alignTask();
                    //Debug.Log("aligning " + transform.name);
                }

                kso = KinematicBody.updateSteering(new DynoSteering(force, torque), Time.deltaTime);
                transform.position = new Vector3(kso.position.x, transform.position.y, kso.position.z);

                transform.rotation = Quaternion.Euler(0f, kso.orientation * Mathf.Rad2Deg, 0f);
                playingClip = false;

               
            }
        }
    }
}