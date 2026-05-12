using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Explosion")]
    public float lifeTime = 5f;
    public float explosionRadius = 4f;
    public float knockbackForce = 12f;
    public AudioClip explosionSound;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Play explosion sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Check everything nearby
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            PlayerMovement player = hit.GetComponent<PlayerMovement>();

            if (player != null)
            {
                Vector3 pushDirection = player.transform.position - transform.position;
                pushDirection.y = 0.5f;

                player.ApplyKnockback(pushDirection, knockbackForce);
            }
        }

        Destroy(gameObject);
    }
}