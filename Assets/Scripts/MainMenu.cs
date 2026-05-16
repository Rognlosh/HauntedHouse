using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject chooseLevelMenu;

    [Tooltip("Имя сцены игрового уровня")]
    [SerializeField] private string levelSceneName = "HountedHouseLvl1";

    public void ChooseLevelStart()
    {
        chooseLevelMenu.SetActive(true);
        startMenu.SetActive(false);
    }

    public void LoadEasy()
    {
        StartLevel(Difficulty.Easy);
    }

    public void LoadMedium()
    {
        StartLevel(Difficulty.Medium);
    }

    public void LoadHard()
    {
        StartLevel(Difficulty.Hard);
    }

    /// <summary>
    /// Старое имя, оставлено для совместимости с кнопкой EasyModeButton,
    /// которая в инспекторе сцены ссылается на LoadEasyScene. После
    /// переподключения кнопки на LoadEasy эту обёртку можно удалить.
    /// </summary>
    public void LoadEasyScene()
    {
        LoadEasy();
    }

    private void StartLevel(Difficulty difficulty)
    {
        GameSettings.SetByMenu(difficulty);
        SceneManager.LoadScene(levelSceneName);
    }
}
