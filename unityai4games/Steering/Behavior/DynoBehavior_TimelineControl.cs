using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringNamespace
{

    public class DynoBehavior_TimelineControl : MonoBehaviour
    {

        private Kinematic KinematicBody;
        private KinematicSteering ks;
        private SteeringParams SP;

        private KinematicSteeringOutput kso;
        private DynoSeek seek;
        private DynoArrive arrive;
        private DynoAlign align;

        //private DynoSteering ds_force;
        //private DynoSteering ds_torque;

        public bool steering = false;
        public Vector3 currentGoal;

        public float goalRadius = 0.5f;
        public float slowRadius = 2.5f;
        public float angularSlowRadius = 1.2f;
        public float arriveTime = 1f;
        public float alignTime = 1f;

        private Action seekTask;
        private Action alignTask;

        private Vector3 force;
        private float torque;

        // Use this for initialization
        void Start()
        {
            SP = GetComponent<SteeringParams>();
            KinematicBody = GetComponent<Kinematic>();
        }

        public bool isDone()
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

            var targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity = targetVelocity * targetSpeed;

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

            if (rotationSize < goalRadius)
            {
                torque = 0f;
                return;
            }

            float targetRotation;
            // if we're outside the slow Radius
            if (rotationSize > slowRadius)
            {
                targetRotation = SP.MAXROTATION;
            }
            else
            {
                targetRotation = SP.MAXROTATION * rotationSize / slowRadius;
            }

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

        public void Steer(Vector3 origin, Vector3 target, bool departed, bool arrive)
        {
            // assume position already set

            currentGoal = target;

            seekTask = () => Seek(arrive);
            alignTask = Align;

            KinematicBody.position = transform.position;

            if (departed)
            {
                // current Velocity is the position + directino * max speed
                var direction = currentGoal - transform.position;
                var currentVelocity = (transform.position + direction).normalized * SP.MAXSPEED;
                // instantaneously set orientation and velocity
                KinematicBody.setVelocity(currentVelocity);
                KinematicBody.setOrientation(KinematicBody.getNewOrientation(currentVelocity));
            }

            steering = true;
        }
        // Update is called once per frame
        void Update()
        {
            if (steering)
            {
                seekTask();
                alignTask();

                //steering = false;
                kso = KinematicBody.updateSteering(new DynoSteering(force, torque), Time.deltaTime);
                transform.position = new Vector3(kso.position.x, transform.position.y, kso.position.z);
                transform.rotation = Quaternion.Euler(0f, kso.orientation * Mathf.Rad2Deg, 0f);
            }
        }
    }
}