using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Boss_1 : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    private float timeBTWNStates;
    public Animator anim;
    private float attack1Distance;
    private int attack1Times;
    private bool onPath = true;
    public GameObject disappearEffect;

    public Transform player;
    public PlayerController playerController;
    Vector2 playerPos;
    public int health;
    public GameObject damageEffect;
    public GameObject deathEffect;
    public SpriteRenderer spriteRenderer;
    public float angle;

    public Crossfade transition;
    public WinTextFade textFade;

    void Awake() {
        transform.position = player.transform.position + new Vector3(1, 1, 0);
        Instantiate(disappearEffect, transform.position, Quaternion.identity);
        onPath = false;
        CameraShake.Instance.Shake(5f, 2f);
        StartCoroutine(playerController.DisablePlayerMovement(5f));

        if (pathCreator != null && onPath)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;

        }
        timeBTWNStates = 5f;
        attack1Distance = Random.Range(-1, 2);
        attack1Times = Random.Range(1,4);
        StartCoroutine("Attack1");
        Debug.Log(attack1Times);
    }

    void Update()
    {
        if(attack1Distance == 0){
            attack1Distance = Random.Range(-1, 2);
        }
        if (pathCreator != null && onPath)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            //transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
        playerPos = player.position;
        Vector2 _position = transform.position;
        Vector2 direction = playerPos - _position;  
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged() {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
    IEnumerator Attack1(){
        yield return new WaitForSeconds(timeBTWNStates);
        if(health > 0){
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;
            Instantiate(disappearEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1);
        if(health > 0){
            for (int i = attack1Times; i > 0; i--)
            {
                onPath = false;
                transform.position = new Vector2(player.transform.position.x + attack1Distance, player.transform.position.y);
                if(health > 0){
                    this.GetComponent<SpriteRenderer>().enabled = true;
                    this.GetComponent<BoxCollider2D>().enabled = true;
                }

                Instantiate(disappearEffect, transform.position, Quaternion.identity);
                anim.SetTrigger("Attack1");
                attack1Distance = Random.Range(-1, 2);
                yield return new WaitForSeconds(1);
            }
        }
        yield return new WaitForSeconds(attack1Times);
        attack1Times = Random.Range(1,4);
        if(health > 0){
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;
            Instantiate(disappearEffect, transform.position, Quaternion.identity);
        }

        if(health > 0){
            onPath = true;
        }

        yield return new WaitForSeconds(1);
        if(health > 0){
            this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<BoxCollider2D>().enabled = true;
        }
        Instantiate(disappearEffect, transform.position, Quaternion.identity);
        timeBTWNStates = Random.Range(2,6);
        StartCoroutine("Attack1");
    }
    public void TakeDamage(int damage, float flashTime){
        health -= damage;
        if(health > 0){
            Instantiate(damageEffect, transform.position, Quaternion.Euler(0, 0, angle));
            IEnumerator TakeDamage_Cor(){
                spriteRenderer.material.SetInt("Hit", 1);
                yield return new WaitForSeconds(flashTime);
                spriteRenderer.material.SetInt("Hit", 0);
            }
            StartCoroutine(TakeDamage_Cor());
        }
        else{
            StartCoroutine(OnDeath(5, 0.01f));
            playerController.canMove = false;
        }
        
    }
    IEnumerator OnDeath(float deathTime, float effectDelay){
        effectDelay = Mathf.Clamp(effectDelay, 0.1f, 1f);
        deathTime = deathTime / effectDelay;
        Debug.Log(deathTime);
        onPath = false;
        yield return new WaitForSeconds(1);
        if(deathTime > 0f){
            for(int i = 0; i < deathTime; i++){
                yield return new WaitForSeconds(effectDelay);
                Instantiate(disappearEffect, transform.position, Quaternion.Euler(0, 0, angle));
                CameraShake.Instance.Shake(effectDelay, 2f);
            }
        }
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        textFade.FadeIn();
        yield return new WaitForSeconds(4);
        transition.LoadNextLevel();
        Destroy(this.gameObject);


    }

}
