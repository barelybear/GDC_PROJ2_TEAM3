using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

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
    [SerializeField] private Slider HealthBar;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField]private Button MenuButton;
    [SerializeField]private Button RestartButton;
    private bool isAttack = false;
    public float coyoteTime       = 0.1f;  // cho phép nhảy dù vừa rời đất
    public float jumpBufferTime   = 0.1f;  // lưu input nhảy trước khi chạm đất
    public float fallMultiplier   = 2.5f;  // tăng tốc rơi
    public float lowJumpMultiplier= 2f;    // nếu nhả phím sớm

    private float coyoteCounter;
    private float jumpBufferCounter;

    void Awake()
    {
        onHealthChanged += UpdateHealthUI;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        GameOverPanel.SetActive(false);
        HealthBar.maxValue = health;
        HealthBar.value = health;
        MenuButton.onClick.AddListener(ReturnMenu);
        RestartButton.onClick.AddListener(RestartGame);

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
        // if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        // {
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        // }
        // 1) Cập nhật Coyote Time
        if (isGrounded) coyoteCounter = coyoteTime;
        else            coyoteCounter -= Time.deltaTime;

        // 2) Cập nhật Jump Buffer
        if (Input.GetKeyDown(KeyCode.W))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // 3) Thực hiện nhảy khi cả 2 điều kiện đều thỏa
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteCounter     = 0f;
        }

        // 4) Variable Jump Height: rơi nhanh hơn hoặc nhảy thấp nếu nhả phím
        if (rb.linearVelocity.y < 0f)
        {
            // đang rơi
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.W))
        {
            // đang nhảy nhưng nhả W → rơi nhanh hơn
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
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
        isAttack = true;
        Debug.Log("Normal attack!");
        animator.SetTrigger("attack");
        StartCoroutine(TriggerHitbox(damage, 1));
        isAttack = false;
    }

    void ComboAttack()
    {
        isAttack = true;
        animator.SetTrigger("attack2");
        StartCoroutine(TriggerHitbox(damage, 2));
        lastAttackTime = Time.time;
        isAttack = false;
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
        if (isAttack)
        {
            StopAllCoroutines();
            isAttack = false;
        }
        base.TakeDamage(damage);
        animator.SetTrigger("hit");
    }

    protected override void Die()
    {
        // hiện panel Game Over
        GameOverPanel.SetActive(true);

        // khóa input và dừng game
        this.enabled     = false;
        Time.timeScale   = 0f;
    }

    void UpdateHealthUI(float newHealth)
    {
        HealthBar.value = newHealth;
    }

    void OnDestroy()
    {
        onHealthChanged -= UpdateHealthUI;
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1; // Resume the game
        GameOverPanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("Menu"); // Load the main menu scene
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        GameOverPanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("GameScene"); // Restart the game scene
    }
}
