using UnityEngine;

public class Player : Entity
{
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded = true;

    public float verticalSpeed => rb.velocity.y;
    private SpriteRenderer spriteRenderer;
    private int spacePressCount = 0;
    private float comboTimer = 0f;
    public float comboWindow = 0.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -10f;
    public GameObject attackHitbox;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // -1 (A), 0, or 1 (D)
        Vector2 direction = new Vector2(moveInput, 0);
        Move(direction);
        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressCount++;
            comboTimer = comboWindow;

            if (spacePressCount == 2)
            {
                ComboAttack();
                ResetCombo();
            }
            else
            {
                lastAttackTime = Time.time;
                NormalAttack();
            }
        }


        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    void ResetCombo()
    {
        spacePressCount = 0;
        comboTimer = 0f;
    }

    void NormalAttack()
    {
        Debug.Log("Normal attack!");
        TriggerHitbox(damage);
    }

    void ComboAttack()
    {
        Debug.Log("COMBO attack!");
        TriggerHitbox(damage * 2); // Example: more damage
        lastAttackTime = Time.time;
    }

    void TriggerHitbox(float dmg)
    {
        if (attackHitbox == null) return;

        playerAttaclBox hitboxScript = attackHitbox.GetComponent<playerAttaclBox>();
        hitboxScript.damage = dmg;
        attackHitbox.SetActive(true);
        Invoke(nameof(DisableHitbox), 0.2f); // Hitbox active for 0.2 seconds
    }

    void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }

    public override void Move(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }
}
