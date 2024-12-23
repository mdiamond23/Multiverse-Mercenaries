using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MeeleEnemy : Enemy
{
    [Header("Attacking")]
    public float meleeCooldown = 4f;
    public Transform meleePosition;
    public float nextMeleeTime = 0f;
    public float meleeRange = 1f;

    [Header("Movement")]
    public float maxAttackHeightDiff = 5f; // track only if player is in y-range
    public float stopDistance = .25f;

    public LayerMask playerLayer;
    private Transform player;
    public float trackingRange = 10f;

    public Animator meeleEnemyAnim;
 
    

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(CutsceneTrigger.isCutsceneOn == true){
            return;
        }
        base.Update();

        if(Vector2.Distance(transform.position,player.position) > trackingRange){
            return;
        }
        FacePlayer();

        if (Mathf.Abs(player.position.x - transform.position.x) <= stopDistance)
        {
            StopForward();

            if (Mathf.Abs(player.position.y - transform.position.y) <= maxAttackHeightDiff)
            {
                MeleeAttack(); // attack if on same height
            }

            return;
        }
        if(Vector2.Distance(transform.position,player.position) < trackingRange && player.position.y - transform.position.y> 3f && Mathf.Abs(rb.velocity.y) <= 0.05){
            enemyJump();
        }

        MoveForward();
    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    private void MeleeAttack()
    {
        enemyAnim.SetTrigger("enemyMeeleAttack");
        if (Time.time >= nextMeleeTime)
        {   
            nextMeleeTime = Time.time + meleeCooldown;

            Collider2D[] objects =
                Physics2D.OverlapCircleAll(meleePosition.position, meleeRange, playerLayer);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].gameObject.TryGetComponent<PlayerController>(out PlayerController p))
                {
                    //EnemyAudioManager.PlayAttackSound();
                    p.Damage(attackDamage);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(meleePosition.position, meleeRange);
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}
