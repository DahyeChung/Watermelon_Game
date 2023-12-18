using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool IsGameOver;
    public int Score;

    [SerializeField]
    GameObject scoreText;
    public TextMeshProUGUI scoreTextInGame;
    //public TextMeshProUGUI scoreTextGameOver;
    public UIanim UIanim;



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
            UIanim.ScaleAnim(scoreText, 1.5f);
        }

        //if (scoreTextGameOver != null)
        //{
        //    scoreTextGameOver.text = Score.ToString();
        //}
    }

    public void GameOver()
    {
        IsGameOver = true;
        UIanim.GameOverUI();
        Debug.Log("Game Over");
    }
}
