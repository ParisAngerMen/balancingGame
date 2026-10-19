using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    

    public void Die()
    {
        Destroy(gameObject);

        if (gameObject.layer == 6)
        {
            SceneManager.LoadScene(SceneManager.GetSceneByName("BlueVictory").buildIndex);
        }

        else
        {
            SceneManager.LoadScene(SceneManager.GetSceneByName("RedVictory").buildIndex);
        }
    }


}
