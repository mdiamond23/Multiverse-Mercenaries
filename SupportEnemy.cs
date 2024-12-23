using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportEnemy : Enemy
{
    public float teleportRange = 100f;
    public float playerRange = 20f;
    public float teleportCooldown = 5f;
    public float nextTeleportTime = 0f;
    public Transform player;
    public Enemy enemyBeingSupported = null;
    public float nextHealTime = 0f;
    public float healCooldown = 1f;
    public LayerMask enemyLayer;
    public Vector3 spawnLocation;



    private void Start() {
        player = GameObject.FindWithTag("Player").transform;
        spawnLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    
    private void FindEnemies() {
        Collider2D[] enemies = 
            Physics2D.OverlapCircleAll(transform.position, teleportRange, enemyLayer);
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 enemyPos = enemies[i].gameObject.transform.position;
            var enemyScript = enemies[i].gameObject.GetComponent<Enemy>();
            if((Vector3.Distance(player.position, enemyPos) < playerRange) 
                && (enemyScript.health <= enemyScript.maxHealth * .5f)) {

                Vector3 TeleportLocation = new Vector3(enemyPos.x + 1f, enemyPos.y + 1f, enemyPos.z);
                transform.position = TeleportLocation;
                enemyBeingSupported = enemyScript;
                nextTeleportTime = teleportCooldown + Time.time;

                Invoke("StopHelpingEnemy", teleportCooldown - 0.1f);

                break;
            }
        }
    }

    private void HelpEnemy(){
        enemyAnim.SetBool("healing", true);
        enemyBeingSupported.health += enemyBeingSupported.maxHealth * .20f;
        enemyBeingSupported.health = Mathf.Min(enemyBeingSupported.health, enemyBeingSupported.maxHealth);
    }

    private void StopHelpingEnemy(){
        enemyAnim.SetBool("healing", false);
        enemyBeingSupported = null;
        transform.position = spawnLocation;
    }

    void Update()
    {
        if(CutsceneTrigger.isCutsceneOn == true){
            return;
        }
        base.Update();

        if(Time.time >= nextTeleportTime){
            FindEnemies();
        }

        if(enemyBeingSupported != null && nextHealTime <= Time.time){
            HelpEnemy();
            EnemyAudioManager.PlayAttackSound();
            nextHealTime = healCooldown + Time.time;
        }
    }

     private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
