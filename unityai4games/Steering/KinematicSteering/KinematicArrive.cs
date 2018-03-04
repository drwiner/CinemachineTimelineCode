using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;

namespace SteeringNamespace
{

    public class KinematicArrive : MonoBehaviour
    {

        private Goal goalObject;
        private Transform goal;
        private SteeringParams sp;
        private KinematicSteering ks;
        public float radius_of_satisfaction = 0.5f;
        public float time_to_target = 0.25f;

        // Use this for initialization
        void Start()
        {
            goalObject = GetComponent<Goal>();
            sp = GetComponent<SteeringParams>();
        }

        // Update is called once per frame
        public KinematicSteering getSteering()
        {
            ks = new KinematicSteering();
            goal = goalObject.getGoal();
            //steering = new Steering();
            Vector3 new_velc = goal.position - transform.position;

            if (new_velc.magnitude < radius_of_satisfaction)
            {
                new_velc = new Vector3(0f, 0f, 0f);
                ks.velc = new_velc;
                return ks;
            }

            new_velc = new_velc / time_to_target;

            // clip speed
            if (new_velc.magnitude > sp.MAXSPEED)
            {
                new_velc.Normalize();
                new_velc = new_velc * sp.MAXSPEED;
            }

            ks.velc = new_velc;

            return ks;
        }
    }
}