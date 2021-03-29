using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private bool closeToPortal = false; // is in trigger range
    [SerializeField] private Transform player;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null)
        {
            if (closeToPortal)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TeleportPlayer(getChildGameObject(GameObject.Find("AcidGrennSkullPortal (1)"), "Teleport point").transform.position);
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision != null)
        {
            closeToPortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision != null)
        {
            closeToPortal = false;
        }
    }

    private void TeleportPlayer(Vector3 position) // where to teleport player
    {
        // play teleport anim
        player.position = position;
    }
}
