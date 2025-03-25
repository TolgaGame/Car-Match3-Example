using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("UI Panels")]
    [SerializeField] private Canvas menuPanelCanvas;
    [SerializeField] private GraphicRaycaster menuPanelGraphicRaycaster;
    [SerializeField] private Canvas gamePanelCanvas;
    [SerializeField] private GraphicRaycaster gamePanelGraphicRaycaster;
    [SerializeField] private Canvas winPanelCanvas;
    [SerializeField] private GraphicRaycaster winPanelGraphicRaycaster;
    [SerializeField] private Canvas gameOverPanelCanvas;
    [SerializeField] private GraphicRaycaster gameOverPanelGraphicRaycaster;

    [Header("Texts")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text coinText;


    ////////////////////////////////////////////////

    #region Unity Methods

    private void OnEnable() {
        GameManager.OnGameWin += HandleGameWin;
        GameManager.OnGameLose += HandleGameLose;
    }

    private void OnDisable() {
        GameManager.OnGameWin -= HandleGameWin;
        GameManager.OnGameLose -= HandleGameLose;
    }

    #endregion

    #region Event Handlers

    private void HandleGameWin() {
        gamePanelCanvas.enabled = false;
        gamePanelGraphicRaycaster.enabled = false;
        winPanelCanvas.enabled = true;
        winPanelGraphicRaycaster.enabled = true;
    }

    private void HandleGameLose() {
        gamePanelCanvas.enabled = false;
        gamePanelGraphicRaycaster.enabled = false;
        gameOverPanelCanvas.enabled = true;
        gameOverPanelGraphicRaycaster.enabled = true;
    }

    #endregion

    #region UI Button Methods

    public void OnPressedStartButton() {
        Locator.Instance.GameManagerInstance.IsGameStarted = true;
        menuPanelCanvas.enabled = false;
        menuPanelGraphicRaycaster.enabled = false;
        gamePanelCanvas.enabled = true;
        gamePanelGraphicRaycaster.enabled = true;
    }

    public void OnPressedNextLevelButton() {
        Locator.Instance.GameManagerInstance.ReloadScene();
    }

    public void OnPressedContinueButton() {
        Locator.Instance.GameManagerInstance.IsGameStarted = true;
        Locator.Instance.GameManagerInstance.DiscardCoin(25);
       // Locator.Instance.MatchControllerInstance.TrueMatch();
    }

    public void OnPressedRestartButton() {
        Locator.Instance.GameManagerInstance.ReloadScene();
    }

    private void UpdateCoinUI() => coinText.text = Locator.Instance.GameManagerInstance.Coin.ToString();

    private void UpdateLevelUI() => levelText.text = "LEVEL " + Locator.Instance.GameManagerInstance.Level;


    #endregion

}