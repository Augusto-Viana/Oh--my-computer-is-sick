using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plot : MonoBehaviour {

    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject towerOBJ;
    public AMV_Manager AMV;
    public BYTE_Manager BYTE;
    public FIREWALL_Manager FIREWALL;
    private Color startColor;

    private void Start() {
        startColor = sr.color;
    }

    private void OnMouseEnter() {
        if (PauseManager.main.IsPaused) {
            return;
        }

        sr.color = hoverColor;
    }

    private void OnMouseExit() {
        sr.color = startColor;
    }

    private void OnMouseDown() {
        if (PauseManager.main.IsPaused) {
            return;
        }

        if (UIManager.main.IsHoveringUI())
            return;

        if (towerOBJ != null) {

            // Se existe um AMV_Manager nessa torre, abre a UI dele
            if (AMV != null) {
                if (AMV.AMVLevel < 3)
                    AMV.OpenUpgradeUI();
                return;
            }

            // Se existe um BYTE_Manager nessa torre, abre a UI dele
            if (BYTE != null) {
                if (BYTE.BYTELevel < 3)
                    BYTE.OpenUpgradeUI();
                return;
            }

            // Se existe um FIREWALL_Manager nessa torre, abre a UI dele
            if (FIREWALL != null) {
                if (FIREWALL.FIREWALLLevel < 3)
                    FIREWALL.OpenUpgradeUI();
                return;
            }

            return;
        }
        ;

        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        if (towerToBuild.cost > LevelManager.main.money) {
            Debug.Log("Você não tem dinheiro para comprar isso");
            return;
        }

        LevelManager.main.SpendMoney(towerToBuild.cost);

        towerOBJ = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        AMV = towerOBJ.GetComponent<AMV_Manager>();
        BYTE = towerOBJ.GetComponent<BYTE_Manager>();
        FIREWALL = towerOBJ.GetComponent<FIREWALL_Manager>();
    }
}