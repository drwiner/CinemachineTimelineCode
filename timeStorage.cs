using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeStorage : MonoBehaviour {

    public float fab_time;
    public GameObject setFabObj;
    private set_fabula_timeline sfl;
    private bool reset = false; 

    void Start()
    {
        sfl = setFabObj.GetComponent<set_fabula_timeline>();
    }
    void Update()
    {
        if (setFabObj.activeSelf && !reset)
        {
            sfl.setFabTime(fab_time);
            reset = true;
        }

        if (reset && !setFabObj.activeSelf)
        {
            reset = false;
        }
    }


}
