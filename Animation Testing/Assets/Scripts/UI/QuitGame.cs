using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private GameObject quitPanel;

    private bool paused;

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;

        paused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        quitPanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        paused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        quitPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Qutting");
        Application.Quit();
    }
}
