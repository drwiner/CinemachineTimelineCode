using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;

namespace SteeringNamespace
{

    public class UnityDynoArrive : MonoBehaviour
    {

        private Goal goalObject;
        private Transform goal;
        private SteeringParams sp;
        private DynoSteering ds;
        private Rigidbody charRigidBody;
        public float goalRadius = 0.5f;
        public float slowRadius = 2.5f;
        public float time_to_target = 0.25f;
        private Vector3 direction;
        private float distance;
        private float targetSpeed;
        private Vector3 targetVelocity;

        // Use this for initialization
        void Start()
        {
            goalObject = GetComponent<Goal>();
            sp = GetComponent<SteeringParams>();
            charRigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        public DynoSteering getSteering()
        {

            ds = new DynoSteering();
            goal = goalObject.getGoal();

            direction = goal.position - transform.position;
            distance = direction.magnitude;

            if (distance > slowRadius)
            {
                targetSpeed = sp.MAXSPEED;
            }
            else
            {
                targetSpeed = sp.MAXSPEED * distance / slowRadius;
            }

            targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity = targetVelocity * targetSpeed;
            ds.force = targetVelocity - charRigidBody.velocity;
            ds.force = ds.force / time_to_target;

            if (ds.force.magnitude > sp.MAXACCEL)
            {
                ds.force.Normalize();
                ds.force = ds.force * sp.MAXACCEL;
            }
            ds.torque = 0f;

            return ds;
        }
    }
}