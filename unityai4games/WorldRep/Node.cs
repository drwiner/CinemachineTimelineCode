using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public override string ToString()
    {
        return "Node(" + transform.position.ToString() + ")";
    }
}
