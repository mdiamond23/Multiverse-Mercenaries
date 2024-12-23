using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float shootRange = 1000f;
    public float shootSpeed = 100f;
    public float bulletDamage = 20;
    public bool shouldDamagePlayer = false;

    public float speedMultiplier = 1f;

    private Vector3 initialPosition;
    public float procChance = 1f;
    //public AudioSource audioSource;

    void Awake()
    {
        initialPosition = transform.position;
        rb.velocity = shootSpeed * transform.up * -1;
        //audioSource = GetComponent<AudioSource>();
        //audioSource.mute = true;    
    }

    void Update() {
        if(Vector3.Distance(initialPosition, transform.position)>= shootRange){
           Destroy(transform.parent.gameObject);    
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        // Damage the other object
        if (other.gameObject.TryGetComponent<Enemy>(out Enemy enemy) 
            && !shouldDamagePlayer) {

            var player = GameObject.FindGameObjectWithTag("Player");
            var playerController = player.GetComponent<PlayerController>();
            float damage = bulletDamage;
            if(playerController.diceChance>=playerController.diceDouble){
                    damage*=2f;
                }
            if(Vector3.Distance(transform.position, player.transform.position) <= playerController.textBookRadius) {
                damage*=1.33f;
            }
            if(playerController.hasRing == true && playerController.playerHealth >= 80) {
                    damage*=3*playerController.ringsNumber;
            }

            //audioSource.mute = false;
            //audioSource.Play();

            enemy.Damage(damage, speedMultiplier);
          
            if(playerController.canPoison = true){
                enemy.Poison(playerController.snakeDamage);
            }

            float chance = Random.value * procChance;
            bool shouldStrike = chance <= playerController.strikeChance && chance > 0;
            if(shouldStrike){
                playerController.OrbitalStrike(enemy.gameObject.transform.position.x);
            }   
               
            Destroy(transform.parent.gameObject);
        } else if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player)
            && shouldDamagePlayer) {
            player.Damage(bulletDamage);
            Destroy(transform.parent.gameObject);
        } else if (other.gameObject.name=="Mirror" && shouldDamagePlayer){
            Vector3 newVelocity = rb.velocity;   
            newVelocity.x *= -1f;  
            newVelocity.y *= -1f;  
            rb.velocity = newVelocity;
            shouldDamagePlayer = false;
        }
    }
}
