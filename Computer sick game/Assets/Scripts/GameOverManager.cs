using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour {
    public static GameOverManager main;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Animator AnimationFadeOut;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioSource music;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsGameOver { get; private set; } = false;
    private bool isTransitioning = false;

    private void Awake() {
        main = this;
    }

    private void Start() {
        gameOverPanel.SetActive(false);
    }

    public void GameOver() {
        if (IsGameOver) {
            return;
        }

        IsGameOver = true;

        Time.timeScale = 0f;

        music.Pause();

        gameOverPanel.SetActive(true);
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
            Debug.LogError("[GameOverManager] Animator do FadeOut não foi associado!");
            yield break;
        }

        AnimationFadeOut.gameObject.SetActive(true);

        AnimationFadeOut.updateMode = AnimatorUpdateMode.UnscaledTime;

        AnimationFadeOut.ResetTrigger("FadeOut");
        AnimationFadeOut.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(fadeDuration);
    }
}