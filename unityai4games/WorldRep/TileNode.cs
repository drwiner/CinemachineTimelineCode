using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphNamespace
{
    public class TileNode : MonoBehaviour
    {

        public Material on_material;
        public Material off_material;
        public Material plan_material;
        public Material next_material;
        private Renderer render;
        //private Vector3 position;

        void Start()
        {
            render = GetComponent<Renderer>();
            off_material = render.material;
            //position = GetComponentInParent<Transform>().position;
        }


        void OnTriggerEnter(Collider other)
        {
            render.material = on_material;
        }

        void OnTriggerExit(Collider other)
        {

            StartCoroutine(WaitToChange(1f));
        }

        public void setPlanMaterial()
        {
            render.material = plan_material;
        }

        public void setOffMaterial()
        {
            render.material = off_material;
        }

        public void setNextMaterial()
        {
            render.material = next_material;
        }

        IEnumerator WaitToChange(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            render.material = off_material;
        }

        public override string ToString()
        {
            return "TileNode(" + transform.position.ToString() + ")";
        }

        public bool isEqual(TileNode other_node)
        {
            if (other_node.transform.position == transform.position)
                return true;
            return false;
        }

    }

}