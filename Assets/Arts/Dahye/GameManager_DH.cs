using TMPro;
using UnityEngine;

public class GameManager_DH : SingletonMonoBehaviour<GameManager_DH>
{
    public bool IsGameOver;
    public int Score;

    [SerializeField] private GameObject scorePoints;
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI scoreTextGameOver;
    public MenuManager Menu;

    public SettingsPanelController settingsPanelController;

    private bool isPaused = false;
    public bool isSettingOn = false;

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
        SoundManager.Instance.PlaySFX(SoundManager.Instance.FinishBell);
        Menu.OnGameOverCanvas();
        Debug.Log("Game Over");
    }

    public void TogglePause()
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

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        SoundManager.Instance.PauseBGM();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        SoundManager.Instance.ResumeBGM();

        if (settingsPanelController != null)
        {
            settingsPanelController.settingsPanel.SetActive(false);
        }

        Debug.Log("turn off setting panel");
    }
}
