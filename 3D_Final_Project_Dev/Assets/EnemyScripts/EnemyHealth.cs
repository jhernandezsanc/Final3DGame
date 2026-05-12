using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 3;
    public float explosionForce = 12f;

    private Renderer enemyRenderer;
    private Color originalColor;

    [Header("Damage Flash Settings")]
    public Light damageLight;
    public float flashIntesnsity = 8f;
    public float flashDuration = 0.15f;

    [Header("Audio Settings")]
    public AudioClip hitMarkerSound;


    /*
    private void Start()
    {
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }
    */
    // When the enemy collides with the player's attack range, it checks the type of attack and either takes damage or explodes.
    private void OnTriggerEnter(Collider other)
    {
        PlayerAttackRange range = other.GetComponent<PlayerAttackRange>();

        if (range == null) return;

        if (range.rangeType == PlayerAttackRange.AttackRangeType.ShortRange)
        {
            Explode();
        }
        else if (range.rangeType == PlayerAttackRange.AttackRangeType.LongRange)
        {
            TakeDamage(1);
        }
    }
    // The TakeDamage method reduces the enemy's health and flashes it red when hit. If health drops to zero or below, the enemy is destroyed.
    private void TakeDamage(int damage)
    {
        health -= damage;

        if (hitMarkerSound != null)
        {
            AudioSource.PlayClipAtPoint(hitMarkerSound, transform.position);
        }

        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    // The FlashRed coroutine changes the intensity of a light to indicate damage taken, then resets it after a short duration.
    private IEnumerator FlashRed()
    {
        if (damageLight != null)
        {
            damageLight.intensity = flashIntesnsity;
            yield return new WaitForSeconds(flashDuration);
            damageLight.intensity = 0f;
        }
    }
    /* The FlashRed coroutine changes the enemy's color to red for a brief moment to indicate it has taken damage, then reverts to the original color.
    private IEnumerator FlashRed()
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        enemyRenderer.material.color = originalColor;
    }
    */
    // The Explode method applies a force to the player in the direction away from the enemy and then destroys the enemy game object.
    private void Explode()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                Vector3 direction = player.transform.position - transform.position;
                direction.y = 0.6f;
                direction.Normalize();

                playerRb.AddForce(direction * explosionForce, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}