using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject settingsPanel; // ���� �ǳ� ������Ʈ�� ����

    void Start()
    {
        settingsPanel.SetActive(false); // �ʱ⿡�� �ǳ��� ����
    }

    public void OnSettingsButtonClicked()
    {
        settingsPanel.SetActive(true); // �ǳ� ��� Ȱ��ȭ
    }
}
