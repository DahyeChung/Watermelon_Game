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

    //ccy�߰�

    // ���� �г� ��Ʈ�ѷ��� ���� ����
    public SettingsPanelController settingsPanelController;

    // ������ �Ͻ� ���� �������� ����
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
    // �Ͻ� ���� �� �簳�� ���� �޼ҵ�
    public void TogglePause()
    {
        if (isPaused)
        {
            // ���� �簳
            ResumeGame();
        }
        else
        {
            // ���� �Ͻ� ����
            PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // ���� �ð��� ����

        // ��� ���� �Ͻ� ����
        SoundManager.Instance.PauseBGM();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // ���� �ð��� �������� �簳

        // ��� ���� �簳
        SoundManager.Instance.ResumeBGM();

        // ���� �г��� ����ϴ�.
        if (settingsPanelController != null)
        {
            settingsPanelController.settingsPanel.SetActive(false);
        }

        Debug.Log("turn off setting panel");

    }

    //ccy
}
