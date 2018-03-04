using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;

namespace SteeringNamespace
{

    public class DynoSeek_PathFollow : MonoBehaviour
    {

        private SteeringParams sp;
        private DynoSteering steering;
        public float changeGoalRadius = 0.4f;
        private Vector3 direction;
        private float distance;
        public bool changeGoal { get; set; }

        // Use this for initialization
        void Start()
        {
            sp = GetComponent<SteeringParams>();
        }

        // Update is called once per frame
        public DynoSteering getSteering(Vector3 goal)
        {
            steering = new DynoSteering();

            direction = goal - transform.localPosition;
            distance = direction.magnitude;
            Debug.Log(distance);

            if (distance < changeGoalRadius) {
                changeGoal = true;
                return steering;
            }
            if (changeGoal)
                changeGoal = false;

            steering.force = direction;
            steering.force.Normalize();
            steering.force = steering.force * sp.MAXACCEL;
            steering.torque = 0f;

            return steering;
        }
    }

}