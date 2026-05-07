using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent-singleton, управляющий музыкой на протяжении всей игры.
/// Создаётся один раз в Bootstrap-сцене и переживает все смены сцен через
/// DontDestroyOnLoad. UI-привязка делается scene-side через VolumeUIBinder.
///
/// Громкость и mute хранятся в PlayerPrefs и переживают рестарт игры.
/// В Bootstrap-сцене музыка не играет — старт происходит при загрузке
/// первой реальной сцены (MainMenu или уровень при прямом запуске).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Треки")]
    [Tooltip("Музыка главного меню")]
    [SerializeField] private AudioClip menuMusic;
    [Tooltip("Музыка игрового уровня")]
    [SerializeField] private AudioClip gameMusic;

    [Header("Настройки сцен")]
    [SerializeField] private string menuSceneName = "MainMenu";
    [Tooltip("Имя bootstrap-сцены — в ней музыка не играет")]
    [SerializeField] private string bootstrapSceneName = "Bootstrap";

    [Header("Громкость по умолчанию")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 0.5f;

    private const string PrefVolume = "music_volume";
    private const string PrefMute = "music_mute";

    private AudioSource audioSource;

    public float Volume => audioSource.volume;
    public bool IsMuted => audioSource.mute;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Восстанавливаем сохранённые настройки громкости и mute
        audioSource.volume = PlayerPrefs.GetFloat(PrefVolume, defaultVolume);
        audioSource.mute = PlayerPrefs.GetInt(PrefMute, 0) == 1;
    }

    private void Start()
    {
        // Если стартовали не из Bootstrap (прямой запуск сцены в редакторе) —
        // запустим музыку для текущей сцены сразу.
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    // -------------------------------------------------------------------------
    // Публичный API (используется VolumeUIBinder-ом из сцены)
    // -------------------------------------------------------------------------

    public void SetVolume(float value)
    {
        audioSource.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PrefVolume, audioSource.volume);
    }

    public void SetMuted(bool muted)
    {
        audioSource.mute = muted;
        PlayerPrefs.SetInt(PrefMute, muted ? 1 : 0);
    }

    // -------------------------------------------------------------------------
    // Приватные методы
    // -------------------------------------------------------------------------

    private void PlayMusicForScene(string sceneName)
    {
        // В Bootstrap-сцене музыки нет — следующая сцена включит её сама.
        if (sceneName == bootstrapSceneName) return;

        AudioClip targetClip = sceneName == menuSceneName ? menuMusic : gameMusic;

        if (targetClip == null || audioSource.clip == targetClip) return;

        audioSource.clip = targetClip;
        audioSource.Play();
    }
}
