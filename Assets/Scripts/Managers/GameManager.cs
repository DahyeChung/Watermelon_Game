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
    public UIanim UIanim;

    //ccy�߰�

    // ���� �г� ��Ʈ�ѷ��� ���� ����
    public SettingsPanelController settingsPanelController;

    // ������ �Ͻ� ���� �������� ����
    private bool isPaused = false;

    //ccy

    public void AddScore(int score)
    {
        Debug.Log("From" + score);
        this.Score += score;
        Debug.Log("To" + score);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreTextInGame != null)
        {
            scoreTextInGame.text = Score.ToString();
            scoreTextGameOver.text = Score.ToString();
            UIanim.ScaleAnim(scorePoints, 1.5f);
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        scoreTextInGame = scoreTextGameOver;
        UIanim.GameOverUI();
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
