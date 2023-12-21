using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject btn_BGM;      // ON OFF music
    public GameObject btn_Sound;    // ON OFF sfx
    public GameObject btn_Setting;  // Slide up setting pannel
    public GameObject btn_Pause;    // Pause game
    public GameObject btn_Continue; // Resume game
    public GameObject btn_ReStart;  // Restart in setting || in gameover
    public GameObject btn_GameExit; // Exit application
    public string thisScene;

    public void OnBGM()
    {
        //SoundManager.Instance.PlayBGM();
    }
    public void OffBGM()
    {
        SoundManager.Instance.OffBGM();
    }
    public void OnSFX()
    {

    }
    public void OffSFX()
    {

    }
    public void PauseGame()
    {
        thisScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 0f;
        btn_Pause.SetActive(true);

    }
    public void ReStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(thisScene);
        // 사운드 설정 유지한 채 씬로드 여부 체크
    }
    public void Continue()
    {
        btn_Pause.SetActive(false);
        Time.timeScale = 1f;
    }
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }







}
