using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class set_speed : MonoBehaviour {
    public float speed;
    private Animator anim;
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        speed = rb.velocity.magnitude;
        anim.SetFloat("speed", rb.velocity.magnitude);
        //anim.SetFloat("Speed", r.desiredVelocity.magnitude);
        //speed = nma.desiredVelocity.magnitude;
    }
}
