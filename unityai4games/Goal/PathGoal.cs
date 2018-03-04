using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphNamespace;

namespace GoalNamespace
{

    public class PathGoal : MonoBehaviour
    {
        public Node tileGoal;
        private Vector3 goalPosition;

        void Start()
        {
            goalPosition = transform.position;
            tileGoal = null;
        }


        public void setGoal(Node tn)
        {
            tileGoal = tn;
            goalPosition = QuantizeLocalize.Localize(tn);

        }

        public Vector3 getGoal()
        {
            return goalPosition;
        }


    }
}