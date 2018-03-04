using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;

namespace SteeringNamespace
{

    public class KinematicSeek : MonoBehaviour
    {

        private KinematicSteering steering;
        private SteeringParams sp;
        private Goal goalObject;
        private Transform goal;

        // Use this for initialization
        void Start()
        {
            goalObject = GetComponent<Goal>();
            sp = GetComponent<SteeringParams>();
        }

        // Update is called once per frame
        public KinematicSteering getSteering()
        {
            goal = goalObject.getGoal();
            steering = new KinematicSteering();
            steering.velc = goal.position - this.transform.position;
            steering.velc.Normalize();
            steering.velc = steering.velc * sp.MAXSPEED;
            return steering;
        }
    }
}