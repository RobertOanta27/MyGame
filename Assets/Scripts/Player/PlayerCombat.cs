using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;
    private CharacterController2D characterController;

    //[SerializeField]private Transform attackPoint;
    [SerializeField]private LayerMask playerLayer;

    [SerializeField]private float attackRange = 0.5f;
    [SerializeField]private int attackDamage = 20;

    [SerializeField]private float attackRate = 2f;
    private float nextAttackTime = 0f;

    private bool isAttacking=false;
    [SerializeField] float collisionCheckDelay; // collision delay with the sword
                                                // tweakable for sync with anim

    private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    //health components
    [SerializeField]private int maxHealth = 100;
    private int currentHealth;

    //Throwing components
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject throwObjPrefab;

    //Picking components
    [SerializeField] private Transform pickPoint;
    [SerializeField] private GameObject pickObjPrefab;
    private GameObject pickObj;
    private int pickCount = 0;
    private bool torchFacingRight = true;
    [SerializeField] private bool movePickableTorch;

    //Audio  
    AudioManager audioManager;


    //Return Child GameObject with name
    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    public bool getMovePickableTorch() { return movePickableTorch; }
    public Transform getPickPoint() { return pickPoint; }
    public int getPickCount() { return pickCount; }
    public GameObject getPickObj() { return pickObj; }
    public void setPickCount(int pickCount) { this.pickCount = pickCount;  }

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController2D>();
        torchFacingRight = characterController.getFacingRight();
        audioManager = FindObjectOfType<AudioManager>();
        attackPoint = getChildGameObject(this.gameObject, "AttackPoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowObj();// throw torch mostly script
            
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Attack();
        }
        

    }

    private void FixedUpdate()
    {
        if (movePickableTorch) // updates pickable torch position should put in fixedupdate
        {
            if (pickObj != null)
            {
                Vector2 newPosition = pickPoint.position;
                pickObj.transform.position = newPosition; // the picked obj follows player at pickpoint pos
                if (characterController.getFacingRight() == true && !torchFacingRight) // flip torch obj
                {
                    FlipTorch();
                }
                else if (characterController.getFacingRight() == false && torchFacingRight)
                {
                    FlipTorch();
                }
            }
            
        }
    }

    private IEnumerator collsionCheck(float collisionCheckDelay) // delay on collision checking for accurate hits
    {
        yield return new WaitForSeconds(collisionCheckDelay);
        //Detect enemies in range
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        //Damage enemies in range
        foreach (Collider2D enemy in hitEnemy)
        {
            Debug.Log("Combat mask: we hit enemy");
            enemy.GetComponent<EnemyAI_CS>().TakeDamage(attackDamage); // enemy take damage
                                                                    
        }
    }

    private IEnumerator attackFinished()
    {
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        animator.SetTrigger("Go_To_Idle");
        Debug.Log("Finished Attacking");
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime) // limits the rate so you can't spam attacks
        {
            //sound to add

            isAttacking = true;
            animator.SetTrigger("Attack");
            StartCoroutine(attackFinished());

            StartCoroutine(collsionCheck(collisionCheckDelay));

            nextAttackTime = Time.time + 1f / attackRate;
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // first pickObj
    public void PickObj(bool facingRight) // pick object script called from pickobjLogic
    {
        if (pickCount == 0)
        {
            Debug.Log("pick");
            pickObj = Instantiate(pickObjPrefab, pickPoint.position, pickPoint.rotation); 
            movePickableTorch = true; // obj can move after player
            torchFacingRight = facingRight; 
            pickCount = 1;
        }
        
    }


    private void ThrowObj() // throwable obj function called from update
    {
        if (pickCount == 1)
        {
            Instantiate(throwObjPrefab, throwPoint.position, throwPoint.rotation);
            pickCount--;
            Destroy(pickObj);
            torchFacingRight = true;
            
        }
        
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        //take damage
        currentHealth -= damage;

        //play hit anim
        //animator.SetTrigger("Go_To_Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    

    void Die()
    {
        Debug.Log("Player died");
        //Die anim
        //animator.SetBool("IsDead", true);

        //Disable the enemy
        this.enabled = false;

    }

    private void FlipTorch()
    {
        // Switch the way the player is labelled as facing.
        torchFacingRight = !torchFacingRight;

        pickObj.transform.Rotate(0f, 180f, 0f);
        // Multiply the player's x local scale by -1.
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;
        //transform.localScale = theScale;

    }
}
