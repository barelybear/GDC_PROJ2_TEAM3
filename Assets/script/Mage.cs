using UnityEngine;

public class Mage : Enemy
{
    public float fleeDistance = 2f;
    public float jumpForce = 5f;
    public float castCooldown = 2f;
    public LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float castTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        base.Update();
        castTimer -= Time.deltaTime;
    }

    protected override void MoveAI(float distanceToPlayer)
    {
        if (distanceToPlayer < fleeDistance)
        {
            Vector2 away = (transform.position - player.position).normalized;

            // If grounded and close, jump away
            if (isGrounded)
            {
                Vector2 jumpDir = new Vector2(away.x, 1).normalized;
                rb.velocity = Vector2.zero;
                rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                // If in air or can't jump, just flee horizontally
                rb.velocity = new Vector2(away.x * speed, rb.velocity.y);
            }

            Flip(away.x);

            // Try to attack
            if (castTimer <= 0f)
            {
                Attack();
                castTimer = castCooldown;
            }
        }
    }

    public override void Attack()
    {
        Debug.Log("Mage casts fireball!");
        // TODO: Instantiate fireball prefab, add logic here
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

}
