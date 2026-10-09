using UnityEngine;

public class Life : MonoBehaviour {
    [Header("Prefabs do Coração")]
    [SerializeField] private GameObject fullHeartPrefab;    // 30�21
    [SerializeField] private GameObject mediumHeartPrefab;  // 20�11
    [SerializeField] private GameObject lowHeartPrefab;     // 10�1
    [SerializeField] private GameObject brokenHeartPrefab;  // 0

    // Instância atual do prefab ativo
    private GameObject currentHeartInstance;

    private enum HeartStage { Full, Medium, Low, Broken }
    private HeartStage currentStage = HeartStage.Full;

    private int lastLife = -1; // para detectar mudan�as na vida

    private void Start() {
        UpdateHeartPrefab(true); // for�a o primeiro cora��o ao iniciar
    }

    private void Update() {
        if (LevelManager.main == null) return;

        int life = LevelManager.main.life;

        // S� atualiza se a vida mudou desde o �ltimo frame
        if (life != lastLife) {
            lastLife = life;
            UpdateHeartPrefab();
        }
    }

    private void UpdateHeartPrefab(bool forceInstantiate = false) {
        int life = LevelManager.main.life;
        HeartStage newStage = DetermineStageForLife(life);

        if (!forceInstantiate && newStage == currentStage && currentHeartInstance != null)
            return; // sem necessidade de trocar o prefab

        currentStage = newStage;

        // Destroi o cora��o antigo, se existir
        if (currentHeartInstance != null) {
            Destroy(currentHeartInstance);
            currentHeartInstance = null;
        }

        // Escolhe o prefab certo de acordo com a vida
        GameObject prefabToSpawn = GetPrefabForStage(currentStage);

        if (prefabToSpawn == null) {
            Debug.LogWarning($"HeartManager: prefab para {currentStage} n�o atribu�do.");
            return;
        }

        // Instancia o novo prefab como filho do HeartManager
        currentHeartInstance = Instantiate(prefabToSpawn, transform);
        currentHeartInstance.transform.localPosition = Vector3.zero;
        currentHeartInstance.transform.localRotation = Quaternion.identity;
        currentHeartInstance.transform.localScale = Vector3.one;
    }

    private HeartStage DetermineStageForLife(int life) {
        if (life > 20)
            return HeartStage.Full;      // 30�21
        else if (life > 10)
            return HeartStage.Medium;    // 20�11
        else if (life > 0)
            return HeartStage.Low;       // 10�1
        else
            return HeartStage.Broken;    // 0
    }

    private GameObject GetPrefabForStage(HeartStage stage) {
        switch (stage) {
            case HeartStage.Full: return fullHeartPrefab;
            case HeartStage.Medium: return mediumHeartPrefab;
            case HeartStage.Low: return lowHeartPrefab;
            case HeartStage.Broken: return brokenHeartPrefab;
            default: return null;
        }
    }
}
