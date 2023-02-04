using UnityEngine;
//using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //[SerializeField]
    //private string NextScene;

    public void StartGame()
    {
        //SceneManager.LoadScene(NextScene);
        GameManager.Instance.LoadLevelScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
