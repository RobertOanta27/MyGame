using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAi : EnemyAI_CS
{

    private Transform posAfterAttack;
    private Rigidbody2D m_rigid;
    [SerializeField] LayerMask layerMask;
    private Transform player;
    private bool canAttack=true;

    public override void Awake()
    {
        base.Awake();
        posAfterAttack = getChildGameObject(this.gameObject, "PositionAfterAttack").transform;
        m_rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    public override bool FindTargetPlayer()
    {
        if (Vector3.Distance(fovStartPoint.position, targetPlayer.transform.position) < viewDistance)
        {
            //Player inside viewDistance
            Vector3 dirToPlayer = (targetPlayer.transform.position - fovStartPoint.position).normalized;
            if (Vector3.Angle(aimDir, dirToPlayer) < fov / 2f)
            {
                //Player inside field of view
                RaycastHit2D raycastHit2D = Physics2D.Raycast(fovStartPoint.position, dirToPlayer, viewDistance, layerMask);
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

    private IEnumerator positionAfterAttack()
    {
        // repositions the ghost enemy transform after the attack anim

        yield return new WaitForSeconds(0.833f);
        if (posAfterAttack != null)
        {
            old_pos = this.transform.position.x;
            Debug.Log("call");
            if (facingRight)
            {
                Vector3 newPos = new Vector3(6.90f, 0);
                this.transform.position += newPos;
                isAttacking = false;
            }
            else if (!facingRight)
            {
                Vector3 newPos = new Vector3(-6.90f, 0);
                this.transform.position += newPos;
                isAttacking = false;
            }
        }
        
    }

    public override void FixedUpdate()
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
        
        if (!isAttacking)
        {
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
        }
        //Debug.Log(animationHandler.getCurrentAnimState().length);
        old_pos = transform.position.x;
    }

    public override void Attack()
    {
        // call animator script for graphics and set the animation to attack one
        if (Time.time >= nextAttackTime && (canAttack))
        {
            FlipToPlayer();

            animationHandler.AttackAnimation();

            soundHandler.AttackSound();

            isAttacking = true;

            StartCoroutine(collsionCheck(collisionCheckDelay));
            StartCoroutine(positionAfterAttack()); // reposition ghost after attack

            nextAttackTime = Time.time + 1f / attackRate; // timeout
        }

    }

    public override IEnumerator collsionCheck(float collisionCheckDelay) // delay on collision checking for accurate hits
    {
        yield return new WaitForSeconds(collisionCheckDelay);
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

    private void FlipToPlayer()
    {
        if (transform.position.x < player.position.x && !facingRight)
        {
            facingRight = !facingRight;

            transform.Rotate(0f, 180f, 0f);
        }
        else if (transform.position.x > player.position.x && facingRight)
        {
            facingRight = !facingRight;

            transform.Rotate(0f, 180f, 0f);
        }
    }

    protected override IEnumerator FullStop(float oldSpeed, float howLong)
    {
        speed = 0;
        yield return new WaitForSeconds(howLong);
        speed = oldSpeed - 0.2f;
        Debug.Log("Ended");

    }

    public override void TakeDamage(int damage, bool onRepeat, float howManyTimes, float seconds, GameObject soundObj, string possibleDeathCause)
    {
        seconds = 0.9f; //  how often to repeat take damage overriding the 1.5f how often from throwTorch
        hitByPlayerVar = true;
        StartCoroutine(hitByPlayerChase(howMuchTimeChasePlayerHit)); // chasing player for how much time

        //speed = 0.90f * speed; //Slow down when hit (if on repeat will slow down recursively) everytime called

        currentHealth -= damage; //take damage

        Coroutine lastRoutine = null;
        lastRoutine = StartCoroutine(FullStop(speed, 0.5f)); // 0.5f aprox hit anim duration

        animationHandler.HitAnimation();

        soundHandler.HitSound();

       

        if (possibleDeathCause == "torch" && fireStarter && fireAnimHitByTorchPoint != null && fireAnimHitByTorch_pf != null)
        {
            canAttack = false;
            fireAnimHitByTorchobj = Instantiate(fireAnimHitByTorch_pf, fireAnimHitByTorchPoint.position, fireAnimHitByTorchPoint.rotation);
            fireAnimHitByTorchobj.GetComponent<Animator>().SetTrigger("Continue_burning");
            fireAnimHitByTorchobj.GetComponent<FollowObj>().FollowObject(fireAnimHitByTorchPoint, true);
            fireStarter = false;
        }

        if (currentHealth <= 0) // when dying
        {
            //fireAnimHitByTorchobj.GetComponent<Animator>().SetTrigger("Ash");
            // fireanimhit
            getChildGameObject(fireAnimHitByTorchobj.gameObject, "Point Light 2D").GetComponent<LightDecay>().decayLight(1.15f, true);
            StartCoroutine(deathFlameAnim()); // triggers the tallFlame/fireAnimHitByTorchobj dissapearence
            StopCoroutine(lastRoutine); // stops remaining routines
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


}
