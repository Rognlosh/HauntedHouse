using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Связывает UI-элементы меню (Slider громкости, Toggle вкл/выкл звука)
/// с MusicManager. Кладётся на сцену MainMenu.
///
/// MusicManager при этом не знает про UI и существует независимо
/// (живёт между сценами как DontDestroyOnLoad). Этот компонент —
/// просто проводник «UI ↔ менеджер» на конкретной сцене.
///
/// Toggle.isOn = true → звук включён (mute = false).
/// </summary>
public class MenuAudioBinder : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [Tooltip("isOn = true → звук включён (не заглушено)")]
    [SerializeField] private Toggle muteToggle;

    private void OnEnable()
    {
        var mm = MusicManager.Instance;
        if (mm == null)
        {
            Debug.LogWarning(
                "[MenuAudioBinder] MusicManager.Instance не готов. " +
                "Проверь, что префаб лежит в Assets/Resources/MusicManager.prefab.");
            return;
        }

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(mm.Volume);
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        if (muteToggle != null)
        {
            // isOn = звук включён → инверсия от mute
            muteToggle.SetIsOnWithoutNotify(!mm.IsMuted);
            muteToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void OnDisable()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        if (muteToggle != null)
            muteToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(value);
    }

    private void OnToggleChanged(bool isOn)
    {
        // isOn = звук включён → muted = !isOn
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMuted(!isOn);
    }
}
