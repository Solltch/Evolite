using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public GameObject pauseMenu;
    public KeyCode pauseButton;
    public bool isPaused;

    public void Awake()
    {
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<GameObject>();
        pauseMenu.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseButton))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PlayGame()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(2);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
