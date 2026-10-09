using UnityEngine;
using UnityEngine.UI;

public class BYTE_Manager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMPro.TextMeshProUGUI costUI;


    [Header("Prefabs da BYTE")]
    [SerializeField] private GameObject BYTE1;
    [SerializeField] private GameObject BYTE2;
    [SerializeField] private GameObject BYTE3;

    private GameObject currentBYTEInstance;
    public int BYTELevel = 1;

    //Fazer o custo de BYTE ser maior a cada nível
    private int BYTEPrice {
        get {
            switch (BYTELevel) {
                case 1:
                    return 800;
                case 2:
                    return 1600;
                default:
                    return 0;
            }
        }
    }

    private enum BYTEStage { BYTE1, BYTE2, BYTE3 }
    private BYTEStage currentStage = BYTEStage.BYTE1;

    private void Start() {
        currentBYTEInstance = Instantiate(BYTE1, transform.position, Quaternion.identity, transform);
        currentStage = BYTEStage.BYTE1;

        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        UpdateCostText();
    }


    public void OpenUpgradeUI() {
        upgradeUI.SetActive(true);
        UpdateCostText();
    }

    public void CloseUpgradeUI() {
        upgradeUI.SetActive(false);
    }

    public void Upgrade() {

        Debug.Log("Upgrade clicado!");

        if (LevelManager.main.money < BYTEPrice || BYTELevel >= 3)
            return;

        LevelManager.main.SpendMoney(BYTEPrice);
        BYTELevel++;

        if (currentBYTEInstance != null) {
            Destroy(currentBYTEInstance);
            currentBYTEInstance = null;
        }


        switch (BYTELevel) {
            case 2:
                currentBYTEInstance = Instantiate(BYTE2, transform.position, Quaternion.identity, transform);
                currentStage = BYTEStage.BYTE2;
                break;
            case 3:
                currentBYTEInstance = Instantiate(BYTE3, transform.position, Quaternion.identity, transform);
                currentStage = BYTEStage.BYTE3;
                break;
        }

        UpdateCostText();
    }

    private void UpdateCostText() {
        if (costUI != null)
            costUI.text = BYTEPrice.ToString();
    }
}
