using UnityEngine;
using System.Collections;
public class Sword : Enemy
{
    public float chargeSpeed = 7f;
    public float attackRange = 1f;
    private Animator animator;
    private Rigidbody2D rb;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private SpriteRenderer spriteRenderer;
    public GameObject attackHitboxPrefab;
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
    protected override void Update()
    {
        base.Update();
        attackTimer -= Time.deltaTime;
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        animator.SetTrigger("hit");
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
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange) return;
        Debug.Log("Sword enemy swings sword!");
        animator.SetTrigger("attack");
        StartCoroutine(TriggerHitbox(damage, 4));
    }
    IEnumerator TriggerHitbox(float dmg, int count)
    {
        float spacing = 0.5f;
        int direction = spriteRenderer.flipX ? -1 : 1;

        for (int i = 0; i < count; i++)
        {
            GameObject hb = Instantiate(attackHitboxPrefab, transform.position, Quaternion.identity);
            hb.transform.localScale = new Vector3(direction, 1, 1);

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
        Destroy(gameObject, 6);
    }
}
