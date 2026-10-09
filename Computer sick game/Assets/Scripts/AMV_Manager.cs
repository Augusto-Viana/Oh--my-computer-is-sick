using UnityEngine;
using UnityEngine.UI;

public class AMV_Manager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMPro.TextMeshProUGUI costUI;


    [Header("Prefabs da AMV")]
    [SerializeField] private GameObject AMV1;
    [SerializeField] private GameObject AMV2;
    [SerializeField] private GameObject AMV3;

    private GameObject currentAMVInstance;
    public int AMVLevel = 1;

    //Fazer o custo de AMV ser maior a cada nível
    private int AMVPrice {
        get {
            switch (AMVLevel) {
                case 1:
                    return 100;
                case 2:
                    return 200;
                default:
                    return 0;
            }
        }
    }

    private enum AMVStage { AMV1, AMV2, AMV3 }
    private AMVStage currentStage = AMVStage.AMV1;

    private void Start() {
        currentAMVInstance = Instantiate(AMV1, transform.position, Quaternion.identity, transform);
        currentStage = AMVStage.AMV1;

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

        if (LevelManager.main.money < AMVPrice || AMVLevel >= 3)
            return;

        LevelManager.main.SpendMoney(AMVPrice);
        AMVLevel++;

        if (currentAMVInstance != null) {
            Destroy(currentAMVInstance);
            currentAMVInstance = null;
        }


        switch (AMVLevel) {
            case 2:
                currentAMVInstance = Instantiate(AMV2, transform.position, Quaternion.identity, transform);
                currentStage = AMVStage.AMV2;
                break;
            case 3:
                currentAMVInstance = Instantiate(AMV3, transform.position, Quaternion.identity, transform);
                currentStage = AMVStage.AMV3;
                break;
        }

        UpdateCostText();
    }

    private void UpdateCostText() {
        if (costUI != null)
            costUI.text = AMVPrice.ToString();
    }
}
