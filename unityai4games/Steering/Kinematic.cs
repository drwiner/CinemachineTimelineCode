using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringNamespace
{

    public class Kinematic : MonoBehaviour
    {

        private SteeringParams sp;
        private Vector3 position;
        private float rotation;
        private float orientation;
        private Vector3 velc;
        private float height;
        private KinematicSteeringOutput steering;

        // Use this for initialization
        void Start()
        {
            sp = GetComponent<SteeringParams>();

            position = this.transform.position;
            velc = new Vector3(0f, 0f, 0f);
            rotation = 0f;
            orientation = 0f;
        }

        // Update is called once per frame
        public KinematicSteeringOutput updateSteering(DynoSteering ds, float time)
        {

            steering = new KinematicSteeringOutput();

            // make Updates
            position += velc * time;
            orientation += rotation * time;

            velc += ds.force * time;
            orientation += ds.torque * time;

            if (velc.magnitude > sp.MAXSPEED)
            {
                velc.Normalize();
                velc = velc * sp.MAXSPEED;
            }

            steering.position = position;
            steering.orientation = orientation;

            return steering;
        }


        public void setOrientation(float new_value)
        {
            orientation = new_value;
        }

        public void setRotation(float new_rotation)
        {
            rotation = new_rotation;
        }

        public float getOrientation()
        {
            return orientation;
        }

        public float getNewOrientation(Vector3 new_force)
        {
            new_force.Normalize();
            if (new_force.magnitude > 0f)
            {
                return Mathf.Atan2(-velc.z, velc.x);
            }
            else
            {
                return orientation;
            }
        }

        public Vector3 getVelocity()
        {
            return velc;
        }

        public void setVelocity(Vector3 new_velc)
        {
            velc = new_velc;
        }

        public float getRotation()
        {
            return rotation;
        }


        public static float mapToRange(float radians)
        {
            float targetRadians = radians;
            while (targetRadians <= -Mathf.PI)
            {
                targetRadians += Mathf.PI * 2;
            }
            while (targetRadians >= Mathf.PI)
            {
                targetRadians -= Mathf.PI * 2;
            }
            return targetRadians;
        }

        public static float randomBinomial()
        {
            return (float)(Random.Range(0f, 1f) - Random.Range(0f, 1f));
        }
    }
}