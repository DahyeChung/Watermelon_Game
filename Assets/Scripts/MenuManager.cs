using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : SingletonMonoBehaviour<MenuManager>
{
    [Header("Activation")]
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject BGMon;
    [SerializeField] private GameObject BGMoff;
    [SerializeField] private GameObject SFXon;
    [SerializeField] private GameObject SFXoff;
    [SerializeField] private GameObject infoOn;
    [SerializeField] private GameObject infoBG;
    [SerializeField] private GameObject Cross;
    [SerializeField] private GameObject CrossInfo;
    [SerializeField] private float animationSpeed;
    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button btn_BGM_ON;      // ON OFF music
    [SerializeField] private Button btn_BGM_OFF;     // ON OFF music
    [SerializeField] private Button btn_Sound_ON;    // ON OFF sfx
    [SerializeField] private Button btn_Sound_OFF;   // ON OFF sfx
    [SerializeField] private Button btn_InfoOn;      // ON info pannel
    [SerializeField] private Button btn_InfoOFF;     // OFF info pannel
    [SerializeField] private Button btn_Setting2;    // Slide up setting panel
    [SerializeField] private Button btn_OFFsetting;    // OFF Setting
    [SerializeField] private Button btn_ReStart;     // Restart in setting 
    [SerializeField] private Button btn_ReStart2;    // Restart in-game over
    [SerializeField] private Button btn_GameExit;    // Exit application


    public bool canPlaySFX;

    public void Start()
    {
        AddListener(btn_BGM_OFF, OnBGM);
        AddListener(btn_BGM_ON, OffBGM);
        AddListener(btn_Sound_OFF, OnSFX);
        AddListener(btn_Sound_ON, OffSFX);
        AddListener(btn_Setting2, OnSettingToggle);
        AddListener(btn_OFFsetting, OnSettingExitToggle);
        AddListener(btn_ReStart, OnRestartGame);
        AddListener(btn_ReStart2, OnRestartGame);
        AddListener(btn_GameExit, OnExitGame);
        AddListener(btn_InfoOn, OnInfo);
        AddListener(btn_InfoOFF, OFFInfo);
        InitButtons();
    }

    private void InitButtons()
    {
        settingCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        BGMoff.SetActive(false);
        SFXoff.SetActive(false);
        infoOn.SetActive(false);
        infoBG.SetActive(false);
        CrossInfo.SetActive(false);
    }

    //============UI ANIMATIONS==========================================================
    public void ScoreScaleAnim(GameObject obj, float scaleSize)
    {
        LeanTween.cancel(obj);
        obj.transform.localScale = Vector2.one;
        LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 1f).setEasePunch();
    }
    public void UnitScaleAnim(GameObject obj, float scaleSize)
    {
        LeanTween.cancel(obj);
        gameObject.transform.localScale = Vector2.one;
        LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 1f).setEasePunch();
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

    public void OnSettingToggle()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        settingCanvas.SetActive(true);
        Cross.SetActive(true);
        LeanTween.moveLocal(settingCanvas, new Vector3(0f, 2f, 0f), 0.01f).setEase(LeanTweenType.easeOutBounce);
        Time.timeScale = 0f;

    }
    public void OnSettingExitToggle()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        settingCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OffBGM()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        SoundManager.Instance.PauseBGM();
        BGMon.SetActive(false);
        BGMoff.SetActive(true);

    }

    public void OnBGM()
    {
        SoundManager.Instance.SetEffectVolume(0.5f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        SoundManager.Instance.ResumeBGM();
        BGMon.SetActive(true);
        BGMoff.SetActive(false);
    }

    public void OffSFX()
    {
        SoundManager.Instance.SetEffectVolume(0f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        SoundManager.Instance.PauseSFX();
        SFXon.SetActive(false);
        SFXoff.SetActive(true);
        canPlaySFX = false;
    }

    public void OnSFX()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        SoundManager.Instance.ResumeSFX();
        SFXon.SetActive(true);
        SFXoff.SetActive(false);
        canPlaySFX = true;
    }

    public void OnRestartGame()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        gameOverCanvas.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void OnInfo()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        infoOn.SetActive(true);
        infoBG.SetActive(true);
        Cross.SetActive(false);
        CrossInfo.SetActive(true);
    }
    public void OFFInfo()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DropSfx);
        infoOn.SetActive(false);
        infoBG.SetActive(false);
        Cross.SetActive(true);
        CrossInfo.SetActive(false);
        OnSettingToggle();


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
