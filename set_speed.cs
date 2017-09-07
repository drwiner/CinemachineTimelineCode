using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class set_speed : MonoBehaviour {
    public float speed;
    Animator anim;
    NavMeshAgent nma;
	// Use this for initialization
	void Start () {
        anim = this.GetComponent<Animator>();
        nma = this.GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        anim.SetFloat("Speed", nma.desiredVelocity.magnitude);
        speed = nma.desiredVelocity.magnitude;
    }
}
