using UnityEditor;
using UnityEngine;

public class FIREWALL : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;                 // Máscara dos inimigos
    [SerializeField] private Animator anim;                       // Controlador de animação
    [SerializeField] private GameObject areaObject;               // Objeto da área vermelha (GameObject inteiro)
    
    [Header("Attributes")]
    [SerializeField] private float damageRange = 4f;              // Raio da área de dano
    [SerializeField] private float damagePerSecond = 2f;          // Dano contínuo por segundo
    [SerializeField] private float checkInterval = 0.2f;          // Intervalo entre verificações
    [SerializeField] private float lingerTimeNoEnemies = 0.1f;    // Tempo de espera antes de desligar (evita flicker)

    private bool isOn = false;            // Estado atual (ligado/desligado)
    private bool animationPlayed = false; // Evita repetir animação desnecessariamente
    private float checkTimer = 0f;        // Timer interno de verificação
    private float noEnemiesTimer = 0f;    // Timer para controlar o desligamento suave

    private System.Collections.Generic.Dictionary<Health_LB1, float> enemyDamageBuffer =
    new System.Collections.Generic.Dictionary<Health_LB1, float>();

    // --- Lógica principal ---
    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            HandleEnemiesInRange();
        }
    }

    // Detecta inimigos próximos e controla o estado da torre
    private void HandleEnemiesInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            damageRange,
            Vector2.zero,
            0f,
            enemyMask
        );

        bool hasEnemies = hits.Length > 0;

        if (hasEnemies)
        {
            noEnemiesTimer = 0f;

            if (!isOn)
                TurnOn();

            ApplyDamageToEnemies(hits);
        }
        else
        {
            noEnemiesTimer += checkInterval;
            if (isOn && noEnemiesTimer >= lingerTimeNoEnemies)
                TurnOff();
        }
    }

    // --- Liga a torre ---
    private void TurnOn()
    {
        isOn = true;

        // Toca a animação apenas uma vez
        if (!animationPlayed && anim != null)
        {
            anim.SetTrigger("Animate");
            animationPlayed = true;
        }

        // Ativa a área de dano (garante que o GameObject apareça)
        if (areaObject != null)
            areaObject.SetActive(true);
    }

    // --- Desliga a torre ---
    private void TurnOff()
    {
        isOn = false;

        anim.SetTrigger("Animate");
        animationPlayed = false;

        if (areaObject != null)
            areaObject.SetActive(false);

    }

    // --- Aplica dano contínuo nos inimigos detectados ---
    private void ApplyDamageToEnemies(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            Health_LB1 enemyHealth = hit.collider.GetComponent<Health_LB1>();
            if (enemyHealth == null) continue;

            // Acumula dano fracionário
            if (!enemyDamageBuffer.ContainsKey(enemyHealth))
                enemyDamageBuffer[enemyHealth] = 0f;

            enemyDamageBuffer[enemyHealth] += damagePerSecond * checkInterval;

            // Aplica 1 de dano cada vez que o acumulado chega a 1
            if (enemyDamageBuffer[enemyHealth] >= 1f)
            {
                int dmgToApply = Mathf.FloorToInt(enemyDamageBuffer[enemyHealth]);
                enemyDamageBuffer[enemyHealth] -= dmgToApply;
                enemyHealth.takeDamage(dmgToApply);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, damageRange);
    }
}
