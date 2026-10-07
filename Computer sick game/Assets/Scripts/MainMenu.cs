using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuPrincipal : MonoBehaviour {
    [Header("Transição de Cena")]
    public GameObject Transicao;       // O painel com a imagem e o Animator
    public Animator AnimationFadeOut;       // O Animator do painel
    public float duracaoTransicao = 1.5f;    // Mesmo tempo da animação
    public AudioSource PlaySound; // Sonzinho top do menu

    public void Jogar() {
        Debug.Log("[MenuPrincipal] Botão Jogar clicado!");
        PlaySound.Play();
        StartCoroutine(CarregarCenaComAnimacao("SampleScene"));
    }

    public void Extras() {
        Debug.Log("[MenuPrincipal] Botão de extras clicado!");
        SceneManager.LoadScene("Extras");
    }
    public void Sair() {
        Debug.Log("[MenuPrincipal] Saindo do jogo...");
        Application.Quit();

        // Apenas para depuração dentro do Editor:
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    private IEnumerator CarregarCenaComAnimacao(string nomeCena) {
        // Garante que o painel está ativo (para o Animator poder tocar)
        if (Transicao != null && !Transicao.activeSelf) {
            Transicao.SetActive(true);
            Debug.Log("[MenuPrincipal] Painel de transição ativado.");
        }

        // Garante que temos um Animator configurado
        if (AnimationFadeOut == null) {
            Debug.LogError("[MenuPrincipal] Nenhum Animator atribuído ao painel de transição!");
            yield break;
        }

        // Dispara a animação de FadeOut
        Debug.Log("[MenuPrincipal] Disparando animação FadeOut...");
        AnimationFadeOut.ResetTrigger("FadeOut"); // limpa antes de usar, só por segurança
        AnimationFadeOut.SetTrigger("FadeOut");

        // Espera o tempo da animação
        yield return new WaitForSeconds(duracaoTransicao);

        // Carrega a cena
        Debug.Log("[MenuPrincipal] Carregando cena: " + nomeCena);
        SceneManager.LoadScene(nomeCena);
    }
}

