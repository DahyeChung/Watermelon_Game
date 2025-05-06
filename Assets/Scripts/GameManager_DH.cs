using TMPro;
using UnityEngine;

public class GameManager_DH : SingletonMonoBehaviour<GameManager_DH>
{
    public bool IsGameOver;
    public int Score;

    [SerializeField] private GameObject scorePoints;
    [SerializeField] private float scoreAnimScale = 2f;
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI scoreTextGameOver;
    public MenuManager Menu;


    private void Update()
    {
        Debug.Log("������Ʈ �� ���ư�?");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("���� �����Կ�~");
            GameOver();
        }
    }

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
            Menu.ScoreScaleAnim(scorePoints, scoreAnimScale);
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        scoreTextInGame = scoreTextGameOver;
        SoundManager.Instance.PlaySFX(SoundManager.Instance.FinishBell);
        Menu.OnGameOverCanvas();

        // PlayfabManager�� ���� �������忡 ���� ���� ����
        PlayfabManager playfabManager = FindObjectOfType<PlayfabManager>();
        if (playfabManager != null)
        {
            playfabManager.SendLeaderBoard(Score);
        }
        else
        {
            Debug.LogError("PlayfabManager�� ���� �����ϴ�!");
        }
    }
}
