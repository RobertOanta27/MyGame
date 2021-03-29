using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 

public class AnimationHandler : MonoBehaviour
{
    Animator animator;
    [Header("Animation Names")]
    [SerializeField] string idle;
    [SerializeField] string hit;
    [SerializeField] string death_transition_bool;
    [SerializeField] string react;
    [SerializeField] string walk;
    [SerializeField] string attack;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void IdleAnimation()
    {
        if (idle != null)
            animator.SetTrigger(idle);
    }

    public void HitAnimation()
    {
        if (hit != null)
            animator.SetTrigger(hit);
    }

    public void DeathAnimation()
    {
        if (death_transition_bool != null)
            animator.SetBool(death_transition_bool, true);
    }

    public void ReactAnimation()
    {
        if (react !=null)
            animator.SetTrigger(react);
    }

    public void WalkAnimation()
    {
        if (walk !=null)
            animator.SetTrigger(walk);
    }

    public void AttackAnimation()
    {
        if (attack !=null)
            animator.SetTrigger(attack);
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public AnimatorStateInfo getCurrentAnimState()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }




}
