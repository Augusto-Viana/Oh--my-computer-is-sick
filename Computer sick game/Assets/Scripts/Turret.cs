using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private Animator anim;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 4f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float bps = 1f;
    [SerializeField] private int bulletDamage = 50;

    [Header("Attack Animation")]
    [SerializeField] private bool burstAnimation = false;
    [SerializeField] private int visualShotsPerCycle = 1;

    [Header("Burst Animation")]
    [SerializeField] private string burstAnimationState = "BYTE1";
    [SerializeField] private string burstAnimationClip = "BYTE1";

    [Header("Burst Animation Frames")]
    [SerializeField] private float firstBurstShotFrame = 5f;
    [SerializeField] private float lastBurstShotFrame = 5f;
    [SerializeField] private float burstAnimationLastFrame = 5f;

    private Transform target;

    private float cooldownTimer;
    private float burstShotTimer;

    private int burstShotCount = 0;

    private float pausedBurstNormalizedTime;

    private bool burstPausedWithoutTarget = false;

    private enum BurstState {
        Idle,
        Playing,
        WaitingBetweenShots,
        Resuming,
        Finishing
    }

    private BurstState burstState = BurstState.Idle;

    private void Start() {
        if (!burstAnimation || anim == null)
            return;

        anim.speed = 0f;

        anim.Play(
            burstAnimationState,
            0,
            0f
        );

        anim.Update(0f);
    }

    private void Update() {
        UpdateTarget();

        if (target != null) {
            RotateTowardsTarget();
        }

        if (!burstAnimation) {
            UpdateNormalTurret();
            return;
        }

        UpdateBurstTurret();
    }

    private void UpdateNormalTurret() {
        if (target == null)
            return;

        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= 1f / bps) {
            shoot();

            cooldownTimer = 0f;

            if (anim != null) {
                anim.speed = bps;
                anim.SetTrigger("Animate");
            }
        }
    }

    private void UpdateBurstTurret() {
        if (anim == null)
            return;

        switch (burstState) {
            case BurstState.Idle:
                UpdateBurstIdle();
                break;

            case BurstState.Playing:
                UpdateBurstPlaying();
                break;

            case BurstState.WaitingBetweenShots:
                UpdateBurstWaiting();
                break;

            case BurstState.Resuming:
                UpdateBurstResuming();
                break;

            case BurstState.Finishing:
                UpdateBurstFinishing();
                break;
        }
    }

    private void UpdateBurstIdle() {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer < 1f / bps)
            return;

        if (target == null)
            return;

        StartBurstAnimation();
    }

    private void UpdateBurstPlaying() {
        if (target == null) {
            PauseBurstWithoutTarget();
            return;
        }

        if (burstPausedWithoutTarget) {
            burstState = BurstState.Resuming;
            return;
        }

        anim.speed = 1f;

        AnimatorStateInfo stateInfo =
            anim.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName(burstAnimationState)) {
            anim.Play(
                burstAnimationState,
                0,
                0f
            );

            anim.Update(0f);

            return;
        }

        float normalizedTime =
            stateInfo.normalizedTime % 1f;

        float currentFrame =
            normalizedTime * burstAnimationLastFrame;

        if (burstShotCount < visualShotsPerCycle) {
            float nextShotFrame =
                GetNextBurstShotFrame();

            if (currentFrame >= nextShotFrame) {
                FireBurstMissile();
                return;
            }
        }

        if (stateInfo.normalizedTime >= 1f &&
            burstShotCount >= visualShotsPerCycle) {

            burstState = BurstState.Finishing;
            anim.speed = 1f;
        }
    }

    private void UpdateBurstResuming() {
        if (target == null) {
            burstState = BurstState.Playing;
            PauseBurstWithoutTarget();
            return;
        }

        anim.Play(
            burstAnimationState,
            0,
            pausedBurstNormalizedTime
        );

        anim.Update(0f);

        burstPausedWithoutTarget = false;

        burstState = BurstState.Playing;

        anim.speed = 1f;
    }

    private void PauseBurstWithoutTarget() {
        AnimatorStateInfo stateInfo =
            anim.GetCurrentAnimatorStateInfo(0);

        if (!burstPausedWithoutTarget) {
            pausedBurstNormalizedTime =
                stateInfo.normalizedTime % 1f;

            burstPausedWithoutTarget = true;

            ForceConfirmedShotVisual();
        }

        anim.speed = 0f;
    }

    private void ForceConfirmedShotVisual() {
        if (anim == null)
            return;

        float visualFrame = 0f;

        if (burstShotCount <= 0) {
            visualFrame = 0f;
        } else {
            int shotIndex = burstShotCount - 1;

            if (visualShotsPerCycle <= 1) {
                visualFrame = firstBurstShotFrame;
            } else {
                float shotSpacing =
                    (lastBurstShotFrame - firstBurstShotFrame) /
                    (visualShotsPerCycle - 1);

                visualFrame =
                    firstBurstShotFrame +
                    (shotSpacing * shotIndex);
            }
        }

        float normalizedTime =
            visualFrame / burstAnimationLastFrame;

        normalizedTime =
            Mathf.Clamp01(normalizedTime);

        anim.Play(
            burstAnimationState,
            0,
            normalizedTime
        );

        anim.Update(0f);
    }

    private void UpdateBurstWaiting() {
        if (target == null) {
            anim.speed = 0f;
            return;
        }

        burstShotTimer += Time.deltaTime;

        if (burstShotTimer >= 1f / bps) {
            burstShotTimer = 0f;

            burstState = BurstState.Playing;

            anim.speed = 1f;
        }
    }

    private void UpdateBurstFinishing() {
        anim.speed = 1f;

        AnimatorStateInfo stateInfo =
            anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f) {
            FinishBurstAnimation();
        }
    }

    private float GetNextBurstShotFrame() {
        if (visualShotsPerCycle <= 1)
            return firstBurstShotFrame;

        float shotSpacing =
            (lastBurstShotFrame - firstBurstShotFrame) /
            (visualShotsPerCycle - 1);

        return firstBurstShotFrame +
               (shotSpacing * burstShotCount);
    }

    private void StartBurstAnimation() {
        burstShotCount = 0;

        burstShotTimer = 0f;
        cooldownTimer = 0f;

        pausedBurstNormalizedTime = 0f;
        burstPausedWithoutTarget = false;

        burstState = BurstState.Playing;

        anim.speed = 1f;

        anim.Play(
            burstAnimationState,
            0,
            0f
        );

        anim.Update(0f);
    }

    private void FireBurstMissile() {
        if (target == null) {
            PauseBurstWithoutTarget();
            return;
        }

        if (!CheckTargetIsInRange()) {
            FindTarget();

            if (target == null) {
                PauseBurstWithoutTarget();
                return;
            }
        }

        shoot();

        burstShotCount++;

        burstPausedWithoutTarget = false;

        ForceShotVisualFrame();

        Debug.Log(
            "BYTE disparou o míssil visual " +
            burstShotCount +
            "/" +
            visualShotsPerCycle
        );

        if (burstShotCount < visualShotsPerCycle) {
            burstShotTimer = 0f;

            burstState = BurstState.WaitingBetweenShots;

            anim.speed = 0f;

            return;
        }

        burstState = BurstState.Finishing;

        anim.speed = 1f;
    }

    private void ForceShotVisualFrame() {
        if (anim == null)
            return;

        int shotIndex = burstShotCount - 1;

        float shotFrame;

        if (visualShotsPerCycle <= 1) {
            shotFrame = firstBurstShotFrame;
        } else {
            float shotSpacing =
                (lastBurstShotFrame - firstBurstShotFrame) /
                (visualShotsPerCycle - 1);

            shotFrame =
                firstBurstShotFrame +
                (shotSpacing * shotIndex);
        }

        float normalizedTime =
            shotFrame / burstAnimationLastFrame;

        normalizedTime =
            Mathf.Clamp01(normalizedTime);

        anim.Play(
            burstAnimationState,
            0,
            normalizedTime
        );

        anim.Update(0f);
    }

    private void FinishBurstAnimation() {
        burstShotCount = 0;

        burstShotTimer = 0f;

        cooldownTimer = 0f;

        pausedBurstNormalizedTime = 0f;
        burstPausedWithoutTarget = false;

        burstState = BurstState.Idle;

        anim.speed = 0f;

        anim.Play(
            burstAnimationState,
            0,
            0f
        );

        anim.Update(0f);
    }

    private void shoot() {
        if (target == null)
            return;

        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firingPoint.position,
            Quaternion.identity
        );

        Bullet bulletScript =
            bulletObj.GetComponent<Bullet>();

        if (bulletScript != null) {
            if (burstAnimation) {
                bulletScript.SetDamage(bulletDamage);
            }

            bulletScript.SetTarget(target);
        }
    }

    private void UpdateTarget() {
        if (target == null) {
            FindTarget();
            return;
        }

        if (!CheckTargetIsInRange()) {
            target = null;
            FindTarget();
            return;
        }

        Transform bestTarget = FindBestTarget();

        if (bestTarget != null) {
            target = bestTarget;
        }
    }

    private Transform FindBestTarget() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            targetingRange,
            (Vector2)transform.position,
            0f,
            enemyMask
        );

        if (hits.Length == 0)
            return null;

        Transform bestTarget = null;
        int bestPathIndex = -1;

        foreach (RaycastHit2D hit in hits) {
            int pathIndex = GetPathIndex(hit.transform);

            if (pathIndex > bestPathIndex) {
                bestPathIndex = pathIndex;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }

    private void FindTarget() {
        target = FindBestTarget();
    }

    private int GetPathIndex(Transform enemy) {
        if (enemy == null)
            return -1;

        if (LevelManager.main == null)
            return -1;

        if (LevelManager.main.path == null ||
            LevelManager.main.path.Length == 0)
            return -1;

        int closestPathIndex = 0;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < LevelManager.main.path.Length; i++) {
            if (LevelManager.main.path[i] == null)
                continue;

            float distance = Vector2.Distance(
                enemy.position,
                LevelManager.main.path[i].position
            );

            if (distance < closestDistance) {
                closestDistance = distance;
                closestPathIndex = i;
            }
        }

        return closestPathIndex;
    }

    private void RotateTowardsTarget() {
        if (target == null)
            return;

        float Angle = Mathf.Atan2(
            target.position.y - transform.position.y,
            target.position.x - transform.position.x
        ) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation =
            Quaternion.Euler(
                new Vector3(0f, 0f, Angle)
            );

        turretRotationPoint.rotation =
            Quaternion.RotateTowards(
                turretRotationPoint.rotation,
                targetRotation,
                rotationSpeed * Time.timeScale
            );
    }

    private bool CheckTargetIsInRange() {
        if (target == null)
            return false;

        return Vector2.Distance(
            target.position,
            transform.position
        ) <= targetingRange;
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.cyan;

        Handles.DrawWireDisc(
            transform.position,
            transform.forward,
            targetingRange
        );
    }
}