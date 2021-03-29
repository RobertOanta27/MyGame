using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : MonoBehaviour
{
    // TO BECOME THE MAIN PLAYER POWERHOUSE @todo
    // Act like a controller for player combat and charact controller 2d

    private PlayerSkills playerSkills;
    private bool canuseSkill1;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        playerSkills = new PlayerSkills();
        rb2d = GetComponent<Rigidbody2D>(); 
    }

    public bool CanUseSkill1()
    {
        canuseSkill1 = playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Skill1);
        return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Skill1);
    }

    public PlayerSkills GetPlayerSkills()
    {
        return playerSkills;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CanUseSkill1();
            if (canuseSkill1 == true)
            {
                Debug.Log("using skill 1");
                rb2d.velocity = Vector2.up * 30; // simple burst in the air to showcase skill1
            }
        }
    }

    private void ExecuteSkill1()
    {
        Debug.Log("excecuting skill1");
    }
}
