using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    GameObject mainMenu;
    GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainMenu = GameObject.Find("MainMenu");
    }

    public void PlayG()
    {
        player.GetComponent<CharacterController2D>().enabled = true;
        player.GetComponent<PlayerCombat>().enabled = true;
        player.GetComponent<PlayerGeneral>().enabled = true;
        StartCoroutine(WaitP());
    }

    IEnumerator WaitP()
    {
        yield return new WaitForSeconds(0.3f);
        mainMenu.SetActive(false);
    }
}
