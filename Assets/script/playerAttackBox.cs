using UnityEngine;

public class playerAttaclBox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null || other.gameObject.CompareTag("Player"))
            return;

        Entity target = other.GetComponent<Entity>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log($"Hit {target.name} for {damage} damage!");
        }
    }
}
