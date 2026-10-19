using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;

    private float damage;
    private int ownerLayer;
    private Vector3 direction;

    public void Init(float dmg, int layer, Vector3 dir)
    {
        damage = dmg;
        ownerLayer = layer;
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }
    // Update is called once per frame
    void Update()
    {
       transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == ownerLayer) return;
        
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
