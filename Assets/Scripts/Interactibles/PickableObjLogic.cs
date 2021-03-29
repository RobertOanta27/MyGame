using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObjLogic : MonoBehaviour
{
    private GameObject player;
    private PlayerCombat playerCombat;
    private CharacterController2D character;
    private GameObject child;
    private GameObject ligthTorch;

    private bool pickedObj = false;
    [SerializeField] private float torchesDisappearingDelay = 30f;


    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    private void Awake()
    {
        ligthTorch = getChildGameObject(this.gameObject, "LightTorch2D");
        child = this.transform.GetChild(0).gameObject;
        GetComponent<DisableIfFarAway>().enabled = false;
        player = GameObject.Find("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
        character = player.GetComponent<CharacterController2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator torchesDisappearing()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        child.GetComponent<SpriteRenderer>().enabled = false;
        ligthTorch.SetActive(false); // stop light when picked


        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(torchesDisappearingDelay);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        child.GetComponent<SpriteRenderer>().enabled = true;
        ligthTorch.SetActive(true); // reset light


        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name == "Player") // if collided with player
        {
            if (character.getFacingRight())
            {
                playerCombat.PickObj(false);
                pickedObj = true;
            }
            else
            {
                playerCombat.PickObj(true);
                pickedObj = true;
            }
        }
        if (pickedObj && playerCombat.getPickCount() == 1)
        {
            StartCoroutine(torchesDisappearing());
        }
        
    }
}
