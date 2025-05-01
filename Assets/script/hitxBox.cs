using UnityEngine;

public class hitBox : MonoBehaviour
{
    public float damage = 10f;
    private string ownerTag;

    public void Init(float damage, string ownerTag)
    {
        this.damage = damage;
        this.ownerTag = ownerTag;
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ownerTag)) return;

        Entity target = other.GetComponent<Entity>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
