using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAi : EnemyAI_CS
{
    private Transform plungeHeight;
    [SerializeField] float goUpPlungeTime=1f;
    [SerializeField]LayerMask layerMask;

    public override void Awake()
    {
        base.Awake();
        plungeHeight = getChildGameObject(this.gameObject, "PlungeHeight").transform;
    }

    private IEnumerator GoUpPlunge()
    {
        float oldFov = fov;
        fov = 0f;
        yield return new WaitForSeconds(goUpPlungeTime);
        fov = oldFov; 
    }

    public override bool JumpedByPlayer()
    {
        return false;
    }
    public override void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            animationHandler.AttackAnimation();

            soundHandler.AttackSound();

            isAttacking = true;

            StartCoroutine(collsionCheck(collisionCheckDelay));

            StartCoroutine(GoUpPlunge());

            nextAttackTime = Time.time + 1f / attackRate; // timeout
        }
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
                RaycastHit2D raycastHit2D = Physics2D.Raycast(fovStartPoint.position, dirToPlayer,viewDistance,layerMask);
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
}
