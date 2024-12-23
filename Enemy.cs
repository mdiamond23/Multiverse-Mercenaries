using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public float maxHealth = 50f;
    public float attackDamage = 10f;
    public float moneyValue = 100f;

    public Rigidbody2D rb;
    public float speed = 5f;
    public float jumpForce = 0f;

    protected bool facingRight;
    public float nextPoisonTime = 0f;
    public float poisonTimeRemaining = 0f;
    public float poisonDamage = 0f;    

    public float paintTimeRemaning;
    public SpriteRenderer sprite;
    protected PlayerController playerController;


    public GameObject confettiEffect;

    public Animator enemyAnim;

    public EnemyAudioManager EnemyAudioManager = null;
    
    void Awake()
    {
        facingRight = true;
        maxHealth = health;
        sprite = GetComponent<SpriteRenderer>();

    }

    public void MoveForward()
    {
        if (facingRight)
        
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        enemyAnim.SetFloat("enemyVspeed", Mathf.Abs(rb.velocity.x));

        
    }

    public void StopForward()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public virtual void Damage(float damage, float enemySpeed)
    {
        /*if(playerController.hasBrush == true){
            Paint();
        }
        if(paintTimeRemaning > 0f){
            damage *= 2f;
            sprite.color = new Color (1,0,0,1);
        }
        else{
            sprite.color = new Color (1,1,1,1);
        }*/
        speed = 1 / enemySpeed;
        EnemyAudioManager.PlayHitSound();
        health -= damage;
        if (health <= 0)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().AddMoney(moneyValue);
            EnemyAudioManager.PlayDieSound();
            OnDeath();
            Destroy(gameObject);
        }
    }

    public virtual void Poison(float poisonDamage, float poisonTime = 5f) {
        poisonTimeRemaining = poisonTime;
        this.poisonDamage = poisonDamage;
    }

    protected virtual void applyPoisonDamage(){
        health -= poisonDamage;
        if (health <= 0)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().AddMoney(moneyValue);
            OnDeath();
            Destroy(gameObject);
        }
    }
    public virtual void Paint(float paintTime = 3f) {
        paintTimeRemaning = paintTime;
    }


    protected virtual void Update(){
        if(poisonTimeRemaining > 0 && nextPoisonTime < Time.time){
            applyPoisonDamage();
            nextPoisonTime = Time.time + 1f;
        }

        poisonTimeRemaining -= Time.deltaTime;
        paintTimeRemaning -= Time.deltaTime;
        enemyAnim.SetBool("enemyisGrounded", Mathf.Abs(rb.velocity.y) <= 0.05);
        if(paintTimeRemaning < 0f){
            sprite.color = new Color (1,1,1,1);
        }
    }

    public void OnDestroy() {
        OnDeath();
    }

    public virtual void OnDeath() {
        if (confettiEffect != null)
        {
            var confetti = Instantiate(confettiEffect, transform.position, Quaternion.identity);
            confetti.transform.parent = null;
        }
    }

    public virtual void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void enemyJump()
    {
         rb
                .AddForce(new Vector2(0, jumpForce),
                ForceMode2D.Force);
        
    }
}
