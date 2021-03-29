using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    [SerializeField] private int attackDamage_player;
    [SerializeField] private int attackDamage_enemy;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if ((collision.CompareTag("Enemy")) && (collision != null))
            collision.GetComponent<EnemyAI_CS>().TakeDamage(attackDamage_enemy);
        else if (collision.name == "Player" && collision != null)
            collision.GetComponent<PlayerCombat>().TakeDamage(attackDamage_player);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
