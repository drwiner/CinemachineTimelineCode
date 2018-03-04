using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoalNamespace;
using GraphNamespace;

namespace SteeringNamespace
{

    public class DynoBehavior_PathFollow : MonoBehaviour
    {
        private Stack<Node> currentPath = new Stack<Node>();
        private Vector3 currentGoal;
        private Vector3 prevGoal;
        private PathGoal goalComponent;
        private PathGoalSwitcher goalSetter;

        public TileGraph tg;
        private Node currentTile;
        private Node currentGoalTile;
        private Node nextTile;

        private Kinematic char_RigidBody;
        private KinematicSteering ks;
        private DynoSteering ds;

        private KinematicSteeringOutput kso;
        private DynoSeek_PathFollow seek;
        private DynoArrive_PathFollow arrive;
        private DynoAlign_PathFollow align;

        private DynoSteering ds_force;
        private DynoSteering ds_torque;
        private bool hasPath = false;

        void Start()
        {
            char_RigidBody = GetComponent<Kinematic>();
            goalComponent = GetComponent<PathGoal>();
            arrive = GetComponent<DynoArrive_PathFollow>();
            align = GetComponent<DynoAlign_PathFollow>();
            seek = GetComponent<DynoSeek_PathFollow>();
            goalSetter = GetComponent<PathGoalSwitcher>();
        }

        void Update()
        {
            // check what the current goal is
            currentGoalTile = goalComponent.tileGoal;

            // check if we are at the goal position
            currentTile = QuantizeLocalize.Quantize(transform.position, tg);


            // if we're at the goal position, then pick a new goal
            if (currentTile == currentGoalTile)
            {
                Debug.Log("here");
                goalSetter.SetNextGoal();
                currentGoalTile = goalComponent.tileGoal;
                
                // find path to new goal
                currentPath = PathFind.Dijkstra(tg, currentTile, currentGoalTile);

                // pick next point to seek
                currentGoal = QuantizeLocalize.Localize(currentPath.Pop());
            } 
            else if (!hasPath)
            {
                currentPath = PathFind.Dijkstra(tg, currentTile, currentGoalTile);
                currentGoal = QuantizeLocalize.Localize(currentPath.Pop());
                hasPath = true;
            }


            // if we are not at last point on path
            if (currentPath.Count > 0)
            {
                // seek next point on path
                ds_force = seek.getSteering(currentGoal);

                // pop when seek says we've made it into range and seek the next target
                if (seek.changeGoal)
                {
                    nextTile = currentPath.Pop();
                    currentGoal = QuantizeLocalize.Localize(nextTile);
                    if (currentPath.Count > 0)
                        ds_force = seek.getSteering(currentGoal);
                    else
                        ds_force = arrive.getSteering(currentGoal);
                }
            }
            // otherwise, we are approaching the path goal.  we should arrive.
            else if (currentPath.Count == 0)
            {
                ds_force = arrive.getSteering(currentGoal);
            }


            ds_torque = align.getSteering(currentGoal);

            ds = new DynoSteering();
            ds.force = ds_force.force;
            ds.torque = ds_torque.torque;

            kso = char_RigidBody.updateSteering(ds, Time.deltaTime);
            transform.position = new Vector3(kso.position.x, transform.position.y, kso.position.z);
            transform.rotation = Quaternion.Euler(0f, kso.orientation * Mathf.Rad2Deg, 0f);


        }
    }
}