using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded = true;

    public float verticalSpeed => rb.linearVelocity.y;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private int spacePressCount = 0;
    private float comboTimer = 0f;
    public float comboWindow = 0.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -10f;
    [SerializeField] private GameObject attackHitboxPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        UpdateAnimationParameters();
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        animator.SetTrigger("attack");
        StartCoroutine(TriggerHitbox(damage, 1));
    }

    void ComboAttack()
    {
        animator.SetTrigger("attack2");
        StartCoroutine(TriggerHitbox(damage, 2));
        lastAttackTime = Time.time;
    }

    IEnumerator TriggerHitbox(float dmg, int count)
    {
        float spacing = 0.5f;
        int direction = spriteRenderer.flipX ? -1 : 1;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3((0.6f + i * spacing) * direction, 0, 0);
            Vector3 spawnPos = transform.position + offset;

            GameObject hb = Instantiate(attackHitboxPrefab, spawnPos, Quaternion.identity);
            hb.transform.localScale = new Vector3(direction, 1, 1);

            hitBox script = hb.GetComponent<hitBox>();
            script.Init(dmg, tag);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public override void Move(Vector2 direction)
    {
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
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

    void UpdateAnimationParameters()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("speedV", rb.linearVelocity.y);
        animator.SetBool("isGround", isGrounded);
        animator.SetFloat("health", health);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        animator.SetTrigger("hit");
    }
}
