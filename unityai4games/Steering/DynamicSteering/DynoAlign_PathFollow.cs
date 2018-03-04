using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;

namespace SteeringNamespace
{


    public class DynoAlign_PathFollow : MonoBehaviour
    {

        //private PathGoal goalObject;
        private Vector3 goal;
        private SteeringParams sp;
        private DynoSteering ds;
        private Kinematic charRigidBody;
        private float targetOrientation;
        public float goalRadius = 0.5f;
        public float slowRadius = 2.5f;
        public float time_to_target = 0.1f;
        private float rotation;
        private float rotationSize;
        private float targetRotation;
        private float angularAcceleration;
        //private  rotation;

        // Use this for initialization
        void Start()
        {
            //goalObject = GetComponent<PathGoal>();
            sp = GetComponent<SteeringParams>();
            charRigidBody = GetComponent<Kinematic>();
        }

        //public virtual DynoSteering getSteering();

        // Update is called once per frame
        public DynoSteering getSteering(Vector3 goal)
        {
            //goal = goalObject.getGoal();

            ds = new DynoSteering();

            //goal.position - transform.position;
            targetOrientation = charRigidBody.getNewOrientation(goal - transform.position);
            //rotation = goal.eulerAngles;
            rotation = targetOrientation - charRigidBody.getOrientation();
            rotation = Kinematic.mapToRange(rotation);
            rotationSize = Mathf.Abs(rotation);

            if (rotationSize < goalRadius)
            {
                return ds;
            }

            // if we're outside the slow Radius
            if (rotationSize > slowRadius)
            {
                targetRotation = sp.MAXROTATION;
            }
            else
            {
                targetRotation = sp.MAXROTATION * rotationSize / slowRadius;
            }

            // Final target rotation combines speed (already in variable) with rotation direction
            targetRotation = targetRotation * rotation / rotationSize;

            ds.torque = targetRotation - charRigidBody.getRotation();
            ds.torque = ds.torque / time_to_target;

            angularAcceleration = Mathf.Abs(ds.torque);

            if (angularAcceleration > sp.MAXANGULAR)
            {
                ds.torque = ds.torque / angularAcceleration;
                ds.torque = ds.torque * sp.MAXANGULAR;
            }

            return ds;
        }

    }
}