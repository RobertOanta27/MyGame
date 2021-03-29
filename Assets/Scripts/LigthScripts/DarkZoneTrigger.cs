using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DarkZoneTrigger : MonoBehaviour
{
    [SerializeField] private Light2D lightForDarkZone;
    [SerializeField] private float UpperIntensityTreshhold; // upper maximum value of the intensity of the light
    private bool activate = false;
    private bool deactivate = false;
    [SerializeField] private float howFastCresc = 0.1f; // how fast should the lighting reach the threshold
    [SerializeField] private float howFastDesc = 0.1f; // how fast should the lighting reach 0 intensity



    // Start is called before the first frame update
    void Awake()
    {
        lightForDarkZone = GameObject.Find("LightForDarkAreas").GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lightForDarkZone != null)
        {
            if (activate)
            {
                if (lightForDarkZone.intensity < UpperIntensityTreshhold)
                {
                    lightForDarkZone.intensity = lightForDarkZone.intensity + howFastCresc * Time.fixedDeltaTime;
                }
                else
                {
                    activate = false;
                }
            }
            else if (deactivate)
            {
                if (lightForDarkZone.intensity >= 0)
                {
                    lightForDarkZone.intensity = lightForDarkZone.intensity - howFastDesc * Time.fixedDeltaTime;
                }
                else
                {
                    deactivate = false;
                }
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            activate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            deactivate = true;
        }
    }
}
