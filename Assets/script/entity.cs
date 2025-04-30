using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health;
    public float damage;
    public float speed;

    public enum Type
    {
        Sword,
        Mage,
        Morph,
        Player
    }
    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
    }
}
