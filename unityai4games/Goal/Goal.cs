using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoalNamespace
{

    public class Goal : MonoBehaviour
    {

        private GameObject previousGoal;
        public GameObject goalObject;
        private Transform goal;
        private float orientation;
        public Material non_goal_material;
        public Material goal_material;

        private bool changed_goal = false;

        public void setGoal(GameObject new_goal_object)
        {
            goalObject = new_goal_object;
            orientation = 0f;
        }

        public Transform getGoal()
        {
            return goal;
        }

        public float getOrientation()
        {
            return orientation;
        }


        void Start()
        {
            previousGoal = goalObject;
            orientation = 0f;
            goal = goalObject.transform;
            goalObject.GetComponent<Renderer>().material = goal_material;
        }

        void Update()
        {
            if (previousGoal != goalObject)
            {
                changed_goal = true;
                goal = goalObject.transform;
            }

            if (changed_goal)
            {
                previousGoal.GetComponent<Renderer>().material = non_goal_material;
                goalObject.GetComponent<Renderer>().material = goal_material;
                changed_goal = false;
                previousGoal = goalObject;
            }
        }
    }
}