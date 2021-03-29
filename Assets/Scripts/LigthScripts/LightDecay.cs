using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightDecay : MonoBehaviour   
{

    //Decays the lighting to 0 then destroys it call the method in a script

    private float howFast=0.1f; // how fast the light should decay
    private Light2D lightObj;
    private LightFlickerEffect lightFlickerEffect;
    private bool activate=false;

    // Start is called before the first frame update
    void Awake()
    {
        lightFlickerEffect = GetComponent<LightFlickerEffect>();
        lightObj = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activate)
        {
            if (lightFlickerEffect != null)
            {
                lightFlickerEffect.enabled = false;
            }
            if (lightObj.intensity <= 0)
            {
                Destroy(lightObj);
            }
            else
            {
                lightObj.intensity = lightObj.intensity - howFast * Time.fixedDeltaTime;
                //Debug.Log(lightObj.intensity);
            }
            
        }
    }

    public void decayLight(float howFast, bool activate)
    {
        this.activate = activate;
        this.howFast = howFast;
    }

}

