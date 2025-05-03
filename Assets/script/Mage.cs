using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Mage : Enemy
{
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float castTimer = 0f;
    private Animator animator;
    public float offsetMin = -2f;
    public float offsetMax = 2f;
    public GameObject minions;
    [SerializeField] private Slider HealthBar;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Khởi tạo thanh máu
        if (HealthBar != null)
        {
            HealthBar.maxValue = health;
            HealthBar.value    = health;
        }
        // Subscribe để cập nhật UI khi nhận damage
        onHealthChanged += UpdateHealthUI;
    }
    void OnDestroy()
    {
        // Unsubscribe event
        onHealthChanged -= UpdateHealthUI;
    }

    protected override void Update()
    {
        base.Update();
        castTimer -= Time.deltaTime;
    }

    protected override void MoveAI(float distanceToPlayer)
    {
        if (distanceToPlayer < detectionRange)
        {
            animator.SetFloat("speed", 1);
            Vector2 away = (transform.position - player.position).normalized;
            if (isGrounded)
            {
                Vector2 jumpDir = new Vector2(away.x, 1).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                rb.linearVelocity = new Vector2(away.x * speed, rb.linearVelocity.y);
            }
            Flip(away.x);
            if (castTimer <= 0f)
            {
                Attack();
                castTimer = castCooldown;
            }
        }
        else animator.SetFloat("speed", 0);
    }

    public override void Attack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return;
        animator.SetTrigger("cast");
        int numberOfObjects = Random.Range(1, 5);

        for (int i = 0; i < numberOfObjects; i++) { 
            float randomOffset = Random.Range(offsetMin, offsetMax);
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset, 10, 0);
            Instantiate(minions, spawnPosition, Quaternion.identity);
        }
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

    private void Flip(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction < 0 ? -1 : 1);
        transform.localScale = scale;
    }
    protected override void UpdateAnimationParameters()
    {
        animator.SetFloat("health", health);

    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        animator.SetTrigger("hit");
    }

    void UpdateHealthUI(float currentHealth)
    {
        if (HealthBar != null)
            HealthBar.value = currentHealth;
    }
}
