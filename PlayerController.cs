using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb = null;

    public float moveSpeed = 10f;

    public float sprintMultiplyer = 1.5f;

    public bool isSprinting = false;

    private bool facingRight;

    public float jumpForce = 100f;

    public float jumpModifier = 0f;
    public float coyoteTime = .2f;
    public float coyoteTimeCounter;
    public float jumpBufferTime = .2f;
    public float jumpBufferCounter;

    private bool canDoubleJump;

    public int maxJumps = 1;
    public int jumpsRemaining = 1;

    public LayerMask playerLayer;

    public LayerMask invincibleLayer;

    public float invincibilityTime = .5f;

    public float dashForce = 600f;
    private bool isDashing;
    public float dashCooldown = .5f;
    public float lastDashTime = 0f;

    public Transform groundCheck;

    public Vector2 groundCheckBox = new Vector2(1f, 0.1f);

    public LayerMask groundMask;

    private bool isGrounded;

    [Header("Health")]
    public HealthBar healthBar;

    public GameOverManager deathManager;

    public float playerHealth = 100f;

    public bool deadCheck;

    private float poisonRemaining = 0f;

    private float poisonDamage = 0f;
    private float lastDamagetime = 0f;
    public float regenMulitplier = 1f;

    public float forceFieldChance = 0f;
    public float healingPercentage = 0f;
    public float healthModifier = 1f;

    public bool invincible = false;

    public float reducedDamageMultiplier = 1f;

    public GameObject Plant;

    [Header("Sound")]
    public AudioManager audioManager = null;
    public float lastStepTime = 0f;
    public float stepCooldown = .5f;
    public float stepSeconds = .5f;    

    [Header("Attacking")]
    public float attackSpeed = 1f;
    public float aoeAttackSpeed = 3f;

    public float attackRange = 1f;
    public float aoeAttackRange = 3f;

    public Transform attackPosition;
    public Transform aoeAttackPosition;

    public float nextAttackTime = 0f;
    public float nextAOEAttackTime = 0f;
    public float knockbackForce = 500f;

    public float attackDamage = 10f;
    public float aoeAttackDamage = 5f;

    public float shootCooldown = 4f;
    public float sndShootCooldown = .1f;

    public Transform shootPosition;

    public Transform RotatePoint;

    public float nextShootTime = 0f;
    public float sndNextShootTime = 0f;
    public bool oneHit = false;

    public GameObject bulletPrefab;
    public GameObject lowDamageBulletPrefab;

    public LayerMask enemyLayer;
    public float slowingMultiplier = 1f;
    public float damageModifier = 1f;
    public float cooldownModier = 1f;

    public float textBookRadius = -1f;

    public float padsDamage = 0f;
    public float padsRange = 0f;
    public float padsKnockBack = 0f;
    public Transform padsPosition;
    public bool hasPads = false;
    public float padsCooldown = 1.5f;
    public float nextPadsTime = 0f;
    public float snakeDamage = 0f;
    public bool canPoison = false;
    public float poisonTime = 5f;
    public float strikeChance = 0f;
    public float strikeDamage = 300f;
    public GameObject strikeLocation;
    public bool hasRing = false;
    public float ringsNumber = 0f;
    public float diceChance = 0f;
    public float diceDouble;
    public bool hasBrush = false;
    
    [Header("Visual")]
    public PlayerFlash flash;
    private Animator anim;

    public Animator Akuma;
    public Animator TR7;
    public GameObject AkumaModel;   
    public GameObject TR7Model;
    public GameObject TR7Gun;
    
    public Image primaryCooldownImage;
    public Image secondaryCooldownImage;
    public Image utilityCooldownImage;

    public GameObject forceField;

    [Header("Money")]
    public Money moneyDisplay;
    public float jumpMoney = 0f;

    [Header("Defense")]

    public GameObject activeMirror;

    // public float currentMoney = 0f;

    public ItemManager itemManager;
    
    public float DiscountModifier= 1f;
    public float Stocks = 0f;
    private float lastStocksTime = 0f;
    
    [Header("Class")]
    public bool isNinja = true;
    public GameObject grappleGun;

    void Awake() {
        grappleGun.SetActive(false);
        strikeLocation.transform.parent = null;
    }

    void Start()
    {
        isGrounded = false;
        canDoubleJump = false;
        isDashing = false;
        deadCheck = false;
        facingRight = true;
        hasBrush = false;
        isNinja = GameState.instance.playerIsNinja;

        if (!isNinja)
        {
            grappleGun.SetActive(true);
            TR7Model.SetActive(true);
            AkumaModel.SetActive(false); 
            anim = TR7;
            TR7Gun.SetActive(true);
        } else {
            TR7Model.SetActive(false);
            AkumaModel.SetActive(true); 
            anim = Akuma;
            TR7Gun.SetActive(false);
        }

        moneyDisplay.updateMoneyCounter(GameState.currentMoney);
    }

    void Update()
    {
        if(CutsceneTrigger.isCutsceneOn == true){
            return;
        }
        if (deadCheck == true)
        {
            return;
        }
        Move();
        Jump();
        RegenerateHealth();

        if (isNinja == true)
        {
            Dash();
            MeleeAttack();
            AoeAttack();
        }

        if (isNinja != true)
        {
            Shoot();
        }

        if (poisonRemaining > 0)
        {
            poisonRemaining -= Time.deltaTime;
            Damage(poisonDamage * Time.deltaTime);
        }

        AddStocks();
    }

    private void AddStocks(){
        if (lastStocksTime + 1 <= Time.time && Stocks > 0)
        {
            AddMoney(Stocks);
            lastStocksTime = Time.time;
        }
    }

    private void RegenerateHealth()
    {
        if (lastDamagetime + 3 <= Time.time)
        {
            Heal(regenMulitplier * Time.deltaTime);
        }
    }

    private void Jump()
    {
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("HSpeed", rb.velocity.y);

        bool jump = Input.GetButtonDown("Jump");

        if(isGrounded){
            coyoteTimeCounter = coyoteTime;
        }
        else{
            coyoteTimeCounter -= Time.deltaTime;
        }
        if(Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferTime;
            AddMoney(jumpMoney);
        }
        else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if (coyoteTimeCounter >0f && jumpBufferCounter > 0f)
        {

            anim.SetTrigger("takeOff");

            jumpModifier = Mathf.Min(1000f, jumpModifier);

            rb.velocity = new Vector2(rb.velocity.x, jumpForce+jumpModifier);


            isGrounded = false;
            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;

            audioManager.PlayJumpSound();
        }
          if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    private void FlipPlayer()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        // TODO player is hidden on Z-axis and gun doesn't stay on correct side of player
        Vector3 pos = transform.position;
        pos.z = -1f;
        transform.position = pos;

        Vector3 newPos = TR7Gun.transform.position;
        if(facingRight == false){
            newPos.z = 1f;
        }
        else {
            newPos.z = 2f;
        }
        TR7Gun.transform.position = newPos;  
    }

    private void Move()
    {
        float move = Input.GetAxis("Horizontal");
        bool toggleSprint = Input.GetKeyDown(KeyCode.LeftShift);
        float fHorizontalVelocity = rb.velocity.x;

        if (toggleSprint)
        {
            isSprinting = !isSprinting;
            anim.SetBool("isSprinting", isSprinting);
        }
        if (isSprinting)
        {
            move *= sprintMultiplyer;
        }
        if (
            (facingRight == true && move < 0) ||
            (facingRight == false && move > 0)
        )
        {
            FlipPlayer();
        }

        if (move != 0)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        anim.SetFloat("VSpeed", Mathf.Abs(rb.velocity.x));
        
        if(isNinja == true && isGrounded == true && Mathf.Abs(rb.velocity.x)>1){
        }
        else if(isNinja == false && isGrounded == true && Mathf.Abs(rb.velocity.x)>1){
        }
    }

    private void Dash()
    {
        if (Time.time < lastDashTime + dashCooldown * cooldownModier)
        {
            utilityCooldownImage.fillAmount = (Time.time - lastDashTime) / (dashCooldown *cooldownModier);
        } else {
            utilityCooldownImage.fillAmount = 1;
        }

        if (!Input.GetKeyDown(KeyCode.G) || isDashing 
            || Time.time < dashCooldown + lastDashTime)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dashDirection = (mousePos - transform.position).normalized;

        rb.AddForce(new Vector2(dashDirection.x, dashDirection.y) * dashForce, ForceMode2D.Force);
        SetDashInvincible();
        Invoke("DisableDashInvincible", invincibilityTime);
        audioManager.PlayDash();
        anim.SetBool("isDashing", isDashing);

        lastDashTime = Time.time;
    }

    public void OrbitalStrike(float enemyX){
        Vector3 newPosition = strikeLocation.transform.position;
        newPosition.x = enemyX;
        strikeLocation.transform.position = newPosition;
        strikeLocation.SetActive(true);
        Invoke("DisableOrbitalStrike", 2f);
    }
    
    private void DisableOrbitalStrike() {
        strikeLocation.SetActive(false);
    }

    private async void MeleeAttack()
    {
        if (Time.time < nextAttackTime)
        {
            primaryCooldownImage.fillAmount = 1f - (nextAttackTime - Time.time) / (attackSpeed * cooldownModier);
        } else {
            primaryCooldownImage.fillAmount = 1;
        }

        if (anim.GetBool("isSecondarySwinging") || anim.GetBool("isSwinging"))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            diceDouble = Random.Range(0.0f,1f);
            audioManager.PlaySwing1();
            Collider2D[] enemies =
                Physics2D
                    .OverlapCircleAll(attackPosition.position,
                    attackRange,
                    enemyLayer);

            float chance = Random.value;
            bool shouldStrike = chance <= strikeChance && chance > 0 && enemies.Length > 0;
            if(shouldStrike){
                OrbitalStrike(enemies[Random.Range(0,enemies.Length-1)].gameObject.transform.position.x);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                float damage = (oneHit) ? Mathf.Infinity : attackDamage * damageModifier;

                if(diceChance>=diceDouble){
                    damage*=2f;
                }
                if(Vector3.Distance(transform.position, enemies[i].gameObject.transform.position) <= textBookRadius) {
                    damage*=1.33f;
                }

                if(hasRing == true && playerHealth >= 80f) {
                    damageModifier *= 3*ringsNumber;
                }

                audioManager.SwordHitSound1();

                enemies[i]
                    .gameObject
                    .GetComponent<Enemy>()
                    .Damage(damage, slowingMultiplier);

                if(canPoison = true){
                    enemies[i]
                    .gameObject
                    .GetComponent<Enemy>()
                    .Poison(snakeDamage);
                }
                Heal(damage * healingPercentage);
            }

            nextAttackTime = Time.time + attackSpeed*cooldownModier;
            anim.SetBool("isSwinging", true);
            Invoke("stopSwinging", attackSpeed - .01f);
        }
    }

    private void AoeAttack()
    {
        if (Time.time < nextAOEAttackTime)
        {
            secondaryCooldownImage.fillAmount = (nextAOEAttackTime - Time.time) / (aoeAttackSpeed / cooldownModier);
        } else {
            secondaryCooldownImage.fillAmount = 1;
        }
        
        if (anim.GetBool("isSecondarySwinging") || anim.GetBool("isSwinging"))
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && Time.time > nextAOEAttackTime)
        {
            diceDouble = Random.Range(0.0f,1f);
            audioManager.PlaySwing2();
            Collider2D[] enemies =
                Physics2D
                    .OverlapCircleAll(aoeAttackPosition.position,
                    aoeAttackRange,
                    enemyLayer);

            float chance = Random.value;
            bool shouldStrike = chance <= strikeChance && chance > 0 && enemies.Length > 0;
            if(shouldStrike){
                OrbitalStrike(enemies[Random.Range(0,enemies.Length-1)].gameObject.transform.position.x);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                Enemy enemy = enemies[i]
                    .gameObject
                    .GetComponent<Enemy>();

                float damage = (oneHit) ? Mathf.Infinity : aoeAttackDamage * damageModifier;

                if(diceChance>=diceDouble){
                    damage*=2f;
                }
                if(Vector3.Distance(transform.position, enemies[i].gameObject.transform.position) <= textBookRadius) {
                    damage*=1.33f;
                }

                if(hasRing == true && playerHealth >= 80f) {
                    damageModifier *=3*ringsNumber;
                }

                audioManager.SwordHitSound2();

                enemy.Damage(damage, slowingMultiplier);
                Vector3 direction = (enemy.transform.position
                    - aoeAttackPosition.position).normalized;
                
                if(canPoison = true){
                    enemies[i]
                    .gameObject
                    .GetComponent<Enemy>()
                    .Poison(snakeDamage);
                }

                Rigidbody2D enemyrb = enemy.gameObject.GetComponent<Rigidbody2D>();
                enemyrb.AddForce(new Vector2(direction.x, direction.y) * knockbackForce, ForceMode2D.Force);

                Heal(damage * healingPercentage);
            }

            nextAOEAttackTime = Time.time + aoeAttackSpeed * cooldownModier;
            anim.SetBool("isSecondarySwinging", true);
            Invoke("stopSwinging", attackSpeed - .01f);
        }
    }

    private void stopSwinging()
    {
        anim.SetBool("isSwinging", false);
        anim.SetBool("isSecondarySwinging", false);
    }

    private void padsAttack(){
      if(hasPads == true){
          Collider2D[] enemies =
                Physics2D
                    .OverlapCircleAll(padsPosition.position,
                    padsRange,
                    enemyLayer);
            for (int i = 0; i < enemies.Length; i++)
            {
                float damage = (oneHit) ? Mathf.Infinity : padsDamage * damageModifier;

                if(Vector3.Distance(transform.position, enemies[i].gameObject.transform.position) <= textBookRadius) {
                    damageModifier*=1.33f;
                }

                if(hasRing == true && playerHealth >= 80f) {
                    damageModifier*=3*ringsNumber;
                }

                enemies[i]
                    .gameObject
                    .GetComponent<Enemy>()
                    .Damage(damage, slowingMultiplier);
                
                if(canPoison = true){
                    enemies[i]
                    .gameObject
                    .GetComponent<Enemy>()
                    .Poison(snakeDamage);
                }

                GameObject enemy = enemies[i].gameObject;
                Vector3 direction = (enemy.transform.position - padsPosition.position).normalized;

                Rigidbody2D enemyrb = enemy.GetComponent<Rigidbody2D>();
                enemyrb.AddForce(new Vector2(direction.x, direction.y) * padsKnockBack, ForceMode2D.Force);

                Heal(damage * healingPercentage);
            }
      }  
    }
  
    private void SetDashInvincible()
    {
        isDashing = true;
        gameObject.layer = (int)Mathf.Log(invincibleLayer.value, 2);
    }

    private void DisableDashInvincible()
    {
        gameObject.layer = (int)Mathf.Log(playerLayer.value, 2);
        isDashing = false;
        anim.SetBool("isDashing", isDashing);
    }

    private void FixedUpdate()
    {
        isGrounded = false;

        Collider2D[] colliders =
            Physics2D
                .OverlapBoxAll(groundCheck.position,
                groundCheckBox,
                0,
                groundMask);
        if (colliders.Length == 0)
        {
            return;
        }

        isGrounded = true;
        canDoubleJump = true;
        jumpsRemaining = maxJumps;
    }

    public void Heal(float amount)
    {
        playerHealth = playerHealth + amount;
        playerHealth = Mathf.Min(playerHealth*healthModifier, 100f);
        healthBar.UpdateHealthbar(playerHealth);
        //* healthModifier
    }

    public void Damage(float amount)
    {
        if (invincible || deadCheck) return;

        // 1. play damage flash
        flash.Flash();

        // Deflect damage
        if (Random.value <= forceFieldChance){
            forceField.SetActive(true);
            Invoke("HideForceField", 0.5f);
            return;
        }

        // 2. update the player health
        playerHealth = Mathf.Max(0, playerHealth - amount * reducedDamageMultiplier);
        if(isNinja==true && playerHealth > 0f){
            audioManager.AkumaGetHit();
        }
        else if(isNinja==false && playerHealth > 0f){
            audioManager.TR7GetHit();
        }
        if (oneHit == true)
        {
            playerHealth = 0;
        }
        healthBar.UpdateHealthbar(playerHealth);

        lastDamagetime = Time.time;

        // 3. check if player health is zero
        if (playerHealth <= 0)
        {
            if(isNinja == true){
                audioManager.AkumaDie();
            }
            else{
                audioManager.TR7Die();
                TR7Gun.SetActive(false);
            }
            deathManager.EndGame();
            deadCheck = true;
            anim.SetTrigger("Die");
            // todo maybe disable other anim params?
        }
    }

    public void HideForceField() {
        forceField.SetActive(false);
    }

    public void Poison(float poisonDamage, float poisonDuration)
    {
        poisonRemaining = poisonDuration;
        this.poisonDamage = poisonDamage;
    }

    public void AddMoney(float amount)
    {
        GameState.currentMoney += amount;
        moneyDisplay.updateMoneyCounter(GameState.currentMoney);
    }

    public void ChargeMoney(float amount)
    {
        AddMoney(-amount*DiscountModifier);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
        Gizmos
            .DrawWireCube(groundCheck.position,
            new Vector3(groundCheckBox.x, groundCheckBox.y, 0));
        Gizmos.DrawWireSphere(aoeAttackPosition.position, aoeAttackRange);

    }

    private void Shoot()
    {
        if (Time.time < nextShootTime)
        {
            primaryCooldownImage.fillAmount = 1f - (nextShootTime - Time.time) / (shootCooldown/cooldownModier);
        } else {
            primaryCooldownImage.fillAmount = 1;
        }

        if (Time.time < sndNextShootTime)
        {
            secondaryCooldownImage.fillAmount = 1f - (sndNextShootTime - Time.time) / (sndShootCooldown / cooldownModier);
        } else {
            secondaryCooldownImage.fillAmount = 1;
        }

        Vector3 mousePositionOnScreen =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDirection = mousePositionOnScreen - shootPosition.position;
        float rotZ =
            Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (mousePositionOnScreen.x < transform.position.x && facingRight)
        {
            FlipPlayer();
        }
        else if (mousePositionOnScreen.x > transform.position.x && !facingRight)
        {
            FlipPlayer();
        }
        if (facingRight == true)
        {
            rotZ = Mathf.Clamp(rotZ, -90, 90);
            RotatePoint.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
        }
        else
        {
            if (rotZ > 0)
            {
                rotZ = Mathf.Clamp(rotZ, 90, 180);
            }
            else
            {
                rotZ = Mathf.Clamp(rotZ, -180, -90);
            }

            RotatePoint.rotation =
                Quaternion.Euler(new Vector3(180f, 0, -rotZ));
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
        {
            diceDouble = Random.Range(0.0f,1f);
            var instance = Instantiate(bulletPrefab,
                shootPosition.position,
                RotatePoint.rotation);

            BulletScript bullet = instance.GetComponentInChildren<BulletScript>();
            bullet.speedMultiplier = slowingMultiplier;
            float damage = (oneHit) ? Mathf.Infinity : bullet.bulletDamage * damageModifier;
            bullet.bulletDamage = damage;

            nextShootTime = Time.time + shootCooldown * cooldownModier;
            anim.SetBool("isShooting", true);
            Invoke("stopShooting", .5f);
            audioManager.PlayShootSound();

            Heal(damage * healingPercentage);
        }
        else if (Input.GetMouseButtonDown(1) && Time.time >= sndNextShootTime)
        {
            diceDouble = Random.Range(0.0f,1f);
            var instance = Instantiate(lowDamageBulletPrefab,
                shootPosition.position,
                RotatePoint.rotation);

            BulletScript bullet = instance.GetComponentInChildren<BulletScript>();
            bullet.speedMultiplier = slowingMultiplier;
            float damage = (oneHit) ? Mathf.Infinity : bullet.bulletDamage * damageModifier;
            bullet.bulletDamage = damage;

            sndNextShootTime = Time.time + sndShootCooldown*cooldownModier;
            anim.SetBool("isShooting", true);
            Invoke("stopShooting", sndShootCooldown);
            audioManager.PlaySecondShootSound();

            Heal(damage * healingPercentage);
        }
    }

    private void stopShooting()
    {
        anim.SetBool("isShooting", false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
       if(isSprinting == true && Time.time > nextPadsTime){
           padsAttack();
           nextPadsTime = padsCooldown + Time.time;
       }
    }
}
