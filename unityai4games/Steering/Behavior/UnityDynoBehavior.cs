using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringNamespace
{

    public class UnityDynoBehavior : MonoBehaviour
    {

        private Rigidbody char_RigidBody;
        private KinematicSteering ks;
        private DynoSteering ds;

        private KinematicSteeringOutput kso;
        private UnityDynoArrive arrive;

        // Use this for initialization
        void Start()
        {
            char_RigidBody = GetComponent<Rigidbody>();
            arrive = GetComponent<UnityDynoArrive>();
        }

        // Update is called once per frame
        void Update()
        {
            // Decide on behavior
            ds = arrive.getSteering();

            // Update Kinematic Steering
            char_RigidBody.AddForce(ds.force * 20);
            char_RigidBody.AddTorque(new Vector3(0f, ds.torque * Mathf.Rad2Deg, 0f));
        }
    }
}