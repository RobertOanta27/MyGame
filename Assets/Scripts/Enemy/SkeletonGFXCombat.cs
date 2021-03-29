using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGFXCombat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint;
    public LayerMask playerLayer;

    //public Collider2D player;

    public float attackRange = 0.5f;
    public int attackDamage = 20;

    //public float attackRate = 2f;
    //float nextAttackTime = 0f;


    [SerializeField] private float collisionCheckDelay = 1f;

    private Transform player;

    IEnumerator collsionCheck(float collisionCheckDelay)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp  : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(collisionCheckDelay);
        //Detect enemies in range
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        //Damage enemies in range
        foreach (Collider2D player in hitPlayer)
        {
            Debug.Log("Combat mask: we hit player");
            player.GetComponent<PlayerCombat>().TakeDamage(attackDamage); // player take damage
            //Play Sound;

        }


        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp FadeSound : " + Time.time);
    }


    private void Awake() // always set objects references here in awake for better optimisation
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {

        //Play Attack anim
        animator.SetTrigger("Go_To_Attack");

        StartCoroutine(collsionCheck(collisionCheckDelay));

        

    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
