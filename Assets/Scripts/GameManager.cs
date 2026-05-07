using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет состоянием игры: старт, победа, поражение, этаж, спавн сокровищ.
/// Активирует выход когда все исходные части сокровища были подняты с пола
/// хотя бы раз. Победа определяется в ExitTrigger.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Ссылки")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private GameObject exitZone;

    [Header("Сокровища")]
    [SerializeField] private int numberOfTreasureParts = 3;
    [Tooltip("Префаб сумки, которая появляется когда игрок бросает сокровище")]
    [SerializeField] private DroppedTreasure droppedTreasurePrefab;
    public GameObject[] PartsOfTreasure;

    // -------------------------------------------------------------------------
    // Состояние
    // -------------------------------------------------------------------------
    public bool GameIsActive { get; private set; }
    public int CurrentFloor { get; set; }
    public bool IsPlayerInside { get; set; }
    public bool FlashlightOn { get; set; }

    public int NumberOfTreasureParts => numberOfTreasureParts;

    private int treasurePartsTakenFromMap;

    // -------------------------------------------------------------------------
    // Unity
    // -------------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (spawnManager == null)
            spawnManager = FindFirstObjectByType<SpawnManager>();
    }

    private void Start() => StartGame();

    // -------------------------------------------------------------------------
    // Управление игрой
    // -------------------------------------------------------------------------

    public void StartGame()
    {
        CurrentFloor = 1;
        treasurePartsTakenFromMap = 0;
        GameIsActive = true;
        FlashlightOn = false;

        exitZone.SetActive(false);

        Player.Instance.PlayerStartPoint();
        spawnManager.PrepareSpawnPositions();
        spawnManager.SpawnTreasureParts(numberOfTreasureParts);

        // Уничтожаем все оставшиеся сброшенные сумки от прошлой игры
        foreach (var leftover in FindObjectsByType<DroppedTreasure>(FindObjectsSortMode.None))
            Destroy(leftover.gameObject);

        GameEvents.GameStarted();
        Debug.Log($"[GameManager] Игра началась. Сокровищ: {numberOfTreasureParts}");
    }

    public void WinGame()
    {
        if (!GameIsActive) return;
        GameIsActive = false;
        GameEvents.GameWon();
    }

    public void LoseGame()
    {
        if (!GameIsActive) return;
        GameIsActive = false;
        GameEvents.GameLost();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // -------------------------------------------------------------------------
    // Сокровища
    // -------------------------------------------------------------------------

    /// <summary>
    /// Вызывается из Treasure при первом подборе с пола.
    /// Когда все исходные части собраны хотя бы раз — открывает выход.
    /// </summary>
    public void NotifyTreasurePartTaken()
    {
        treasurePartsTakenFromMap++;
        Debug.Log($"[GameManager] Поднято с пола: {treasurePartsTakenFromMap}/{numberOfTreasureParts}");

        if (treasurePartsTakenFromMap >= numberOfTreasureParts)
        {
            exitZone.SetActive(true);
            GameEvents.AllTreasuresCollected();
            Debug.Log("[GameManager] Все части собраны — выход открыт!");
        }
    }

    /// <summary>Создаёт сумку со сброшенным сокровищем в указанной точке.</summary>
    public void SpawnDroppedTreasure(Vector3 position, int parts)
    {
        if (droppedTreasurePrefab == null)
        {
            Debug.LogError("[GameManager] Не назначен droppedTreasurePrefab в инспекторе!");
            return;
        }

        var bag = Instantiate(droppedTreasurePrefab, position, Quaternion.identity);
        bag.Initialize(parts);
        Debug.Log($"[GameManager] Сброшена сумка с {parts} частями на {position}");
    }

    /// <summary>Размещает сокровище в сцене. Вызывается SpawnManager-ом.</summary>
    public void TreasureSetAndActivate(int i, Vector3 spawnPos)
    {
        PartsOfTreasure[i].SetActive(true);
        PartsOfTreasure[i].transform.position = spawnPos;
    }
}