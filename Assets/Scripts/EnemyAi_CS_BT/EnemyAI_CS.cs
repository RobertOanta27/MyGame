using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class EnemyAI_CS : MonoBehaviour
{
    //useful components
    protected Transform targetPlayer;
    protected float old_pos;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool rb;
    protected bool hitByPlayerVar = false;
    protected bool jumpedByPlayerVar = false;
    protected AnimationHandler animationHandler;
    protected SoundHandler soundHandler;
    [SerializeField] string graphicsName;
    protected float oldSpeed;
    protected bool fireStarter = true;

    public float getSpeed() { return speed; }
    public void setSpeed(float newSpeed) { speed = newSpeed; }
    public bool getIsMoving() { return isMoving; }

    //BT obj
    protected PandaBehaviour pandaBT;

    //AudioManager
    protected AudioManager audioManager;

    //Field of view
    [Header("Field Of View")]
    [Tooltip("Field of view prefab for testing")]
    [SerializeField] protected Transform pfFieldOfView; // only for testing
    protected FieldOfView fieldOfView;                  // only ...
    [Tooltip("How large is the angle ( 90f = perpendicular angle )")]
    [SerializeField] protected float fov = 90f;
    [Tooltip("How far does it go")]
    [SerializeField] protected float viewDistance = 5f;
    protected Transform fovStartPoint;
    protected Vector3 aimDir;

    //Enemy components
    [Header("Enemy components")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2;
    [SerializeField] protected float endReachedDistance = 2;
    protected int currentHealth;


    //Death disabaling component
    protected float switchtTimeDeath = float.PositiveInfinity;


    [Header("Enemy Combat")]
    [Tooltip("Gizmo Mover && Range of the attack")]
    [SerializeField] protected float attackRange = 0.5f;
    [SerializeField] protected int attackDamage = 20;
    [SerializeField] protected GameObject fireAnimHitByTorch_pf;
    protected GameObject fireAnimHitByTorchobj;

    //attack timeout
    [Tooltip("Attack Rate ( smaller -> slower attack rate)")]
    [SerializeField] protected float attackRate = 2f;
    protected float nextAttackTime = 0f;

    [Tooltip("Attack collision checking delay")]
    [SerializeField] protected float collisionCheckDelay = 1f;
    [Tooltip("Enemy destruction delay")]
    [SerializeField] protected float corpseDissapiringDelay = 0;
    [Tooltip("Must be bigger than take damage onRepeat hit because it will get recalled")]
    [SerializeField] protected float howMuchTimeChasePlayerHit = 10f;
    [Tooltip("Literally when the enemy sees the player it will start a couroutine for x sec to find him")]
    [SerializeField] protected float howMuchTimeChasePlayerJumped = 3f;

    protected Transform attackPoint;
    protected Transform fireAnimHitByTorchPoint;
    [SerializeField]protected LayerMask playerLayer;

    //Patrol
    // Current target index
    [Header("Patrol")]
    [SerializeField] protected float waitingAtTargetsDelay;
    // Points to move to in order
    [Tooltip("Patrol targets transform")]
    [SerializeField] protected Transform[] targets;
    protected float switchTimePatrol = float.PositiveInfinity;
    protected int index;

    //flip component
    protected bool facingRight = true;



    //Return Child GameObject with name
    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    public virtual void Awake()
    {
        // useful
        targetPlayer = GameObject.FindWithTag("Player").transform;
        audioManager = FindObjectOfType<AudioManager>();
        pandaBT = GetComponent<PandaBehaviour>();
        rb = GetComponent<Rigidbody2D>();
        old_pos = transform.position.x;

        //fov
       fovStartPoint = getChildGameObject(this.gameObject, "FovStartPoint").transform;
        if (fovStartPoint == null)
        {
            Debug.LogWarning("Null FovStartPoint");
        }
        aimDir = new Vector3(1, 1f, 0f); // aim direction(field of view init) best leave it like this
        fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fieldOfView.SetFoV(fov);
        fieldOfView.SetViewDistance(viewDistance);

        //combat
        currentHealth = maxHealth;
        try
        {
            fireAnimHitByTorchPoint = getChildGameObject(this.gameObject, "FireHitByTorchPoint").transform;
        }
        catch
        {
            Debug.LogWarning("FireHitByTorchPoint is null");
        }
       
        attackPoint = getChildGameObject(this.gameObject, "AttackPoint").transform;
        if (attackPoint == null)
        {
            Debug.LogWarning("Null AttackPoint");
        }

        //Handlers
        animationHandler = getChildGameObject(this.gameObject, graphicsName).GetComponent<AnimationHandler>();
        soundHandler = getChildGameObject(this.gameObject, graphicsName).GetComponent<SoundHandler>();
    }

    public virtual void FixedUpdate()
    {
        pandaBT.Reset();
        pandaBT.Tick();
        fieldOfView.SetOrigin(fovStartPoint.position);
        fieldOfView.SetAimDirection(aimDir);
        if (old_pos < transform.position.x || old_pos > transform.position.x)
        {
            isMoving = true;
            
        }
        else
        {
            isMoving = false;
        }

        if (isMoving)
        {
            animationHandler.WalkAnimation();
        }
        else if (!isMoving && !isAttacking)
        {
            animationHandler.IdleAnimation();
        }

        if (transform.position.x > old_pos && !facingRight)
        {
            Flip();
            aimDir = new Vector3(1, 1f, 0f);
        }
        else if (transform.position.x < old_pos && facingRight)
        {
            Flip();
            aimDir = new Vector3(-1, 1f, 0f);
        }
        old_pos = transform.position.x;
    }

    protected void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    [Task]
    public virtual bool FindTargetPlayer()
    {
        if (Vector3.Distance(fovStartPoint.position, targetPlayer.transform.position) < viewDistance)
        {
            //Player inside viewDistance
            Vector3 dirToPlayer = (targetPlayer.transform.position - fovStartPoint.position).normalized;
            if (Vector3.Angle(aimDir, dirToPlayer) < fov / 2f)
            {
                //Player inside field of view
                RaycastHit2D raycastHit2D = Physics2D.Raycast(fovStartPoint.position, dirToPlayer, viewDistance);
                if (raycastHit2D.collider != null)
                {
                    //hit something
                    if (raycastHit2D.collider.transform.name == "Player")
                    {
                        //hit player
                        jumpedByPlayerVar = true;
                        StartCoroutine(JumpedByPlayerChase(howMuchTimeChasePlayerJumped));
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

    [Task]
    protected void MoveTowardsPlayer()
    {
        //Debug.Log("mov player");
        transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);
        
    }

    [Task]
    protected virtual bool ReachedPlayer()
    {
        if (Vector2.Distance(transform.position, targetPlayer.position) >= endReachedDistance)
        {
            //Debug.Log("enemy didn't reach player");
            return false;

        }
        else
        {
            //Debug.Log("enemy reached player");
            return true;
        }
    }

    public virtual IEnumerator collsionCheck(float collisionCheckDelay) // delay on collision checking for accurate hits
    {
        yield return new WaitForSeconds(collisionCheckDelay);
        isAttacking = false;
        //Detect enemies in range
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        //Damage enemies in range
        foreach (Collider2D player in hitPlayer)
        {
            Debug.Log("Combat mask: we hit player");
            player.GetComponent<PlayerCombat>().TakeDamage(attackDamage); // player take damage
            //Task.current.Succeed();
            

        }
    }

    [Task]
    public virtual void Attack()
    {
        // call animator script for graphics and set the animation to attack one
        if (Time.time >= nextAttackTime)
        {
            animationHandler.AttackAnimation();

            soundHandler.AttackSound();

            isAttacking = true;

            StartCoroutine(collsionCheck(collisionCheckDelay));

            nextAttackTime = Time.time + 1f / attackRate; // timeout
        }

    }

    protected void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    [Task]
    protected void MoveTowardsTargets()
    {

        //Debug.Log("mov targets");
        if (ReachedTarget() && float.IsPositiveInfinity(switchTimePatrol))
        {
            switchTimePatrol = Time.time + waitingAtTargetsDelay;
            
        }

        if ((Time.time >= switchTimePatrol))
        {
            index = index + 1;
            switchTimePatrol= float.PositiveInfinity;
        }

        index = index % targets.Length;
        transform.position = Vector2.MoveTowards(transform.position, targets[index].position, speed * Time.deltaTime);

        Task.current.Succeed();

    }

    protected bool ReachedTarget()
    {
        if (Vector2.Distance(transform.position, targets[index].position) < 0.3)
        {
            return true;
            
        }
        else
        {
            return false;
        }
    }

    [Task]
    protected bool HitByPlayer()
    {
        return hitByPlayerVar;
    }

    [Task]
    public virtual bool JumpedByPlayer()
    {
        return jumpedByPlayerVar;
    }


    [Task]
    public virtual bool Patrolling()
    {
        if (targets != null && targets.Length !=0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Task]
    protected bool Idle()
    {
        //call animator play idle;
        animationHandler.IdleAnimation();
        return true;
    }


    // takeDamage repeating activator courotine
    protected IEnumerator repeatHit(int damage, bool onRepeat, float howManyTimes, float seconds, GameObject soundObj, string possibleDeathCause)
    {
        yield return new WaitForSeconds(seconds);
        TakeDamage(damage, onRepeat, howManyTimes, seconds, soundObj, possibleDeathCause);
    }

    // dissap. of enemy after death courotine
    protected IEnumerator corpseDissapiring()
    {
        yield return new WaitForSeconds(corpseDissapiringDelay);
        Destroy(this.gameObject);
    }

    // first takeDamage function
    public void TakeDamage(int damage)
    {
        hitByPlayerVar = true;
        StartCoroutine(hitByPlayerChase(howMuchTimeChasePlayerHit));

        //speed = 0.90f * speed; //Slow down when hit (if on repeat will slow down recursively) everytime called

        currentHealth -= damage; //take damage

        animationHandler.HitAnimation();

        soundHandler.HitSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator FullStop(float oldSpeed,float howLong)
    {
        speed = 0;
        yield return new WaitForSeconds(howLong);
        speed = oldSpeed;
        Debug.Log("Ended");
        
    }

    // second TakeDamage function
    public virtual void TakeDamage(int damage, bool onRepeat, float howManyTimes, float seconds, GameObject soundObj, string possibleDeathCause)
    {

        hitByPlayerVar = true;
        StartCoroutine(hitByPlayerChase(howMuchTimeChasePlayerHit)); // chasing player for how much time

        //speed = 0.90f * speed; //Slow down when hit (if on repeat will slow down recursively) everytime called

        currentHealth -= damage; //take damage

        Coroutine lastRoutine = null;
        lastRoutine = StartCoroutine(FullStop(speed, 0.6f));

        animationHandler.HitAnimation();

        soundHandler.HitSound();

        if (possibleDeathCause == "torch" && fireStarter && fireAnimHitByTorchPoint !=null && fireAnimHitByTorch_pf!=null)
        {
            fireAnimHitByTorchobj = Instantiate(fireAnimHitByTorch_pf, fireAnimHitByTorchPoint.position, fireAnimHitByTorchPoint.rotation);
            fireAnimHitByTorchobj.GetComponent<Animator>().SetTrigger("Continue_burning");
            fireAnimHitByTorchobj.GetComponent<FollowObj>().FollowObject(fireAnimHitByTorchPoint, true);
            fireStarter = false;
        }

        if (currentHealth <= 0)
        {
            //fireAnimHitByTorchobj.GetComponent<Animator>().SetTrigger("Ash");
            // fireanimhit
            getChildGameObject(fireAnimHitByTorchobj.gameObject, "Point Light 2D").GetComponent<LightDecay>().decayLight(1.15f, true);
            StartCoroutine(deathFlameAnim());
            StopCoroutine(lastRoutine);
            string deathCause = possibleDeathCause;
            Debug.Log(deathCause);
            Die(soundObj, deathCause);
            return;
        }
        else if (onRepeat && howManyTimes > 0) // repeat take damage (hit by a torch)
        {
            howManyTimes--;
            StartCoroutine(repeatHit(damage, onRepeat, howManyTimes, seconds, soundObj, possibleDeathCause)); // couroutine for repeating damage

        }
    }

    protected IEnumerator deathFlameAnim()
    {
        // the tall flame that is generated on the character when hit by a torch 
        // gets destoryed after a certain amount of time
        

        // the enemy dissap delay must be bigger than 1.3f otherwise this corountine
        // WONT EXCECUTE TILL THE END
        yield return new WaitForSeconds(0.5f); // constant number of how long fireTall/fireAnimHitByTorch lasts
        fireAnimHitByTorchobj.GetComponent<Animator>().SetTrigger("Ash");
        yield return new WaitForSeconds(0.75f); 
        Destroy(fireAnimHitByTorchobj);
        Debug.Log("destroyed fire tall");
    }

    protected IEnumerator hitByPlayerChase(float howMuchTimeToChasePlayer)
    {
        if (hitByPlayerVar)
        {
            yield return new WaitForSeconds(howMuchTimeToChasePlayer);
            hitByPlayerVar = false;
        }
        
    }

    protected IEnumerator JumpedByPlayerChase(float seconds)
    {
        if (jumpedByPlayerVar)
        {
            yield return new WaitForSeconds(howMuchTimeChasePlayerJumped);
            jumpedByPlayerVar = false;
        }
    }


    // First die function
    protected void Die()
    {
        Debug.Log("Enemy died");

        speed = 0;

        animationHandler.DeathAnimation();

        pandaBT.enabled = false;
        getChildGameObject(gameObject, graphicsName).GetComponent<AnimationHandler>().enabled = false;


        Physics.IgnoreLayerCollision(11, 13);

        StartCoroutine(corpseDissapiring()); //dissapering delay

    }

    // Second die function
    protected virtual void Die(GameObject soundObj, string deathCause)
    {
        Destroy(soundObj); //destroying sound given in takeDamage
        if (deathCause == "torch")
        {
            audioManager.Play("HoldableTorchExtinguish", 0.6f); // if death by torch it plays extinguish sound
        }
        Die();

    }
}
