using UnityEngine;

public class Sword : Enemy
{
    public float chargeSpeed = 7f;

    protected override void MoveAI(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            Move(dir.normalized);
        }
    }

    public override void Attack()
    {
        Debug.Log("Sword enemy swings sword!");
    }
}
