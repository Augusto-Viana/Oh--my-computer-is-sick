using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour {
    public static PauseManager main;

    public AudioSource music;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] public GameObject FadePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Animator AnimationFadeOut;
    [SerializeField] private float fadeDuration = 1.5f;

    public bool IsPaused { get; private set; } = false;

    private bool isTransitioning = false;

    private void Awake() {
        main = this;
    }

    private void Start() {
        pausePanel.SetActive(false);
    }

    private void Update() {
        if (isTransitioning) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    private void TogglePause() {
        if (IsPaused) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    public void PauseGame() {
        if (isTransitioning) {
            return;
        }

        IsPaused = true;
        Time.timeScale = 0f;
        music.Pause();
        pausePanel.SetActive(true);
    }

    public void ResumeGame() {
        if (isTransitioning) {
            return;
        }

        IsPaused = false;
        Time.timeScale = 1f;
        music.UnPause();
        pausePanel.SetActive(false);
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

        IsPaused = false;
        Time.timeScale = 1f;
        music.UnPause();

        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator FadeAndLoadScene(string sceneName) {
        yield return PlayFadeOut();

        IsPaused = false;
        Time.timeScale = 1f;
        music.UnPause();

        SceneManager.LoadScene(sceneName);
    }

private IEnumerator PlayFadeOut() {
        isTransitioning = true;

        // Verifica se o painel foi associado.
        if (FadePanel == null) {
            Debug.LogError("[PauseManager] FadePanel não foi associado!");
            isTransitioning = false;
            yield break;
        }

        // Garante que o painel esteja ativo.
        if (!FadePanel.activeSelf) {
            FadePanel.SetActive(true);
        }

        // Verifica se o Animator foi associado.
        if (AnimationFadeOut == null) {
            Debug.LogError("[PauseManager] Animator não foi associado!");
            isTransitioning = false;
            yield break;
        }

        // Permite que a animação continue com o jogo pausado.
        AnimationFadeOut.updateMode = AnimatorUpdateMode.UnscaledTime;

        Debug.Log("[PauseManager] Disparando animação FadeOut...");

        AnimationFadeOut.ResetTrigger("FadeOut");
        AnimationFadeOut.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(fadeDuration);
    }
}