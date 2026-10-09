using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryManager : MonoBehaviour {
    public static VictoryManager main;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Animator AnimationFadeOut;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private AudioSource music;

    public bool IsVictory { get; private set; } = false;
    private bool isTransitioning = false;

    private void Awake() {
        main = this;
    }

    private void Start() {
        victoryPanel.SetActive(false);

        //Victory();
    }

    public void Victory() {
        if (IsVictory) {
            return;
        }

        IsVictory = true;

        if (GameOverManager.main != null &&
            GameOverManager.main.IsGameOver) {
            Debug.Log("[VictoryManager] O jogador perdeu. Vitória cancelada.");
            return;
        }

        Time.timeScale = 0f;

        if (music != null) {
            music.Pause();
        }

        victoryPanel.SetActive(true);
    }

    public void RestartLevel() {
        if (isTransitioning) {
            return;
        }

        StartCoroutine(FadeAndLoadScene(
            SceneManager.GetActiveScene().buildIndex
        ));
    }

    public void ReturnToMainMenu() {
        if (isTransitioning) {
            return;
        }

        StartCoroutine(FadeAndLoadScene(mainMenuSceneName));
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex) {
        yield return PlayFadeOut();

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator FadeAndLoadScene(string sceneName) {
        yield return PlayFadeOut();

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator PlayFadeOut() {
        isTransitioning = true;

        if (AnimationFadeOut == null) {
            Debug.LogError("[VictoryManager] Animator do FadeOut não foi associado!");
            yield break;
        }

        AnimationFadeOut.gameObject.SetActive(true);

        AnimationFadeOut.updateMode = AnimatorUpdateMode.UnscaledTime;

        AnimationFadeOut.ResetTrigger("FadeOut");
        AnimationFadeOut.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(fadeDuration);
    }
}