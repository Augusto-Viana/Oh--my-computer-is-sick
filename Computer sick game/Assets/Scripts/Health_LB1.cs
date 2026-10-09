using Unity.VisualScripting;
using UnityEngine;

public class Health_LB1 : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private double hitPoints = 3;
    [SerializeField] private int moneyWorth = 5;
    public int Damage = 1;

    private bool isDestroyed = false;
    public void takeDamage(double dmg) {
        hitPoints -= dmg;

        if (hitPoints <= 0 && !isDestroyed) {
            EnemySpawner.onEnemyDestroy.Invoke();
            isDestroyed = true;
            Destroy(gameObject);
            LevelManager.main.IncreaseMoney(moneyWorth);
        }
    }
}
