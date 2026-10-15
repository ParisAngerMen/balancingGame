using UnityEngine;

public interface IDamageable
{
    float maxHealth { get; set; }
    float currentHealth { get; set; }
    void TakeDamage(float damage);
    void Die();
}
