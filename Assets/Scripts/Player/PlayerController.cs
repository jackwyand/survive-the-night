using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    Freezer freezer;
    public GameObject mgr;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask whatIsGround;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundLinearDrag;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airSpeed;
    private float horizontalMovement;
    private bool changingDirections => (rb.velocity.x > 0f && horizontalMovement < 0f) || (rb.velocity.x < 0f && horizontalMovement > 0f);
    private bool facingRight = true;
    public bool canMove = true;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    private bool canJump => jumpBufferCounter > 0f && hangTimeCounter > 0f;
    [SerializeField] private float airLinearDrag;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float hangTime = 0.1f;
    [SerializeField] private float jumpBufferLength = 0.1f;
    private float hangTimeCounter;
    private float jumpBufferCounter;
    public List<Sprite> jumpSprites;

    [Header("Ground Collision Variables")]
    [SerializeField] private float groundRayCastLength;
    public bool onGround;

    [Header("Inventory Variables")]
    public InventoryObject inventory;

    [Header("Animator")]
    public Animator anim;

    [Header("Attack Variables")]
    public float attackCooldownStart;
    private float attackCooldown;
    public int damage;
    public int combo;
    private float timeBTWNAttacks;
    public bool attackBuffer;
    public bool isAttacking;
    public Transform attackPos;
    public float attackRange;
    public LayerMask whatIsEnemies;

    [Header("Damage Variables")]
    public int health;
    public int maxHealth;
    public float knockbackForce;
    public float staggerLength;
    public float staggerLinearDrag;

    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerDeath;

    private void Start(){
        rb = GetComponent<Rigidbody2D>();  

        freezer = mgr.GetComponent<Freezer>();

        health = maxHealth;
    }
    private void Update(){
        Vector3 y = transform.position;
        if(y.y <= -1f){
            y.y = -0.93f;
            transform.position = y;
        }
        horizontalMovement = GetInput().x;
        if(Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferLength;
        }
        else{
            jumpBufferCounter -= Time.deltaTime; 
        }

        if(Input.GetKeyDown(KeyCode.O)){
            inventory.Save();
        }
        if(Input.GetKeyDown(KeyCode.P)){
            inventory.Load();
        }
        
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if(onGround){
            anim.SetBool("isGrounded", true);
        }
        else{
            anim.SetBool("isGrounded", false);
        }

        if(horizontalMovement < 0f && facingRight && !isAttacking || horizontalMovement > 0f && !facingRight && !isAttacking){
            if(canMove){
                facingRight = !facingRight;
                transform.Rotate(0f, 180f, 0f);
            }

        }
        Combos();
        AttackBuffer();
        attackCooldown -= Time.deltaTime;
        timeBTWNAttacks += Time.deltaTime;

    }
    private Vector2 GetInput(){
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void FixedUpdate(){
        if(canMove)
        {
            Move();
        }
        CheckCollisions();
        if(onGround){
            ApplyGroundLinearDrag();
            hangTimeCounter = hangTime;
        }
        else{
            ApplyAirLinearDrag();
            FallMultiplier();
            hangTimeCounter -= Time.fixedDeltaTime;
        }
        if(canJump && canMove) Jump();
        if(!onGround && !Input.GetMouseButtonDown(0)){
            GetAirSprite();
        }
        else{
            anim.speed = 1f;
        }
    }
    private void Move(){
        if(onGround){
            rb.AddForce(new Vector2(horizontalMovement, 0f) * movementAcceleration);
            if(Mathf.Abs(rb.velocity.x) > moveSpeed){
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * moveSpeed, rb.velocity.y);
        }
            }
        else{
            rb.AddForce(new Vector2(horizontalMovement, 0f) * airAcceleration);
                if(Mathf.Abs(rb.velocity.x) > airSpeed){
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * airSpeed, rb.velocity.y);
            }
        }
        
    }
    private void ApplyGroundLinearDrag(){
        if(Mathf.Abs(horizontalMovement) < 0.4f && !isAttacking || !canMove){
            rb.drag = groundLinearDrag;
        }
        else if (isAttacking){
            rb.drag = groundLinearDrag * 4f;
        }
        else if(!canMove){
            rb.drag = staggerLinearDrag;
        }
        else{
            rb.drag = 0f;
        }
    }
    private void ApplyAirLinearDrag(){
        if(canMove){
            rb.drag = airLinearDrag;
        }
        else{
            rb.drag = staggerLinearDrag;
        }
    }
    private void Jump(){
        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        hangTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }

    void GetAirSprite(){

        int airIndex = (int)Mathf.Clamp(
            Helpers.Map(rb.velocity.y, -jumpForce, jumpForce, 0, jumpSprites.Count),
            0,
            jumpSprites.Count - 1
        );
        if(!Input.GetMouseButton(0) && !isAttacking){
            anim.speed = 0f;
            anim.Play("Player_Jump", 0, (float)airIndex / jumpSprites.Count);
        }
        else{
            anim.speed = 1f;
        }
    }

    private void FallMultiplier(){
        if(rb.velocity.y < 1){
            rb.gravityScale = fallMultiplier;

        }
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump")){
            rb.gravityScale = lowJumpMultiplier;
        }
        else{
            rb.gravityScale = 1f;
        }
    }
    private void CheckCollisions(){
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundRayCastLength, whatIsGround);
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRayCastLength);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);

            Destroy(other.gameObject);
        }
    }
    public void Combos(){
        if(combo > 4){
            combo = 0;
        }

        float currentStateLength = anim.GetCurrentAnimatorStateInfo(0).length;
        if(currentStateLength - timeBTWNAttacks <= 0f){
            combo = 0;
        }
        if(attackCooldown <= 0 && canMove){
            if(Input.GetMouseButtonDown(0) || attackBuffer){
                if(combo > 0 && combo < 3){
                    if(currentStateLength - timeBTWNAttacks >= 0f){
                        anim.SetTrigger("Attack" + combo);
                        attackCooldown = attackCooldownStart;
                        attackBuffer = false;
                        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                        for (int i = 0; i < enemiesToDamage.Length; i++)
                        {
                            enemiesToDamage[i].GetComponent<Boss_1>().TakeDamage(damage, 0.1f);
                            CameraShake.Instance.Shake(.3f, 0.7f);
                        }
                    }
                }
                else if(combo == 3){
                    anim.SetTrigger("Attack" + combo);
                    attackCooldown = attackCooldownStart;
                    attackBuffer = false;
                    StartCoroutine("DealDamage");
                }
                else if(combo < 4){
                    anim.SetTrigger("Attack" + combo);
                    attackCooldown = attackCooldownStart;
                    attackBuffer = false;
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                    for (int i = 0; i < enemiesToDamage.Length; i++)
                    {
                        enemiesToDamage[i].GetComponent<Boss_1>().TakeDamage(Mathf.FloorToInt(damage * 1.5f), 0.1f);
                        CameraShake.Instance.Shake(.3f, 0.7f);
                    }
                }
            }
        }
        if(attackCooldown <= attackCooldownStart * -2f){
                combo = 0;
            }
    }
    public IEnumerator DealDamage(){
        yield return new WaitForSeconds(0.24999999f);
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Boss_1>().TakeDamage(damage, 0.1f);
            CameraShake.Instance.Shake(.3f, 0.7f);
        }
    }
    public void StartCombo(){
        if(combo < 4){
            combo += 1;
            timeBTWNAttacks = 0;
            isAttacking = true;
        }
    }
    public void FinishCombo(){
        isAttacking = false;
    }

    public void AttackBuffer(){
        if(Input.GetMouseButtonDown(0) && isAttacking){
            attackBuffer = true;
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Enemy"){
            health -= 1;
            if(health > 0){
                OnPlayerDamaged?.Invoke();

                Vector3 direction = (transform.position - other.transform.position).normalized;
                CameraShake.Instance.Shake(.4f, 1.5f);
                StartCoroutine(DisablePlayerMovement(staggerLength));
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
            else{
                OnPlayerDamaged?.Invoke();
                OnPlayerDeath?.Invoke();
                StartCoroutine(DisablePlayerMovement(10));
                CameraShake.Instance.Shake(.4f, 1.5f);
                fallMultiplier = 0;
                lowJumpMultiplier = 0;
                rb.gravityScale = 0;
                this.GetComponent<BoxCollider2D>().enabled = false;

            }
           
        }
    }
    public IEnumerator DisablePlayerMovement(float time){
        canMove = false;

        freezer.Freeze();

        yield return new WaitForSeconds(time);

        canMove = true;
    }

    private void OnApplicationQuit(){
        inventory.Container.Items = new InventorySlot[3];
    }
}
