using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    //ai comp
    private Transform targetPlayer;
    [SerializeField] private float speed;
    [SerializeField] private float distance; // endReachedDistance
    private int canMove = 0; // 0=false no movement 1=movement to player 2=patrolling
    


    //flip component
    protected bool m_FacingRight = true;


    public float getDistance() { return distance; }
    public float getSpeed() { return speed;  }
    public void setSpeed(float newSpeed) { speed = newSpeed; }
    public bool getFacingRight() { return m_FacingRight;  }
    public int getCanMove() { return canMove;  }
    public void setCanMove(int newCanMove) { canMove = newCanMove; }
    public bool getIsMoving() { return isMoving; }


    //patrol

    // Points to move to in order
    public Transform[] targets;

    // Time in seconds to wait at each target
    public float delay = 0;

    // Current target index
    int index;

    float switchTime = float.PositiveInfinity;

    private float old_pos;
    private bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        old_pos = transform.position.x;
        targetPlayer = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {

        //Debug.Log("fixed1");
        if (canMove == 1)
        {
           
            moveTowardsPlayer();
            ////Flip sprite script
            if (transform.position.x < targetPlayer.position.x && !m_FacingRight)
            {
                Flip();

            }
            else if (transform.position.x > targetPlayer.position.x && m_FacingRight)
            {
                Flip();
            }
        }
        else if (canMove == 2)
        {
            if (targets.Length != 0)
            {
                moveTowardsTargets();
                ////Flip sprite script
                if (transform.position.x < targets[index].position.x && !m_FacingRight)
                {
                    Flip();

                }
                else if (transform.position.x > targets[index].position.x && m_FacingRight)
                {
                    Flip();
                }
            }
        }
           

        if (old_pos < transform.position.x || old_pos > transform.position.x)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        old_pos = transform.position.x;
        //Debug.Log(isMoving);


    }

    protected void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    public void moveTowardsPlayer()
    {
        
        //Debug.Log("mov player");
        transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);
        
    }

    public bool reachedPlayer()
    {
        if (Vector2.Distance(transform.position, targetPlayer.position) >= distance)
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

    private void moveTowardsTargets()
    {
        
        //Debug.Log("mov targets");
        if (reachedTarget() && float.IsPositiveInfinity(switchTime))
        {
            switchTime = Time.time + delay;
        }

        if (Time.time >= switchTime)
        {
            index = index + 1;
            switchTime = float.PositiveInfinity;
        }

        index = index % targets.Length;
        transform.position = Vector2.MoveTowards(transform.position, targets[index].position, speed * Time.deltaTime);

    }

    private bool reachedTarget()
    {
        if (Vector2.Distance(transform.position, targets[index].position) < 0.2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DestoryObj()
    {
        Destroy(gameObject);
    }

}
