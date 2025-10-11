using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
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
}
