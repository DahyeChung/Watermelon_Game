using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 판넬 오브젝트를 연결

    void Start()
    {
        settingsPanel.SetActive(false); // 초기에는 판넬을 숨김
    }

    public void OnSettingsButtonClicked()
    {
        settingsPanel.SetActive(true); // 판넬 즉시 활성화
    }
}
