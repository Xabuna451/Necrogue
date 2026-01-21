using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleGameStart : MonoBehaviour
{
    public Button gameStart;

    public void GameStart()
    {
        SceneManager.LoadScene("Necrogue");
    }
}
