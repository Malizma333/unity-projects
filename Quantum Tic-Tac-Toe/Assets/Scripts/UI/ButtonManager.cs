using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

internal class ButtonManager : MonoBehaviour {
    private static GameManager gameManager;

    [SerializeField]
    private Slider modeSlider;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private Slider playerSlider;
    [SerializeField]
    private GameObject instructionPanel;
    [SerializeField]
    private GameObject gameCanvas;

    private static bool instructShown = false;

    public void Initialize(GameManager gameManager) {
        ButtonManager.gameManager = gameManager;
        
        modeSlider.value = PlayerPrefs.GetInt("ModeSlider", 0);
        sizeSlider.value = PlayerPrefs.GetInt("SizeSlider", 3);
        playerSlider.value = PlayerPrefs.GetInt("PlayerSlider", 2);

        ShowInstructions();
    }

    public void SwitchMode() {
        modeSlider.value = 1 - modeSlider.value;
        PlayerPrefs.SetInt("ModeSlider", (int) modeSlider.value);
    }

    public void ChangeBoardSize() {
        PlayerPrefs.SetInt("SizeSlider", (int) sizeSlider.value);
    }

    public void ChangePlayerCount() {
        PlayerPrefs.SetInt("PlayerSlider", (int) playerSlider.value);
    }

    private void ShowInstructions() {
        if(!instructShown) {
            gameCanvas.SetActive(false);
            instructionPanel.SetActive(true);
            instructShown = true;
        } else {
            HideInstructions();
        }
    }

    public void HideInstructions() {
        gameCanvas.SetActive(true);
        instructionPanel.SetActive(false);
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
