using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MenuFunctions : MonoBehaviour
{
    public Material playerSkin;
    public Material playerSkin2;
    public Material playerEye;
    public Material playerPupil;
    public GameObject pauseMenu;
    public GameObject customMenu;
    public GameObject UsableMenus;
    private Vector3 CMpos;
    public KeyCode pauseButton;
    public bool isAbleToPause = true;
    public bool isPaused;
    public bool isInSkillTree;
    public Color pickedColor;
    public Button autoclick;

    public void Start()
    {
        CMpos = customMenu.GetComponent<RectTransform>().anchoredPosition;
        isAbleToPause = true;
        pauseMenu.SetActive(false);
        UsableMenus = GameObject.Find("UsableMenus");
        autoclick = GameObject.Find("HeadButton").GetComponent<Button>();
        autoclick.onClick.Invoke();
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseButton))
        {
            if (isAbleToPause)
            {
                if (!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
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
        if (!isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    //CUSTOMIZAÇÃO DE PERSONAGEM

    public void PickPlayerColor(UnityEngine.UI.Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerSkin.color = pickedColor;
    }
    public void PickPlayerColor2(UnityEngine.UI.Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerSkin2.SetColor("_Color", pickedColor);
    }
    public void PickEye(UnityEngine.UI.Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerEye.SetColor("_Color", pickedColor);
    }
    public void PickPupil(UnityEngine.UI.Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerPupil.SetColor("_Color", pickedColor);
    }

    public void LeaveCustumization()
    {
        customMenu.GetComponent<RectTransform>().anchoredPosition = CMpos;
        GameObject.Find("Player Collider").GetComponent<Player_Movement>().isAbleToMove = true;
        UsableMenus.SetActive(true);
        GameObject.Find("Player Sprite").GetComponent<Player_General>().isCustomizing = false;
        Time.timeScale = 1f;
        CinemachineRotationComposer cameraRotate = GameObject.Find("FreeLook Camera").GetComponent<CinemachineRotationComposer>();
        cameraRotate.Damping = Vector2.zero * 0.5f;
        cameraRotate.TargetOffset = Vector3.zero;
        isPaused = false;
        isAbleToPause = true;
    }
}
