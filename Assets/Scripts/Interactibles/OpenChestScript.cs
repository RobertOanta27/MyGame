using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChestScript : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private bool closeToBox=false; // is in trigger range

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null)
        {
            if (closeToBox)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    anim.enabled = true;
                }
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision != null)
        {
            closeToBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision != null)
        {
            closeToBox = false;
        }
    }
}
