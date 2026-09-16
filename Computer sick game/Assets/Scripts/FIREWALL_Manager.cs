using UnityEngine;
using UnityEngine.UI;

public class FIREWALL_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject upgradeUI; 
    [SerializeField] private Button upgradeButton; 
    [SerializeField] private TMPro.TextMeshProUGUI costUI;


    [Header("Prefabs da FIREWALL")]
    [SerializeField] private GameObject FIREWALL1;    
    [SerializeField] private GameObject FIREWALL2;  
    [SerializeField] private GameObject FIREWALL3;     

    private GameObject currentFIREWALLInstance;
    public int FIREWALLLevel = 1;

    //Fazer o custo de FIREWALL ser maior a cada nível
    private int FIREWALLPrice
    {
        get
        {
            switch (FIREWALLLevel)
            {
                case 1:
                    return 600;
                case 2:
                    return 850;
                default:
                    return 0;
            }
        }
    }

    private enum FIREWALLStage { FIREWALL1, FIREWALL2, FIREWALL3 }
    private FIREWALLStage currentStage = FIREWALLStage.FIREWALL1;

    private void Start()
    {
        currentFIREWALLInstance = Instantiate(FIREWALL1, transform.position, Quaternion.identity, transform);
        currentStage = FIREWALLStage.FIREWALL1;

        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        UpdateCostText();
    }


    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
        UpdateCostText();
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }

    public void Upgrade() {

        if (LevelManager.main.money < FIREWALLPrice || FIREWALLLevel >= 3)
            return;

        LevelManager.main.SpendMoney(FIREWALLPrice);
        FIREWALLLevel++;

        if (currentFIREWALLInstance != null)
        {
            Destroy(currentFIREWALLInstance);
            currentFIREWALLInstance = null;
        }


        switch (FIREWALLLevel)
        {
            case 2:
                currentFIREWALLInstance = Instantiate(FIREWALL2, transform.position, Quaternion.identity, transform);
                currentStage = FIREWALLStage.FIREWALL2;
                break;
            case 3:
                currentFIREWALLInstance = Instantiate(FIREWALL3, transform.position, Quaternion.identity, transform);
                currentStage = FIREWALLStage.FIREWALL3;
                break;
        }

        UpdateCostText();
    }

    private void UpdateCostText()
    {
        if (costUI != null)
            costUI.text = FIREWALLPrice.ToString();
    }
}
