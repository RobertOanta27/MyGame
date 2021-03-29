using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObj : MonoBehaviour
{
    private bool follow=false;
    private Transform objToFollow;
    private Transform thisObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (follow && objToFollow !=null && thisObj == null)
        {
            this.transform.position = objToFollow.transform.position;
        }
        if (follow && thisObj != null && objToFollow != null)
        {
            thisObj.transform.position = objToFollow.transform.position;
        }
    }

    public void FollowObject(Transform obj,bool activated)
    {
        if (activated && obj !=null)
        {
            objToFollow = obj;
            follow = true;
        }
        else
        {
            follow = false;
        }
            
    }

    public void FollowObject(Transform thisObj,Transform obj, bool activated)
    {
        if (activated && obj != null)
        {
            objToFollow = obj;
            this.thisObj = thisObj;
            follow = true;
        }
        else
        {
            follow = false;
        }

    }
}
