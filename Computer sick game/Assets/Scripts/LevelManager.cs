using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static LevelManager main; //Aqui � criado a classe do levelManager, e diz que ele � o principal

    public Transform startPoint;
    public Transform[] path; //Aqui s�o os campos que podem receber informa��es dentro do jogo, isso � uma lista e a de cima � algo fixo, nessa lista dentro do jogo tu coloca todos os pontos que v�o ser o caminho dos bixos no jogo.
    //Depois disso entendi que o comando "target = LevelManager.main.path[pathIndex];" diz que onde o inimigo tem que ir � o index em sequ�ncia dos pontos onde o inimigo tem que seguir que s�o colocados dentro do jogo, fazendo assim o seu path como alvo.

    public int money;
    public int wave;
    public int life;

    private void Awake() {
        main = this;
    }

    private void Start() {
        money = 999999999;
        life = 30;
        wave = 1;
    }

    public void IncreaseMoney(int amount) {
        money += amount;
    }

    public bool SpendMoney(int amount) {
        if (amount <= money) {
            money -= amount;
            return true;        
        } else {
            return false;
        }

    }
}