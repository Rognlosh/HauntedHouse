using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет музыкой на протяжении всей игры.
///
/// Особенности:
///   • Префаб должен лежать в Assets/Resources/MusicManager.prefab.
///   • Инстанцируется автоматически до загрузки первой сцены
///     (RuntimeInitializeOnLoadMethod), поэтому музыка играет и при
///     прямом запуске уровня в редакторе — без необходимости держать
///     MusicManager на каждой сцене.
///   • Не зависит от UI: громкостью и mute-флагом управляет MenuAudioBinder.
///   • Громкость и mute сохраняются в PlayerPrefs.
///
/// Транзитные сцены (Bootstrap) — на них музыка не меняется,
/// текущий трек продолжает играть или молчать.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const string PrefabResourcePath = "MusicManager";
    private const string VolumePrefKey = "MusicManager.Volume";
    private const string MutedPrefKey = "MusicManager.Muted";

    [Header("Треки")]
    [Tooltip("Музыка главного меню")]
    [SerializeField] private AudioClip menuMusic;
    [Tooltip("Музыка игрового уровня")]
    [SerializeField] private AudioClip gameMusic;

    [Header("Ссылки")]
    [SerializeField] private AudioSource audioSource;

    [Header("Сцены")]
    [Tooltip("Имя сцены с главным меню — на ней играет menuMusic")]
    [SerializeField] private string menuSceneName = "MainMenu";
    [Tooltip("Сцены, на которых музыка не должна меняться " +
             "(текущий трек продолжает играть). Полезно для Bootstrap.")]
    [SerializeField] private string[] transitSceneNames = { "Bootstrap" };

    [Header("Дефолты")]
    [Tooltip("Стартовая громкость, если в PlayerPrefs ничего не сохранено")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 0.7f;

    public float Volume => audioSource != null ? audioSource.volume : 0f;
    public bool IsMuted => audioSource != null && audioSource.mute;

    public event System.Action<float> VolumeChanged;
    public event System.Action<bool> MutedChanged;

    // -------------------------------------------------------------------------
    // Автоспавн до загрузки первой сцены
    // -------------------------------------------------------------------------

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoSpawn()
    {
        if (Instance != null) return;

        var prefab = Resources.Load<GameObject>(PrefabResourcePath);
        if (prefab == null)
        {
            Debug.LogError(
                $"[MusicManager] Префаб не найден по пути " +
                $"Assets/Resources/{PrefabResourcePath}.prefab. " +
                $"Музыка играть не будет.");
            return;
        }

        Instantiate(prefab);
    }

    // -------------------------------------------------------------------------
    // Жизненный цикл
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

        if (audioSource == null)
        {
            Debug.LogError("[MusicManager] AudioSource не назначен.");
            return;
        }

        // Восстанавливаем настройки из PlayerPrefs
        audioSource.volume = PlayerPrefs.GetFloat(VolumePrefKey, defaultVolume);
        audioSource.mute = PlayerPrefs.GetInt(MutedPrefKey, 0) == 1;
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
    // Публичное API
    // -------------------------------------------------------------------------

    /// <summary>Установить громкость [0..1].</summary>
    public void SetVolume(float value)
    {
        if (audioSource == null) return;
        value = Mathf.Clamp01(value);
        audioSource.volume = value;
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        VolumeChanged?.Invoke(value);
    }

    /// <summary>Заглушить или включить звук. true = заглушено.</summary>
    public void SetMuted(bool muted)
    {
        if (audioSource == null) return;
        audioSource.mute = muted;
        PlayerPrefs.SetInt(MutedPrefKey, muted ? 1 : 0);
        MutedChanged?.Invoke(muted);
    }

    // -------------------------------------------------------------------------
    // Приватные методы
    // -------------------------------------------------------------------------

    private void PlayMusicForScene(string sceneName)
    {
        // Транзитные сцены — не трогаем текущее воспроизведение
        if (transitSceneNames != null)
        {
            foreach (var transit in transitSceneNames)
            {
                if (sceneName == transit) return;
            }
        }

        if (audioSource == null) return;

        AudioClip targetClip = sceneName == menuSceneName ? menuMusic : gameMusic;
        if (targetClip == null) return;

        // Если уже играет нужный трек — ничего не делаем
        if (audioSource.clip == targetClip && audioSource.isPlaying) return;

        audioSource.clip = targetClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
