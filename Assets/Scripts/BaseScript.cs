using UnityEngine;

public class BaseScript : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void IDamageable.Die()
    {
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
