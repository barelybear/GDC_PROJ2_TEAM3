using UnityEngine;

public class Entity : MonoBehaviour
{
    public float speed = 5f;
    public float health = 100f;
    public float damage = 10f;
    public bool hitted = false;
    public virtual void Move(Vector2 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public virtual void TakeDamage(float amount)
    {
        hit();
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public void hit()
    {
        hitted = true;
        //Some logic
        hitted = false;
    }
}