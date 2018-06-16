using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{

    private string[] attackAnimations = { "PixelCharAnim_Sword_quickAtk", "PixelCharAnim_Sword_mediumAtk", "PixelCharAnim_Sword_heavyAtk", "PixelCharAnim_Sword_slideAtk" };

    void Update()
    {
        if (JumpCheck())
        {
            Jump = true;
        }
        else if (Input.GetButtonDown("QuickAttack") && !AttackCheck() && IsGrounded())
        {
            QuickAttack = true;
            attacking = true;
        }
        else if (Input.GetButtonDown("MediumAttack") && !AttackCheck() && IsGrounded())
        {
            MediumAttack = true;
            attacking = true;
        }
        else if (Input.GetButtonDown("HeavyAttack") && !AttackCheck() && IsGrounded())
        {
            HeavyAttack = true;
            attacking = true;
        }
        else if (Input.GetButtonDown("SlideAttack") && !AttackCheck() && IsGrounded())
        {
            SlideAttack = true;
            attacking = true;
        }
        else if (Input.GetButtonDown("Crouch") && !AttackCheck() && IsGrounded())
        {
            Crouch = !Crouch;
            Anim.SetBool("Crouch", Crouch);
        }
    }
   
    void FixedUpdate()
    {
        float horizMove = Input.GetAxis("Horizontal");
        Anim.SetFloat("Speed", Mathf.Abs(horizMove));

        if (Alive)
        {

            if (horizMove * GetComponent<Rigidbody2D>().velocity.x < speedCheck())
            {
                rb.AddForce(Vector2.right * horizMove * MoveForce);
            }
            if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > speedCheck())
            {
                rb.velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * speedCheck(), GetComponent<Rigidbody2D>().velocity.y);
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
        if (Jump)
        {
            Anim.SetTrigger("Jump");
            rb.AddForce(new Vector2(0f, JumpForce));
            Jump = false;
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (QuickAttack)
        {
            Anim.SetTrigger("Quick Attack");
            QuickAttack = false;
            attacking = false;
        }
        if (MediumAttack)
        {
            Anim.SetTrigger("Medium Attack");
            MediumAttack = false;
            attacking = false;
        }
        if (HeavyAttack)
        {
            Anim.SetTrigger("Heavy Attack");
            HeavyAttack = false;
            attacking = false;
        }
        if (SlideAttack)
        {
            Anim.SetTrigger("Slide Attack");
            if (facingRight)
            {
                rb.AddForce(Vector2.right * 600);
            }
            else
            {
                rb.AddForce(Vector2.left * 600);
            }
            SlideAttack = false;
            attacking = false;
        }
    }

    private bool JumpCheck()
    {
        return Input.GetButtonDown("Jump") && IsGrounded();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") && HitBox.isActiveAndEnabled)
        {
            if (col.gameObject.GetComponent<EnemyController>().CheckIFrame())
            {
                return;
            }
            col.gameObject.GetComponent<EnemyController>().Damage(2);
            Debug.Log("Hit");
            if (facingRight)
            {
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * AttackForce);
            }
            else
            {
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * AttackForce);
            }        
        }
    }

    private bool AttackCheck()
    {
        if (attacking)
        {
            return true;
        }
        foreach (string attackAnimation in attackAnimations)
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName(attackAnimation))
            {
                return true;
            }
        }
        return false;
    }
}       