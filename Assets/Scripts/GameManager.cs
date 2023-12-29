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
            Menu.ScoreScaleAnim(scorePoints, 1.5f);
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

    }

}
