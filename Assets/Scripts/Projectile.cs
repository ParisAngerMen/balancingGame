using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifetime = 5f;

    private static readonly RaycastHit[] sweepHits = new RaycastHit[16];

    private float damage;
    private int ownerLayer;
    private Vector3 direction;
    private bool hasHit;

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
        if (hasHit || direction == Vector3.zero)
        {
            return;
        }

        float step = speed * Time.deltaTime;

        // Fast projectiles can jump over thin colliders between two frames,
        // so check the path travelled this frame before moving
        if (SweepForHit(step))
        {
            return;
        }

        transform.position += direction * step;
    }

    private bool SweepForHit(float distance)
    {
        int count = Physics.RaycastNonAlloc(transform.position, direction, sweepHits, distance,
            Physics.AllLayers, QueryTriggerInteraction.Collide);

        // Results are not ordered, so try the closest valid one first
        while (count > 0)
        {
            int closest = 0;
            for (int i = 1; i < count; i++)
            {
                if (sweepHits[i].distance < sweepHits[closest].distance)
                {
                    closest = i;
                }
            }

            if (TryHit(sweepHits[closest].collider))
            {
                return true;
            }

            sweepHits[closest] = sweepHits[count - 1];
            count--;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private bool TryHit(Collider other)
    {
        // Destroy() is delayed until the end of the frame: never damage twice
        if (hasHit || other == null || other.gameObject.layer == ownerLayer)
        {
            return false;
        }

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return false;
        }

        hasHit = true;
        target.TakeDamage(damage);
        Destroy(gameObject);
        return true;
    }
}
