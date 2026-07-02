using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 🌟 추가: UI 선택(Focus)을 제어하기 위해 반드시 필요합니다!

public class GameManager : MonoBehaviour
{
    public GameObject pauseUI;
    public Slider volumeSlider;

    private bool isPaused = false;

    void Start()
    {
        pauseUI.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // 🌟 추가: 창이 열리자마자 볼륨 슬라이더를 '선택(Focus)' 상태로 만듭니다.
        if (volumeSlider != null)
        {
            // 혹시 다른 것이 선택되어 있을까 봐 한번 비워주고
            EventSystem.current.SetSelectedGameObject(null);
            // 슬라이더를 딱 집어서 선택해 줍니다!
            EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        }
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}