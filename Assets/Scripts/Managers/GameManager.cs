using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool IsGameOver;
    public int Score;

    [SerializeField] private GameObject scorePoints; // Use for score animation
    //[SerializeField] private GameObject scoreResults;
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI scoreTextGameOver;
    public MenuManager Menu;

    //ccy추가

    // 설정 패널 컨트롤러에 대한 참조
    public SettingsPanelController settingsPanelController;

    // 게임이 일시 정지 상태인지 추적
    private bool isPaused = false;
    public bool isSettingOn = false;

    //ccy

    public void AddScore(int score)
    {
        this.Score += score;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreTextInGame != null)
        {
            scoreTextInGame.text = Score.ToString();
            scoreTextGameOver.text = Score.ToString();
            Menu.ScaleAnim(scorePoints, 1.5f);
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        scoreTextInGame = scoreTextGameOver;
        Menu.OnGameOverCanvas();
        Debug.Log("Game Over");
    }


    //ccy
    // 일시 정지 및 재개를 위한 메소드
    public void TogglePause()
    {
        if (isPaused)
        {
            // 게임 재개
            ResumeGame();
        }
        else
        {
            // 게임 일시 정지
            PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // 게임 시간을 정지

        // 배경 음악 일시 정지
        SoundManager.Instance.PauseBGM();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // 게임 시간을 정상으로 재개

        // 배경 음악 재개
        SoundManager.Instance.ResumeBGM();

        // 설정 패널을 숨깁니다.
        if (settingsPanelController != null)
        {
            settingsPanelController.settingsPanel.SetActive(false);
        }

        Debug.Log("turn off setting panel");

    }

    //ccy
}
