using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTriggerGrid : MonoBehaviour
{
    public GameObject[] targets;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject targ in targets)
        {
            Instantiate(targ);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
