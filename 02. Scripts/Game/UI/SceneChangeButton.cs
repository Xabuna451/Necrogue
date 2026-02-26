using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Necrogue");
    }
    public void GameTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
