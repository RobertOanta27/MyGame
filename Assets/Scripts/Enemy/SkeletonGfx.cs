using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEditor;

public class SkeletonGfx : MonoBehaviour
{
    //useful components
    private EnemyAi enemyAi;
    private Animator animator;
    private Transform Player;

    //Field of view
    [SerializeField] private Transform pfFieldOfView;
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private Transform FovStartPoint;
    private FieldOfView fieldOfView;
    private Vector3 aimDir;

    //health components
    public int maxHealth = 100;
    int currentHealth;

    //attack timeout
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    //Death disabaling component
    private float switchTime = float.PositiveInfinity;
    [SerializeField]private float corpseDissapiringDelay = 0;

    //AudioManager
    private AudioManager audioManager;

    private void Awake()
    {
        enemyAi = GetComponentInParent<EnemyAi>();
        animator = GetComponent<Animator>();
        Player = GameObject.FindWithTag("Player").transform;
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        //direction(field of view) init
        aimDir = new Vector3(1, 1f, 0f);
        //Health init
        currentHealth = maxHealth;
        //Prefab field of view instantiate
        fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        //fov and distance init
        fieldOfView.SetFoV(fov);
        fieldOfView.SetViewDistance(viewDistance);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //raycast field of view old
        //RaycastHit2D fovInfo = Physics2D.Raycast(FOV.position, FOV.right);

        if (enemyAi.getFacingRight())
        {
            aimDir = new Vector3(1, 1f, 0f);
        }
        else if (!enemyAi.getFacingRight())
        {
            aimDir = new Vector3(-1, 1f, 0f);
        }
        
        fieldOfView.SetOrigin(FovStartPoint.position);
        fieldOfView.SetAimDirection(aimDir);
        
        
        if (FindTargetPlayer())
        {
            //enemyAi.setSpeed(2f);
            GetComponentInParent<EnemyAi>().enabled = true;
            //if raycast hit player (enemy sees player)
            if (enemyAi.reachedPlayer())
            {
            //reached player so attack with timeout
                if (Time.time >= nextAttackTime)
                {
                    GetComponent<SkeletonGFXCombat>().Attack();  //atack + anim

                    enemyAi.setCanMove(0);

                    nextAttackTime = Time.time + 1f / attackRate;

                    //audio of attacking
                    audioManager.Play("AxeSwoosh1");
                    audioManager.Play("AxeHitMetal_1",0.2f);
                }
            }
            else
            {
            //didn't reach player, so move towards player

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime  > 0.8f && !animator.IsInTransition(0)) // if attack anim finished
                {
                    enemyAi.setCanMove(1);
                }
    
                if (enemyAi.getIsMoving()) // if enemy moving
                {
                    animator.SetTrigger("Go_To_Walk");
                }
      
            }
            
        }
        else
        {

            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Skeleton_Attack")) // if attack anim finished
            {
                enemyAi.setCanMove(2);
            }
                

            if (enemyAi.getIsMoving()) // if enemy moving
            {
                animator.SetTrigger("Go_To_Walk");
            }
            else
            {
                animator.SetTrigger("Go_To_Idle");
            }


        }
    }

    private bool FindTargetPlayer()
    {
        if (Vector3.Distance(FovStartPoint.position, Player.transform.position) < viewDistance)
        {
            //Player inside viewDistance
            Vector3 dirToPlayer = (Player.transform.position - FovStartPoint.position).normalized;
            if (Vector3.Angle(aimDir,dirToPlayer) < fov / 2f)
            {
                //Player inside field of view
                RaycastHit2D raycastHit2D = Physics2D.Raycast(FovStartPoint.position, dirToPlayer, viewDistance);
                if (raycastHit2D.collider != null)
                {
                    //hit something
                    if (raycastHit2D.collider.transform.name == "Player")
                    {
                        //hit player
                        
                        return true;
                    }
                    else
                    {
                        //hit something else
                        return false;
                    }
                }
            }
        }
        return false;
    }

    // takeDamage repeating activator courotine
    IEnumerator repeatHit(int damage, bool onRepeat, float howManyTimes, float seconds, GameObject soundObj, string possibleDeathCause)
    {
        yield return new WaitForSeconds(seconds);
        TakeDamage(damage, onRepeat, howManyTimes, seconds, soundObj, possibleDeathCause);
    }

    // dissap. of enemy after death courotine
    IEnumerator corpseDissapiring()
    {
        yield return new WaitForSeconds(corpseDissapiringDelay);
        Destroy(enemyAi.gameObject);
    }

    // first takeDamage function
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; //take damage

        animator.SetTrigger("Go_To_Hit"); 

        audioManager.Play("SkeletonBoneCrack"); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // second TakeDamage function
    public void TakeDamage(int damage, bool onRepeat, float howManyTimes,float seconds,GameObject soundObj,string possibleDeathCause)
    {
        
        enemyAi.setSpeed(0.90f * enemyAi.getSpeed()); //Slow down when hit (if on repeat will slow down recursively) everytime called

        currentHealth -= damage; //take damage

        animator.SetTrigger("Go_To_Hit");

        audioManager.Play("SkeletonBoneCrack");

        if (currentHealth <= 0)
        {
            string deathCause = possibleDeathCause;
            Debug.Log(deathCause);
            Die(soundObj,deathCause);
            return;
        } 
        else if (onRepeat && howManyTimes>0) // repeat take damage (hit by a torch)
        {
            howManyTimes--;
            StartCoroutine(repeatHit(damage,onRepeat,howManyTimes,seconds, soundObj,possibleDeathCause)); // couroutine for repeating damage
            
        }
    }

    
    // First die function
    void Die()
    {
        Debug.Log("Enemy died");

        animator.SetBool("IsDead", true); //Die anim

        enemyAi.setCanMove(0);

        Physics.IgnoreLayerCollision(11, 13);

        StartCoroutine(corpseDissapiring()); //dissapering delay

    }

    // Second die function
    void Die(GameObject soundObj,string deathCause) 
    {
        Destroy(soundObj); //destroying sound given in takeDamage
        if (deathCause == "torch")
        {
            audioManager.Play("HoldableTorchExtinguish", 0.6f); // if death by torch it plays extinguish sound
        }
        Die();
        
    }


}
