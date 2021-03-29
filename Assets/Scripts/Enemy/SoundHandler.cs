using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Audio prefab names")]
    [SerializeField] string idle;
    [SerializeField] string[] hit;
    [SerializeField] float[] hitDelay;
    [SerializeField] string death;
    [SerializeField] string react;
    [SerializeField] string walk;
    [SerializeField] string[] attack;
    [SerializeField] float[] attackDelay;


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void IdleSound()
    {
        if (idle !=null)
            audioManager.Play(idle);
    }

    public void HitSound()
    {
        if (hit != null)
            for (int i = 0; i < hit.Length; i++)
            {
                audioManager.Play(hit[i], hitDelay[i]);
            }
    }

    public void DeathSound()
    {
        if (death !=null)
            audioManager.Play(death);
    }

    public void ReactSound()
    {
        if (react !=null)
            audioManager.Play(react);
    }

    public void WalkSound()
    {
        if (walk !=null)
            audioManager.Play(walk);
    }

    public void AttackSound()
    {
        if (attack !=null)
            for (int i = 0; i < attack.Length; i++)
            {
                audioManager.Play(attack[i], attackDelay[i]);
            }
    }
}