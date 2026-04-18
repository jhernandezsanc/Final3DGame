using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 20;

    private Transform target;

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }

    private void Start() {
        Destroy(gameObject, lifeTime);
    }

    private void Update() {
        if (target == null) {
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

