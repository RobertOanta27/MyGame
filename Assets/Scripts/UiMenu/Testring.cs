using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testring : MonoBehaviour
{
    private PlayerGeneral playerGeneral;
    private Ui_SkillTree ui_SkillTree;

    private void Awake()
    {
        playerGeneral = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGeneral>();
        ui_SkillTree = GameObject.Find("Canvas").GetComponent<Ui_SkillTree>();
    }

    private void Start()
    {
        ui_SkillTree.SetPlayerSkills(playerGeneral.GetPlayerSkills());
    }

}
