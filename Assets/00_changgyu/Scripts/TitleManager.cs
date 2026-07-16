using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void GameStartBtn()
    {
        SceneManager.LoadScene("mian_village");
    }
}
