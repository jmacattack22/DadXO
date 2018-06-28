using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    public enum State
    {
        Aggressive,Neutral,Cautious
    }
    private float randTimer;
    private int horizMove;
    Vector3 startingPos;
    protected State state;
    private float randJumpTimer;
    public GameObject player;
    public Stats playerStats;
    public float apThreshold = 45f;

    
    System.Random rnd;
    int rndJump;

    //On start

    private void Start()
    {
        jump = false;
        rnd = new System.Random();
        horizMove = rnd.Next(-1, 2);
        rndJump = rnd.Next(-1, 2);
        randTimer = 3f;
        randJumpTimer = 2f;
        startingPos = transform.position;
        state = State.Neutral;
        playerStats = player.GetComponent<Stats>();
    }

    //Updates

    void Update()
    {
		Timers();
        if (GetComponent<Stats>().currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if(randTimer < 0)
        {
            horizMove = 0;
            horizMove = rnd.Next(-1, 2);
            randTimer = 3f;
        }
        if (randJumpTimer < 0)
        {
            rndJump = rnd.Next(-1, 2);
            if(rndJump == -1)
            {
                jump = true;
            }            
            randJumpTimer = 3f;
        }
        if (horizMove != 0)
        {
            //stats.ap = stats.ap - Time.deltaTime;
        }
        if (transform.position.x > startingPos.x + 3 || transform.position.x < startingPos.x - 9)
        {
            horizMove *= -1;
        }


        randJumpTimer -= Time.deltaTime;
        randTimer -= Time.deltaTime;
        iframe -= Time.deltaTime;
        DetermineState();
        print(state);
    }

    private void FixedUpdate()
    {
        MovementCheck();
    }

    //Action Checks

    private void MovementCheck()
    {
        Anim.SetFloat("Speed", Mathf.Abs(horizMove));
        if (horizMove * rb.velocity.x < SpeedCheck())
        {
            rb.AddForce(Vector2.right * horizMove * GetComponent<Stats>().moveForce);
        }
        if (Mathf.Abs(rb.velocity.x) > SpeedCheck())
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * SpeedCheck(), rb.velocity.y);
        }
        if (jump)
        {
            rb.AddForce(new Vector2(0f, GetComponent<Stats>().jumpForce * 3f));
            jump = false;
        }
        if (horizMove > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizMove < 0 && facingRight)
        {
            Flip();
        }
    }

    /*private void AttackInitiate()
    {
        if((state != State.Cautious) && AttackCheck())
		{
			return;
		}
    }*/

    private void ChooseAttack()
    {
        int rndAttack = rnd.Next(0, 4);
        switch (rndAttack)
        {
            case 0:
                quickAttack = true;
                break;
            case 1:
                mediumAttack = true;
                break;
            case 2:
                heavyAttack = true;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponentInParent<PlayerController>().CheckIFrame())
            {
                return;
            }
            col.gameObject.GetComponentInParent<FighterDamage>().Damage(5);
            if (facingRight)
            {
                col.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(Vector2.right * stats.attackForce);
            }
            else
            {
                col.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(Vector2.left * stats.attackForce);
            }
        }
    }

    //Determine State Functions

    protected void DetermineState()
    {
        float currentScore = DetermineAPScore() + DetermineHPDiffScore() + DetermineAPDiffScore();

        if (currentScore > 2f)
        {
            state = State.Aggressive;
        }
        else if(currentScore > 1f)
        {
            state = State.Neutral;
        }
        else
        {
            state = State.Cautious;
        }
    }

    protected float DetermineAPDiffScore()
    {
        float apDiffScore = 0.5f;
        float apThreshold = 45f;
        float apDiff = (float)stats.ap - (float)playerStats.ap;
        if (apDiff > 0f)
        {
           apDiffScore = ((apThreshold - apDiff)/apThreshold)* 0.5f + 0.5f;
           apDiffScore = apDiffScore > 1 ? 1 : apDiffScore;
        }
        else if (apDiff < 0f)
        {
            apDiffScore = ((apThreshold + (apDiff)) / apThreshold) * 0.5f;
            apDiffScore = apDiffScore < 0 ? 0 : apDiffScore;
        }
        return apDiffScore;
    }

    protected float DetermineHPDiffScore()
    {
        float hpDiffScore = 0.5f;
        float hpThreshold = 45f;
        float playerHealth = ((float)playerStats.currentHealth / (float)playerStats.maxHealth) * 100f;
        float Health = ((float)stats.currentHealth / (float)stats.maxHealth) * 100f;
        float hpDiff = Health - playerHealth;

        if (hpDiff > 0f)
        {
            hpDiffScore = ((hpThreshold - hpDiff) / hpThreshold) * 0.5f + 0.5f;
            hpDiffScore = hpDiffScore > 1 ? 1 : hpDiffScore;
        }
        else if (hpDiff < 0f)
        {
            hpDiffScore = ((hpThreshold + (hpDiff)) / hpThreshold) * 0.5f;
            hpDiffScore = hpDiffScore < 0 ? 0 : hpDiffScore;
        }
        return hpDiffScore;
    }

    protected float DetermineAPScore()
    {
        float apScore = (float)stats.ap / 100f;
        return apScore;
    }

}