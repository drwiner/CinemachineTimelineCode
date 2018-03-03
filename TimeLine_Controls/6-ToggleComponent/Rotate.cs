using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour 
{
	void Update () 
    {
        transform.Rotate(transform.up, 200 * Time.deltaTime);
	}
}
