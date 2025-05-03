using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Sword : Enemy
{
    public float chargeSpeed = 7f;
    public float attackRange = 1f;
    private Animator animator;
    private Rigidbody2D rb;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private object spriteRenderer;
    public GameObject attackHitboxPrefab;
    [SerializeField] private Slider HealthBar;
    private bool isAttack = false;
    
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
        attackTimer -= Time.deltaTime;
    }
    public override void TakeDamage(float damage)
    {
        if(isAttack)
        {
            StopAllCoroutines();
            isAttack = false;
        }
        base.TakeDamage(damage);
        animator.SetTrigger("hit");
    }
    void UpdateHealthUI(float currentHealth)
    {
        if (HealthBar != null)
            HealthBar.value = currentHealth;
    }
    protected override void MoveAI(float distanceToPlayer)
    {
        if (Mathf.Abs(distanceToPlayer) <= detectionRange)
        {
            animator.SetFloat("speed", 1);
            Vector2 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            Flip(dir.x);
            Move(dir.normalized);

            if (distanceToPlayer <= attackRange && attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
        else animator.SetFloat("speed", 0);
    }


    public override void Attack()
    {
        isAttack = true;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange) return;
        Debug.Log("Sword enemy swings sword!");
        animator.SetTrigger("attack");
        StartCoroutine(TriggerHitbox(damage, 4));
        isAttack = false;
    }
    IEnumerator TriggerHitbox(float dmg, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject hb = Instantiate(attackHitboxPrefab, transform.position, Quaternion.identity);

            hitBox script = hb.GetComponent<hitBox>();
            script.Init(dmg, tag);
            yield return new WaitForSeconds(0.2f);
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
    protected override void Die()
    {
        Destroy(gameObject);
    }
}
