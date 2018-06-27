using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    private float setAttackCooldown = 1f;
    private bool justJumped;

    void Update()
    { 
        if(stats.currentHealth <= 0)
        {
            Debug.Log("Toodles!");
            Destroy(gameObject);
        }
        if (JumpCheck())
        {
            Jump = true;
            dashTimer = 0f;
        }
        else if (Input.GetButtonDown("QuickAttack") && AttackCheck() && IsGrounded())
        {
            QuickAttack = true;
            attackCooldown = setAttackCooldown;
        }
        else if (Input.GetButtonDown("MediumAttack") && AttackCheck() && IsGrounded())
        {
            MediumAttack = true;
            attackCooldown = setAttackCooldown;
        }
        else if (Input.GetButtonDown("HeavyAttack") && AttackCheck() && IsGrounded())
        {
            HeavyAttack = true;
            attackCooldown = setAttackCooldown;
        }
        else if (Input.GetButtonDown("SlideAttack") && AttackCheck())
        {
            SlideAttack = true;
            attackCooldown = setAttackCooldown;
            dashTimer = 0.5f;
        }
        else if (Input.GetButtonDown("Crouch") && AttackCheck() && IsGrounded())
        {
            Crouch = !Crouch;
            Anim.SetBool("Crouch", Crouch);
        }
        dashTimer -= Time.deltaTime;
        attackCooldown -= Time.deltaTime;
        iframe -= Time.deltaTime;

    }
   
    void FixedUpdate()
    {
        movementInputCheck();

        jumpInputCheck();

        attackInputCheck();

        if (IsGrounded() && (rb.velocity.y == 0) && !justJumped)
        {
            Anim.SetBool("Grounded",true);
        }
        justJumped = false;
    }

    private void jumpInputCheck()
    {
        if (Jump)
        {
            Anim.SetBool("Grounded", false);
            rb.AddForce(new Vector2(0f, stats.jumpForce));
            Anim.SetTrigger("Jump");
            Jump = false;
            justJumped = true;
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void movementInputCheck()
    {
        float horizMove = Input.GetAxis("Horizontal");
        Anim.SetFloat("Speed", Mathf.Abs(horizMove));
        if (horizMove * GetComponent<Rigidbody2D>().velocity.x < SpeedCheck())
        {
            rb.AddForce(Vector2.right * horizMove * stats.moveForce);
        }
        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > SpeedCheck())
        {
            rb.velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * SpeedCheck(), GetComponent<Rigidbody2D>().velocity.y);
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

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") && HitBox.isActiveAndEnabled)
        {
            if (col.gameObject.GetComponentInParent<EnemyController>().CheckIFrame())
            {
                return;
            }
            col.gameObject.GetComponentInParent<FighterDamage>().Damage(5);
            col.gameObject.GetComponentInParent<EnemyController>().iframe = 1f;
            StartCoroutine(col.gameObject.GetComponentInParent<EnemyController>().DamageFlash());
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
}       