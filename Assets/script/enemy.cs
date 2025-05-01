using UnityEngine;

public class Enemy : Entity
{
    public Transform player;
    public float detectionRange = 5f;
    public float castCooldown = 2f;

    protected float lastCastTime = -Mathf.Infinity;
    protected virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (Time.time >= lastCastTime + castCooldown)
        {
            Cast(distanceToPlayer);
        }

        MoveAI(distanceToPlayer);
    }

    protected virtual void Cast(float distanceToPlayer)
    {
        lastCastTime = Time.time;
        Attack();
    }

    protected virtual void MoveAI(float distanceToPlayer) { }

    public virtual void Attack()
    {
        Debug.Log($"{gameObject.name} attacks!");
    }
}
