using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveZoneTrigger : MonoBehaviour
{

    private AudioManager audioManager;
    private GameObject caveAmbiance;
    private GameObject oldCaveAmbiance;
    private bool caveAmbianceDescendo=false;
    private bool oldCaveAmbianceDescendo = false;

    // Start is called before the first frame update
    void Awake()
    {
        audioManager = GetComponentInParent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (caveAmbianceDescendo && caveAmbiance != null) // descendo activators need to be in update  or fixedUpdate actually
        {
            audioManager.Descendo(caveAmbiance,0.03f);
        }
        if (oldCaveAmbianceDescendo && oldCaveAmbiance != null) 
        {
            audioManager.Descendo(oldCaveAmbiance, 0.055f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player") // if collided with player
        {
            if (!GameObject.Find("CaveAmbiance(Clone)"))
            {
                caveAmbiance = audioManager.Play("CaveAmbiance");
                caveAmbianceDescendo = false;
            }
            else
            {
                //Destroy(GameObject.Find("CaveAmbiance(Clone)"));
                oldCaveAmbiance = GameObject.Find("CaveAmbiance(Clone)"); // removes the old cave ambiance soundObj with a little descendo
                oldCaveAmbianceDescendo = true;
                caveAmbiance = audioManager.Play("CaveAmbiance");
                caveAmbianceDescendo = false;
            }
            
            //caveAmbiance = audioManager.Play("CaveAmbiance", 54.0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player") // if collided with player
        {
            caveAmbianceDescendo = true;
            Debug.Log("Trigger exit cave");
        }
    }
}
