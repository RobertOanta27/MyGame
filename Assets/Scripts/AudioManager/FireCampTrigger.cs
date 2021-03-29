using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCampTrigger : MonoBehaviour
{
    private AudioManager audioManager;
    private GameObject fireCampAmbiance;
    private bool activate = false;
    private bool deactivate = false;


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // if collided with player
        {
            fireCampAmbiance = audioManager.Play("FireCampAmbiance",this.transform.position,this.transform.rotation);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(fireCampAmbiance);
        }
    }


}
