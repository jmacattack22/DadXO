using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {

    protected float fallMultiplier = 2.5f;
    protected float lowJumpMultiplier = 2f;
	protected float setAttackCooldown = 1f;
	protected bool facingRight = true;
    protected bool jump = false;
	protected bool justJumped = false;
	protected bool quickAttack = false;
    protected bool mediumAttack = false;
    protected bool heavyAttack = false;
    protected bool slideAttack = false;
    protected bool crouch = false;

    [SerializeField]
    private Sprite dashFirstFrame;
    [SerializeField]
    private Sprite dashJumpFirstFrame;

    public float iframe = 0f;
    public float dashTimer = 0f;
    public float attackCooldown = 0f;

    public bool alive;

    protected Stats stats;
    public Rigidbody2D rb;
    private LayerMask groundLayer; 
    private BoxCollider2D hurtBox;
    private BoxCollider2D hitBox;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hurtBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        hitBox = transform.GetChild(1).GetComponent<BoxCollider2D>();
        groundLayer = LayerMask.GetMask("Ground");
        hitBox.enabled = false;        
        alive = true;
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        StartCoroutine(APRegen());
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
        float distance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

	protected void Timers()
	{
		dashTimer -= Time.deltaTime;
		attackCooldown -= Time.deltaTime;
		iframe -= Time.deltaTime;
	}

	public bool CheckIFrame()
    {
        if (iframe > 0)
        {
            return true;
        }
        return false;
    }

	protected bool AttackCheck(int apCost)
    {
        if (attackCooldown < 0 && stats.ap >= apCost)
        {
            return true;
        }
        return false;
    }
   
    protected float SpeedCheck()
    {
        if (dashTimer > 0f)
        {
            return stats.maxSpeed * 10f;
        }
        if (!IsGrounded())
        {
            return stats.maxSpeed * 2f;
        }
        return stats.maxSpeed;
    }

    protected void attackInputCheck()
    {
        if (QuickAttack)
        {
            Anim.SetTrigger("Quick Attack");
            QuickAttack = false;
			stats.ap -= 5;
        }
        else if (MediumAttack)
        {
            Anim.SetTrigger("Medium Attack");
            MediumAttack = false;
			stats.ap -= 5;
		}
        else if (HeavyAttack)
        {
            Anim.SetTrigger("Heavy Attack");
            HeavyAttack = false;
			stats.ap -= 5;
		}
        else if (SlideAttack)
        {
            Anim.SetTrigger("Slide Attack");

            StartCoroutine(Dash());
            if (facingRight)
            {
                rb.AddForce(Vector2.right * 300);
            }
            else
            {
                rb.AddForce(Vector2.left * 300);
            }
            SlideAttack = false;
			stats.ap -= 5;
		}
    }

	protected IEnumerator APRegen()
	{
        while (true)
        {
            if (stats.ap < 100)
            {
                stats.ap += 1;
            }
            yield return new WaitForSeconds(2);            
        }

	}

	public IEnumerator DamageFlash()
	{
		Color defColor = GetComponent<SpriteRenderer>().color;
		while (CheckIFrame())
		{
			GetComponent<SpriteRenderer>().color = Color.white;
			yield return new WaitForSeconds(0.1f);
			GetComponent<SpriteRenderer>().color = defColor;
			yield return new WaitForSeconds(0.1f);
		}
		GetComponent<SpriteRenderer>().color = defColor;
		yield return null;
	}

	IEnumerator Dash()
    {
        var go = new GameObject();
        var sr = go.AddComponent<SpriteRenderer>();
        if (IsGrounded())
        {
            sr.sprite = dashFirstFrame;
        }
        else
        {
            sr.sprite = dashJumpFirstFrame;
        }

        go.transform.position = transform.position;
        go.transform.localScale = transform.localScale;

        float aValue = 0;
        float aTime = 0.4f;
        float alpha = sr.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            sr.color = newColor;
            yield return null;
        }

        GameObject.Destroy(go);
    }

    //Getters and Setters
    public bool Jump
    {
        get { return jump; }
        set { jump = value; }
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
