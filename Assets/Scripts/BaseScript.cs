using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScript : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float maxHealth { get; set; }
    public float currentHealth { get; set; }

    private bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Several hits can land on the same frame: only end the game once
        if (isDead)
        {
            return;
        }

        isDead = true;
        Destroy(gameObject);

        if (gameObject.layer == LayerMask.NameToLayer("Red")) // the red base fell
        {
            SceneManager.LoadScene("BlueVictory");
        }

        else
        {
            SceneManager.LoadScene("RedVictory");
        }
    }
}
