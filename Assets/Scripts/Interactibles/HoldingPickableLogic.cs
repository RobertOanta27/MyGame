using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingPickableLogic : MonoBehaviour
{
    [SerializeField] private float objectDisappearingDelay;
    private GameObject player;
    private PlayerCombat playerCombat;

    //
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator objectDisappearing()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp Holdable : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(objectDisappearingDelay);
        audioManager.Play("HoldableTorchExtinguish");
        playerCombat.setPickCount(0);
        Destroy(gameObject);


        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp Holdable : " + Time.time);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name == "Player") // if holded by player
        {
            StartCoroutine(objectDisappearing());
        }
    }
}
