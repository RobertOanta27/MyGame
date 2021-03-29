using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlingWindOutsideTrigger : MonoBehaviour
{
    private AudioManager audioManager;
    private GameObject howlingWindAmbiance;
    private GameObject oldHowlingWindAmbiance;
    private bool howlingWindAmbianceDescendo = false;
    private bool oldHowlingWindAmbianceDescendo = false;

    void Awake()
    {
        audioManager = GetComponentInParent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (howlingWindAmbianceDescendo && howlingWindAmbiance != null) // descendo activators need to be in update  or fixedUpdate actually
        {
            audioManager.Descendo(howlingWindAmbiance, 0.03f);
        }
        if (oldHowlingWindAmbianceDescendo && oldHowlingWindAmbiance != null)
        {
            audioManager.Descendo(oldHowlingWindAmbiance, 0.055f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player") // if collided with player
        {
            if (!GameObject.Find("HowlingWindAmbiance(Clone)"))
            {
                howlingWindAmbiance = audioManager.Play("HowlingWindAmbiance");
                howlingWindAmbianceDescendo = false;
            }
            else
            {
                oldHowlingWindAmbiance = GameObject.Find("HowlingWindAmbiance(Clone)"); // removes the old howling wind ambiance soundObj with a little descendo
                oldHowlingWindAmbianceDescendo = true;
                howlingWindAmbiance = audioManager.Play("HowlingWindAmbiance");
                howlingWindAmbianceDescendo = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player") // if collided with player
        {
            howlingWindAmbianceDescendo = true;
            Debug.Log("Trigger exit wind");
        }
    }
}
