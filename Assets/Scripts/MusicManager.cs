using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет музыкой на протяжении всей игры.
/// Живёт между сценами (DontDestroyOnLoad) и автоматически
/// меняет трек при переходе между главным меню и уровнями.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Треки")]
    [Tooltip("Музыка главного меню")]
    [SerializeField] private AudioClip menuMusic;
    [Tooltip("Музыка игрового уровня")]
    [SerializeField] private AudioClip gameMusic;

    [Header("Ссылки")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;
    [Tooltip("Галочка вкл/выкл звука")]
    [SerializeField] private Toggle muteToggle;

    [Header("Настройки")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (volumeSlider != null)
            audioSource.volume = volumeSlider.value;

        // Синхронизируем mute с начальным состоянием Toggle.
        // Если Toggle.isOn = true → звук включён, false → выключен.
        if (muteToggle != null)
            audioSource.mute = !muteToggle.isOn;

        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // -------------------------------------------------------------------------
    // Обработчики
    // -------------------------------------------------------------------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    // -------------------------------------------------------------------------
    // Публичное API (кнопки UI)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Вкл/выкл музыку. Привязывается к OnValueChanged у Toggle.
    /// isOn = true → звук включён, isOn = false → выключен.
    /// </summary>
    public void OnOffAudio(bool isOn)
    {
        audioSource.mute = !isOn;
    }

    /// <summary>Изменить громкость через слайдер.</summary>
    public void VolumeBySlider()
    {
        if (volumeSlider != null)
            audioSource.volume = volumeSlider.value;
    }

    // -------------------------------------------------------------------------
    // Приватные методы
    // -------------------------------------------------------------------------

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip targetClip = sceneName == menuSceneName ? menuMusic : gameMusic;

        if (targetClip == null || audioSource.clip == targetClip) return;

        audioSource.clip = targetClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}