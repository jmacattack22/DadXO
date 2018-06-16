using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {

    protected float fallMultiplier = 2.5f;
    protected float lowJumpMultiplier = 2f;
    protected bool facingRight = true;
    protected bool jump = false;
    protected bool quickAttack = false;
    protected bool mediumAttack = false;
    protected bool heavyAttack = false;
    protected bool slideAttack = false;
    protected bool attacking = false;
    protected bool crouch = false;
    public float health = 30;
    public bool alive;

    protected float moveForce = 200f;
    protected float maxSpeed = 1f;
    protected float jumpForce = 100f;
    protected float attackForce = 120f;
    public float iframe = 0f;
    public Rigidbody2D rb;

    private LayerMask groundLayer; 
    private BoxCollider2D hurtBox;
    private BoxCollider2D hitBox;

    private Boxer boxer;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        hurtBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        hitBox = transform.GetChild(1).GetComponent<BoxCollider2D>();
        groundLayer = LayerMask.GetMask("Ground");
        hitBox.enabled = false;
        setBoxer(new Boxer(new Vector2Int(0, 0), "", "", 0, 0, BoxerClass.Type.BushWacker, 30, 0, 30, 0, 30, 0, 30, 0, 30, 0));
        alive = true;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    public void setBoxer(Boxer boxer)
    {
        this.boxer = boxer;
        this.moveForce = boxer.Speed * 2;
        this.maxSpeed = 0.5f + ((float)(boxer.Speed / 999) * 0.7f);
        this.health = boxer.Health;
    }
    
    public float speedCheck()
    {
        if (!IsGrounded())
        {
            return maxSpeed * 2f;
        }
        return maxSpeed;
    }

    public bool CheckIFrame()
    {
        if (iframe > 0)
        {
            return true;
        }
        return false;
    }

    public void Damage(int damage)
    {
        Health -= damage;
        iframe = 1f;
    }

    //Getters
    public bool Jump
    {
        get { return jump; }
        set { jump = value; }
    }
    public float Health
    {
        get { return health; }
        set { health = value; }
    }
    public bool Alive
    {
        get { return alive; }
        set { alive = value; }
    }

    public bool QuickAttack
    {
        get { return quickAttack; }
        set { quickAttack = value; }
    }

    public bool MediumAttack
    {
        get { return mediumAttack; }
        set { mediumAttack = value; }
    }

    public bool HeavyAttack
    {
        get { return heavyAttack; }
        set { heavyAttack = value; }
    }

    public bool SlideAttack
    {
        get { return slideAttack; }
        set { slideAttack = value; }
    }

    public bool Crouch
    {
        get { return crouch; }
        set { crouch = value; }
    }
    public float MoveForce
    {
        get { return moveForce; }
        set { moveForce = value; }
    }
    public float MaxSpeed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }
    public float JumpForce
    {
        get { return jumpForce; }
        set { jumpForce = value; }
    }
    public float AttackForce
    {
        get { return attackForce; }
        set { attackForce = value; }
    }
    public Animator Anim
    {
        get { return anim; }
    }
    public BoxCollider2D HitBox
    {
        get { return hitBox; }
    }
    public BoxCollider2D HurtBox
    {
        get { return hurtBox; }
    }


}
