using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwObjLogic : MonoBehaviour
{
    [Header("Object Physics Components")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float upSpeed = 5f;
    [SerializeField] private float revSpeed = 50f;   
    Rigidbody2D rb;



    //Audio  
    AudioManager audioManager;

    //Damage To Enemy
    [Header("Enemy Damage Components")]
    [SerializeField] private int damage = 5;
    [Tooltip("How many times(+1) take damage is called")]
    [SerializeField] private float howManyTimes =4;
    [Tooltip("How often(seconds) take damage is called")]
    [SerializeField] private float howOften=1.5f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed + transform.up * upSpeed;
        
    }

    private void FixedUpdate()
    {
        rb.SetRotation(rb.rotation + revSpeed*Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameObject soundObj = audioManager.Play("TorchThrowHit");
            collision.GetComponent<EnemyAI_CS>().TakeDamage(damage,true,howManyTimes,howOften,soundObj,"torch");
        }
        Destroy(gameObject);
    }


}
