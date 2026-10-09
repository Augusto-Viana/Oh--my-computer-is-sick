using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FIREWALL : MonoBehaviour {
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject areaObject;

    [Header("Attributes")]
    [SerializeField] private float damageRange = 4f;
    [SerializeField] private float damagePerSecond = 2f;
    [SerializeField] private float slowAmount = 0.25f;
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float lingerTimeNoEnemies = 0.1f;

    private bool isOn = false;
    private bool animationPlayed = false;
    private float checkTimer = 0f;
    private float noEnemiesTimer = 0f;

    private Dictionary<Health_LB1, float> enemyDamageBuffer =
        new Dictionary<Health_LB1, float>();

    // Inimigos que estão atualmente dentro da área desta Firewall
    private HashSet<Enemy_Movement> enemiesAffectedBySlow =
        new HashSet<Enemy_Movement>();

    private void Update() {
        checkTimer += Time.deltaTime;

        if (checkTimer >= checkInterval) {
            checkTimer = 0f;
            HandleEnemiesInRange();
        }
    }

    private void HandleEnemiesInRange() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            damageRange,
            Vector2.zero,
            0f,
            enemyMask
        );

        bool hasEnemies = hits.Length > 0;

        if (hasEnemies) {
            noEnemiesTimer = 0f;

            if (!isOn)
                TurnOn();

            ApplyEffectsToEnemies(hits);
        } else {
            noEnemiesTimer += checkInterval;

            if (isOn && noEnemiesTimer >= lingerTimeNoEnemies)
                TurnOff();

            RemoveAllSlows();
        }
    }

    private void TurnOn() {
        isOn = true;

        if (!animationPlayed && anim != null) {
            anim.SetTrigger("Animate");
            animationPlayed = true;
        }

        if (areaObject != null)
            areaObject.SetActive(true);
    }

    private void TurnOff() {
        isOn = false;

        if (anim != null)
            anim.SetTrigger("Animate");

        animationPlayed = false;

        if (areaObject != null)
            areaObject.SetActive(false);
    }

    private void ApplyEffectsToEnemies(RaycastHit2D[] hits) {
        HashSet<Enemy_Movement> enemiesCurrentlyInRange =
            new HashSet<Enemy_Movement>();

        foreach (RaycastHit2D hit in hits) {
            Health_LB1 enemyHealth = hit.collider.GetComponent<Health_LB1>();
            Enemy_Movement enemyMovement = hit.collider.GetComponent<Enemy_Movement>();

            if (enemyHealth == null || enemyMovement == null)
                continue;

            enemiesCurrentlyInRange.Add(enemyMovement);

            // Aplica o slow desta Firewall
            enemyMovement.ApplySlow(this, slowAmount);

            // -------------------------
            // DANO
            // -------------------------

            if (!enemyDamageBuffer.ContainsKey(enemyHealth))
                enemyDamageBuffer[enemyHealth] = 0f;

            enemyDamageBuffer[enemyHealth] += damagePerSecond * checkInterval;

            if (enemyDamageBuffer[enemyHealth] >= 1f) {
                int dmgToApply = Mathf.FloorToInt(enemyDamageBuffer[enemyHealth]);

                enemyDamageBuffer[enemyHealth] -= dmgToApply;

                enemyHealth.takeDamage(dmgToApply);
            }
        }

        // Descobre quem estava dentro antes,
        // mas agora não está mais.
        foreach (Enemy_Movement enemy in enemiesAffectedBySlow) {
            if (!enemiesCurrentlyInRange.Contains(enemy)) {
                if (enemy != null)
                    enemy.RemoveSlow(this);
            }
        }

        enemiesAffectedBySlow = enemiesCurrentlyInRange;
    }

    private void RemoveAllSlows() {
        foreach (Enemy_Movement enemy in enemiesAffectedBySlow) {
            if (enemy != null)
                enemy.RemoveSlow(this);
        }

        enemiesAffectedBySlow.Clear();
    }

    private void OnDestroy() {
        RemoveAllSlows();
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(
            transform.position,
            transform.forward,
            damageRange
        );
    }
}