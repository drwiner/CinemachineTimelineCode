using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphNamespace;

namespace GoalNamespace {
    public class SetGoalWithMouseClick : MonoBehaviour {

        private Vector3 clicked;
        public TileGraph tg;
        private PathGoal goal_script;

        // Use this for initialization
        void Start() {
            goal_script = GetComponent<PathGoal>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool false_alarm = false;
                Vector3 mouse_pos = Input.mousePosition;

                if (mouse_pos.x < 0 || mouse_pos.y < 0 || mouse_pos.x > Screen.width || mouse_pos.y > Screen.height)
                {
                    false_alarm = true;
                }

                if (!false_alarm)
                {
                    clicked = cursorOnTransform;
                    Debug.Log("clicked: " + clicked.ToString());

                    Node tn = QuantizeLocalize.Quantize(clicked, tg);
                    goal_script.setGoal(tn);
                }
            }
    

         }

        // https://answers.unity.com/questions/540888/converting-mouse-position-to-world-stationary-came.html#
        private static Vector3 cursorWorldPosOnNCP
        {
            get
            {
                return Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));
            }
        }

        private static Vector3 cameraToCursor
        {
            get
            {
                return cursorWorldPosOnNCP - Camera.main.transform.position;
            }
        }

        private Vector3 cursorOnTransform
        {
            get
            {
                Vector3 camToTrans = transform.position - Camera.main.transform.position;
                return Camera.main.transform.position +
                    cameraToCursor *
                    (Vector3.Dot(Camera.main.transform.forward, camToTrans) / Vector3.Dot(Camera.main.transform.forward, cameraToCursor));
            }
        }

    }
}