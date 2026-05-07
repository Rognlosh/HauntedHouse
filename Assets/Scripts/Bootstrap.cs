using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Точка входа в игру. Сцена с этим компонентом должна быть нулевой
/// в Build Settings — Unity всегда грузит её первой.
///
/// Задача:
///   • объекты, которые должны жить между сценами (MusicManager и т.п.),
///     помечаются DontDestroyOnLoad;
///   • после этого сразу загружается основная сцена меню — Bootstrap-сцена
///     при этом выгружается, но persistent-объекты остаются.
///
/// Как использовать:
///   1. На Bootstrap-сцене лежит один пустой GameObject с этим компонентом.
///   2. Туда же кладутся все объекты-кандидаты на DontDestroyOnLoad
///      (MusicManager, SettingsManager и пр.).
///   3. Эти объекты перетаскиваются в массив persistentObjects в инспекторе.
///   4. firstScene — имя сцены, которая должна загрузиться сразу после
///      bootstrap-а (по умолчанию MainMenu).
///
/// Если объект сам себе DontDestroyOnLoad в Awake (как MusicManager) —
/// добавлять его в массив всё равно безопасно: DDOL идемпотентен.
/// </summary>
public class Bootstrap : MonoBehaviour
{
    [Header("Что грузить дальше")]
    [Tooltip("Имя сцены, которая откроется сразу после bootstrap-а")]
    [SerializeField] private string firstScene = "MainMenu";

    [Header("Persistent-объекты")]
    [Tooltip("Объекты этой сцены, которые должны пережить смену сцен. " +
             "Они автоматически получают DontDestroyOnLoad.")]
    [SerializeField] private GameObject[] persistentObjects;

    private void Awake()
    {
        foreach (var obj in persistentObjects)
        {
            if (obj == null) continue;
            DontDestroyOnLoad(obj);
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(firstScene))
        {
            Debug.LogError("[Bootstrap] Не указано имя стартовой сцены (firstScene).");
            return;
        }

        SceneManager.LoadScene(firstScene);
    }
}
