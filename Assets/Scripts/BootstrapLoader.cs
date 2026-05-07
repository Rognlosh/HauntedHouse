using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Стартовая сцена приложения. Грузит MainMenu сразу после инициализации
/// persistent-менеджеров (MusicManager и т.п.), которые лежат в этой же сцене
/// и помечают себя DontDestroyOnLoad в своих Awake.
///
/// Сценарий запуска:
///   Bootstrap.Awake (всех менеджеров) → DontDestroyOnLoad
///   BootstrapLoader.Start → SceneManager.LoadScene(MainMenu)
///   Bootstrap-сцена выгружается, менеджеры остаются жить
/// </summary>
public class BootstrapLoader : MonoBehaviour
{
    [Tooltip("Имя сцены главного меню. Должна быть в Build Settings.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
