using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private float animationSpeed;
    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button btn_BGM;      // ON OFF music
    [SerializeField] private Button btn_Sound;    // ON OFF sfx
    [SerializeField] private Button btn_Setting;  // Slide up setting panel
    [SerializeField] private Button btn_Setting2;  // Slide up setting panel
    [SerializeField] private Button btn_Pause;    // Pause game
    [SerializeField] private Button btn_Continue; // Resume game
    [SerializeField] private Button btn_ReStart;  // Restart in setting || in-game over
    [SerializeField] private Button btn_GameExit; // Exit application
    private string thisScene;
    //[SerializeField] private Unit unitScript;
    Unit unit;


    public void Start()
    {
        AddListener(btn_BGM, OnBGMToggle);
        AddListener(btn_Sound, OnSoundToggle);
        AddListener(btn_Setting, OnSettingToggle);
        AddListener(btn_Setting2, OnSettingToggle);
        AddListener(btn_Pause, OnPauseGame);
        AddListener(btn_Continue, OnContinueGame);
        AddListener(btn_ReStart, OnRestartGame);
        AddListener(btn_GameExit, OnExitGame);

        unit = new Unit();
    }

    //============UI ANIMATIONS==========================================================
    public void ScaleAnim(GameObject obj, float scaleSize)
    {
        LeanTween.cancel(obj);
        gameObject.transform.localScale = Vector2.one;
        LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 1f).setEasePunch();
        //LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 2f).setEase(LeanTweenType.easeOutElastic);
    }
    public void ButtonScaleAnim(GameObject obj, float scaleSize)
    {
        LeanTween.cancel(obj);
        gameObject.transform.localScale = Vector2.one;
        LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 1f).setEase(LeanTweenType.easeOutElastic);
    }
    public void OnGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
        //LeanTween.moveLocal(gameOver, new Vector3(0f, -12f, 0f), 0.4f).setEase(LeanTweenType.easeOutBounce);
        //LeanTween.moveLocal(backGroundMove, new Vector3(0f, 2f, 0f), 0.4f).setEase(LeanTweenType.easeOutBounce);
        //LeanTween.scale(starScale, new Vector3(100f, 100f, 100f), 0.2f).setEase(LeanTweenType.easeOutBounce);
    }
    //============BUTTONS==========================================================
    public void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    public void ButtonAnim()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
    }

    public void OnBGMToggle()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        //SoundManager.Instance.OffBGM();
    }

    public void OnSoundToggle()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        // Your sound toggle logic here
    }

    public void OnSettingToggle()
    {
        Debug.Log("Setting Button Worked");
        unit.IsMovable = false;
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        settingCanvas.SetActive(true);
        LeanTween.moveLocal(settingCanvas, new Vector3(0f, 2f, 0f), 0.01f).setEase(LeanTweenType.easeOutBounce);
        Time.timeScale = 0f;
        Debug.Log("∆€¡Ó  ≥°");

    }
    public void OnSettingExitToggle()
    {
        unit.IsMovable = true;
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        settingCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnPauseGame()
    {
        Time.timeScale = 0f;
    }

    public void OnContinueGame()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
    }

    public void OnRestartGame()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        gameOverCanvas.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void OnExitGame()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
