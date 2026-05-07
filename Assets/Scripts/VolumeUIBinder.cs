using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Связывает Slider громкости и Toggle включения звука с persistent MusicManager.
/// Вешается на любой объект в сцене (Canvas, VolumeSettings — без разницы).
///
/// При старте читает текущие значения из MusicManager и подписывает UI на изменения.
/// UnityEvents на самих Slider/Toggle нужно очистить — биндер делает всё в коде.
/// </summary>
public class VolumeUIBinder : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [SerializeField] private Slider volumeSlider;
    [Tooltip("Toggle, в котором isOn=true означает «звук включён»")]
    [SerializeField] private Toggle muteToggle;

    private void Start()
    {
        var music = MusicManager.Instance;
        if (music == null)
        {
            Debug.LogWarning("[VolumeUIBinder] MusicManager.Instance == null. " +
                             "Сцена запущена не из Bootstrap, биндинг пропущен.");
            return;
        }

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(music.Volume);
            volumeSlider.onValueChanged.AddListener(music.SetVolume);
        }

        if (muteToggle != null)
        {
            // isOn=true → звук вкл, isOn=false → выкл
            muteToggle.SetIsOnWithoutNotify(!music.IsMuted);
            muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
        }
    }

    private void OnDestroy()
    {
        if (volumeSlider != null && MusicManager.Instance != null)
            volumeSlider.onValueChanged.RemoveListener(MusicManager.Instance.SetVolume);

        if (muteToggle != null)
            muteToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);
    }

    private void OnMuteToggleChanged(bool isOn)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMuted(!isOn);
    }
}
