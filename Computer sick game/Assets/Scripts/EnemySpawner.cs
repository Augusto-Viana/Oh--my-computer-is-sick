using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour {
    public static EnemySpawner main;

    [Header("References")]
    [SerializeField] private GameObject[] commonEnemyPrefabs;
    // 0 = Logic Bomb
    // 1 = Malware
    // 2 = Worm

    [SerializeField] private GameObject[] eliteEnemyPrefabs;
    // 0 = Maniacware
    // 1 = Logic Inferno
    // 2 = Worm.EXE

    [SerializeField] private GameObject[] bosses;
    // 0 = P.H.A.G.E.
    // 1 = R.A.I.D.E.R.

    [Header("Attributes")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float baseEnemiesPerSecond = 0.7f;
    [SerializeField] private float maxEnemiesPerSecond = 3f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private List<GameObject> currentWaveEnemies = new List<GameObject>();

    private void Awake() {
        main = this;
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void OnDestroy() {
        onEnemyDestroy.RemoveListener(EnemyDestroyed);
    }

    private void Start() {
        StartCoroutine(StartWave());
    }

    private void Update() {
        if (!isSpawning)
            return;

        timeSinceLastSpawn += Time.deltaTime;

        float currentEnemiesPerSecond = CalculateEnemiesPerSecond();

        if (timeSinceLastSpawn >= (1f / currentEnemiesPerSecond) && enemiesLeftToSpawn > 0) {
            SpawnEnemy();

            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0) {
            EndWave();
        }
    }

    private void EnemyDestroyed() {
        enemiesAlive--;

        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }

    private void SpawnEnemy() {
        if (currentWaveEnemies.Count == 0)
            return;

        int index = Random.Range(0, currentWaveEnemies.Count);

        GameObject enemyPrefab = currentWaveEnemies[index];

        currentWaveEnemies.RemoveAt(index);

        Instantiate(
            enemyPrefab,
            LevelManager.main.startPoint.position,
            Quaternion.identity
        );
    }

    private IEnumerator StartWave() {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWaveEnemies = GetEnemiesForWave(currentWave);

        enemiesLeftToSpawn = currentWaveEnemies.Count;
        enemiesAlive = 0;
        isSpawning = true;
        timeSinceLastSpawn = 0f;
    }

    private void EndWave() {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        enemiesLeftToSpawn = 0;
        enemiesAlive = 0;

        if (currentWave >= 30) {
            if (GameOverManager.main != null &&
                GameOverManager.main.IsGameOver) {
                Debug.Log("[EnemySpawner] Derrota detectada. Vitória cancelada.");
                return;
            }

            LevelManager.main.wave = 30;

            Debug.Log("Todas as 30 ondas foram concluídas!");

            if (VictoryManager.main != null) {
                VictoryManager.main.Victory();
            } else {
                Debug.LogError("[EnemySpawner] VictoryManager.main não foi encontrado!");
            }

            return;
        }

        currentWave++;
        LevelManager.main.wave++;

        StartCoroutine(StartWave());
    }

    private List<GameObject> GetEnemiesForWave(int wave) {
        List<GameObject> enemies = new List<GameObject>();

        switch (wave) {
            case 1:
                AddEnemies(enemies, commonEnemyPrefabs[0], 8);
                break;

            case 2:
                AddEnemies(enemies, commonEnemyPrefabs[0], 10);
                break;

            case 3:
                AddEnemies(enemies, commonEnemyPrefabs[0], 12);
                break;

            case 4:
                AddEnemies(enemies, commonEnemyPrefabs[0], 10);
                AddEnemies(enemies, commonEnemyPrefabs[1], 2);
                break;

            case 5:
                AddEnemies(enemies, commonEnemyPrefabs[0], 12);
                AddEnemies(enemies, commonEnemyPrefabs[1], 4);
                break;

            case 6:
                AddEnemies(enemies, commonEnemyPrefabs[0], 14);
                AddEnemies(enemies, commonEnemyPrefabs[1], 5);
                break;

            case 7:
                AddEnemies(enemies, commonEnemyPrefabs[0], 14);
                AddEnemies(enemies, commonEnemyPrefabs[1], 8);
                break;

            case 8:
                AddEnemies(enemies, commonEnemyPrefabs[0], 12);
                AddEnemies(enemies, commonEnemyPrefabs[1], 8);
                AddEnemies(enemies, commonEnemyPrefabs[2], 1);
                break;

            case 9:
                AddEnemies(enemies, commonEnemyPrefabs[0], 15);
                AddEnemies(enemies, commonEnemyPrefabs[1], 8);
                AddEnemies(enemies, commonEnemyPrefabs[2], 3);
                break;

            case 10:
                AddEnemies(enemies, commonEnemyPrefabs[0], 15);
                AddEnemies(enemies, commonEnemyPrefabs[1], 10);
                AddEnemies(enemies, commonEnemyPrefabs[2], 4);
                break;

            case 11:
                AddEnemies(enemies, commonEnemyPrefabs[0], 16);
                AddEnemies(enemies, commonEnemyPrefabs[1], 10);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                break;

            case 12:
                AddEnemies(enemies, commonEnemyPrefabs[0], 16);
                AddEnemies(enemies, commonEnemyPrefabs[1], 12);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                break;

            case 13:
                AddEnemies(enemies, commonEnemyPrefabs[0], 15);
                AddEnemies(enemies, commonEnemyPrefabs[1], 12);
                AddEnemies(enemies, commonEnemyPrefabs[2], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 1);
                break;

            case 14:
                AddEnemies(enemies, commonEnemyPrefabs[0], 17);
                AddEnemies(enemies, commonEnemyPrefabs[1], 12);
                AddEnemies(enemies, commonEnemyPrefabs[2], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 2);
                break;

            case 15:
                AddEnemies(enemies, commonEnemyPrefabs[0], 17);
                AddEnemies(enemies, commonEnemyPrefabs[1], 14);
                AddEnemies(enemies, commonEnemyPrefabs[2], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 2);
                break;

            case 16:
                AddEnemies(enemies, commonEnemyPrefabs[0], 18);
                AddEnemies(enemies, commonEnemyPrefabs[1], 14);
                AddEnemies(enemies, commonEnemyPrefabs[2], 7);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 3);
                break;

            case 17:
                AddEnemies(enemies, commonEnemyPrefabs[0], 18);
                AddEnemies(enemies, commonEnemyPrefabs[1], 14);
                AddEnemies(enemies, commonEnemyPrefabs[2], 7);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 3);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 1);
                break;

            case 18:
                AddEnemies(enemies, commonEnemyPrefabs[0], 19);
                AddEnemies(enemies, commonEnemyPrefabs[1], 15);
                AddEnemies(enemies, commonEnemyPrefabs[2], 8);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 3);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 2);
                break;

            case 19:
                AddEnemies(enemies, commonEnemyPrefabs[0], 19);
                AddEnemies(enemies, commonEnemyPrefabs[1], 16);
                AddEnemies(enemies, commonEnemyPrefabs[2], 8);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 4);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 2);
                break;

            case 20:
                AddEnemies(enemies, bosses[0], 1);
                break;

            case 21:
                AddEnemies(enemies, commonEnemyPrefabs[0], 14);
                AddEnemies(enemies, commonEnemyPrefabs[1], 10);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 4);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 2);
                break;

            case 22:
                AddEnemies(enemies, commonEnemyPrefabs[0], 14);
                AddEnemies(enemies, commonEnemyPrefabs[1], 10);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 3);
                break;

            case 23:
                AddEnemies(enemies, commonEnemyPrefabs[0], 13);
                AddEnemies(enemies, commonEnemyPrefabs[1], 9);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 3);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 1);
                break;

            case 24:
                AddEnemies(enemies, commonEnemyPrefabs[0], 13);
                AddEnemies(enemies, commonEnemyPrefabs[1], 9);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 4);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 1);
                break;

            case 25:
                AddEnemies(enemies, commonEnemyPrefabs[0], 12);
                AddEnemies(enemies, commonEnemyPrefabs[1], 9);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 7);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 4);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 1);
                AddEnemies(enemies, bosses[0], 1);
                break;

            case 26:
                AddEnemies(enemies, commonEnemyPrefabs[0], 12);
                AddEnemies(enemies, commonEnemyPrefabs[1], 8);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 7);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 2);
                AddEnemies(enemies, bosses[0], 1);
                break;

            case 27:
                AddEnemies(enemies, commonEnemyPrefabs[0], 11);
                AddEnemies(enemies, commonEnemyPrefabs[1], 8);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 8);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 2);
                AddEnemies(enemies, bosses[0], 1);
                break;

            case 28:
                AddEnemies(enemies, commonEnemyPrefabs[0], 10);
                AddEnemies(enemies, commonEnemyPrefabs[1], 7);
                AddEnemies(enemies, commonEnemyPrefabs[2], 5);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 8);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 2);
                AddEnemies(enemies, bosses[0], 2);
                break;

            case 29:
                AddEnemies(enemies, commonEnemyPrefabs[0], 9);
                AddEnemies(enemies, commonEnemyPrefabs[1], 7);
                AddEnemies(enemies, commonEnemyPrefabs[2], 4);
                AddEnemies(enemies, eliteEnemyPrefabs[0], 8);
                AddEnemies(enemies, eliteEnemyPrefabs[1], 6);
                AddEnemies(enemies, eliteEnemyPrefabs[2], 3);
                AddEnemies(enemies, bosses[0], 2);
                break;

            case 30:
                AddEnemies(enemies, bosses[1], 1);
                break;
        }

        return enemies;
    }

    private void AddEnemies(List<GameObject> enemies, GameObject enemyPrefab, int amount) {
        for (int i = 0; i < amount; i++) {
            enemies.Add(enemyPrefab);
        }
    }

    private float CalculateEnemiesPerSecond() {
        float scaledRate =
            baseEnemiesPerSecond *
            Mathf.Pow(1.1f, currentWave - 1);

        return Mathf.Min(
            scaledRate,
            maxEnemiesPerSecond
        );
    }
}