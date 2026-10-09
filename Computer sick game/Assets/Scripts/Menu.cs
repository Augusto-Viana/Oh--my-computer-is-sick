using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour {
    [Header("References")]
    [SerializeField] TextMeshProUGUI moneyUI;
    [SerializeField] TextMeshProUGUI lifeUI;
    [SerializeField] TextMeshProUGUI WaveUI;
    [SerializeField] Animator anim;

    private bool isMenuOpen = false;

    public void ToggleMenu() {
        isMenuOpen = !isMenuOpen;
        anim.SetBool("MenuOpen", isMenuOpen);

    }

    private void OnGUI() {
        moneyUI.text = LevelManager.main.money.ToString();
        lifeUI.text = LevelManager.main.life.ToString();
        WaveUI.text = ($"{LevelManager.main.wave.ToString()}/50");
    }


}
