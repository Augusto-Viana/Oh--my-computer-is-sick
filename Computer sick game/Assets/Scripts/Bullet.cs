using UnityEngine;

public class Bullet : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attribute")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float timeSinceSpawn = 0f;

    [Header("Guidance")]
    [SerializeField] private bool guided = false;
    [SerializeField] private float turnSpeed = 360f;

    private Vector2 moveDirection;
    private Transform target;

    private void Update() {
        EliminateBullet();

        if (guided && target != null) {
            GuideTowardsTarget();
        }
    }

    public void SetDamage(int newDamage) {
        bulletDamage = newDamage;
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;

        if (target == null)
            return;

        moveDirection = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(
            moveDirection.y,
            moveDirection.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);
    }

    private void GuideTowardsTarget() {
        Vector2 directionToTarget = (
            target.position - transform.position
        ).normalized;

        moveDirection = Vector2.Lerp(
            moveDirection,
            directionToTarget,
            turnSpeed * Time.deltaTime
        ).normalized;

        float angle = Mathf.Atan2(
            moveDirection.y,
            moveDirection.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            angle - 180f
        );
    }

    private void EliminateBullet() {
        timeSinceSpawn += Time.deltaTime;

        if (timeSinceSpawn > 5f) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        rb.linearVelocity = moveDirection * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Health_LB1 enemyHealth = collision.gameObject.GetComponent<Health_LB1>();

        if (enemyHealth != null) {
            enemyHealth.takeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
}