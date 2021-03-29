using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_SkillTree : MonoBehaviour
{

    private GameObject uiTree;
    private GameObject skill1Button;
    private Transform fireCamp;
    private GameObject player;
    private PlayerSkills playerSkills;
    
    private void Awake()
    {
        uiTree = GameObject.Find("Ui_Skill_Tree");
        fireCamp = GameObject.Find("FireCamp").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        skill1Button = GameObject.Find("SkillButton");
        skill1Button.SetActive(false);
    }

    public void ClickSkill1()
    {
        //clicked the skill1 button
        playerSkills.UnlockSkill(PlayerSkills.SkillType.Skill1);
        Debug.Log("Click");
    }
    public void Update()
    {
        // shows the skill1 button if you are close to campfire
        if (Vector2.Distance(player.transform.position, fireCamp.position) <= 5) // if close to campfire
        {
            skill1Button.SetActive(true);
        } 
        else if (Vector2.Distance(player.transform.position, fireCamp.position) > 5) // if far from campfire
        {
            skill1Button.SetActive(false);
        }
    }

    public void SetPlayerSkills(PlayerSkills playerSkills)
    {
        this.playerSkills = playerSkills;
    }
}
