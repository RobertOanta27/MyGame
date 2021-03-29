using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private bool facingRight = true;
    private Transform playeTrans;

    // Start is called before the first frame update
    void Awake()
    {
        playeTrans = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < playeTrans.position.x && !facingRight)
        {
            Flip();
        }
        else if (transform.position.x > playeTrans.position.x && facingRight)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }
}
