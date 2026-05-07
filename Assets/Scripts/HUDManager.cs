using UnityEngine;

/// <summary>
/// Управляет интерфейсом: иконки сокровищ в слоте игрока, иконки предметов,
/// меню победы/поражения. Подписывается на GameEvents.
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Меню")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;

    [Header("HUD — Сокровища (то что в слоте у игрока)")]
    [Tooltip("Иконка пустого слота — видна когда игрок ничего не несёт")]
    [SerializeField] private GameObject hudNothingCollected;
    [Tooltip("Иконки частей. Будут зажигаться по очереди: 0..count-1")]
    [SerializeField] private GameObject[] hudTreasures;

    [Header("HUD — Предметы")]
    [SerializeField] private GameObject hudItemUncollected;
    [SerializeField] private GameObject[] hudItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnGameWon += HandleGameWon;
        GameEvents.OnGameLost += HandleGameLost;
        GameEvents.OnPlayerBagChanged += HandlePlayerBagChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnGameWon -= HandleGameWon;
        GameEvents.OnGameLost -= HandleGameLost;
        GameEvents.OnPlayerBagChanged -= HandlePlayerBagChanged;
    }

    // -------------------------------------------------------------------------
    // Обработчики
    // -------------------------------------------------------------------------
    private void HandleGameStarted()
    {
        if (loseMenu != null) loseMenu.SetActive(false);
        if (winMenu != null) winMenu.SetActive(false);

        if (hudNothingCollected != null) hudNothingCollected.SetActive(true);
        foreach (var icon in hudTreasures) icon.SetActive(false);

        if (hudItemUncollected != null) hudItemUncollected.SetActive(true);
        foreach (var icon in hudItems) icon.SetActive(false);
    }

    private void HandleGameWon() { if (winMenu != null) winMenu.SetActive(true); }
    private void HandleGameLost() { if (loseMenu != null) loseMenu.SetActive(true); }

    /// <summary>
    /// Зажигает первые N иконок сокровищ.
    /// Если count=0 — все гаснут и появляется иконка пустого слота.
    /// </summary>
    private void HandlePlayerBagChanged(int count)
    {
        bool hasAny = count > 0;
        if (hudNothingCollected != null)
            hudNothingCollected.SetActive(!hasAny);

        for (int i = 0; i < hudTreasures.Length; i++)
            hudTreasures[i].SetActive(i < count);
    }

    // -------------------------------------------------------------------------
    // API для предметов
    // -------------------------------------------------------------------------
    public void ShowItemOnHUD(int itemIndex)
    {
        if (hudItemUncollected != null) hudItemUncollected.SetActive(false);

        if (itemIndex >= 0 && itemIndex < hudItems.Length)
            hudItems[itemIndex].SetActive(true);
    }

    public void HideItemFromHUD(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < hudItems.Length)
            hudItems[itemIndex].SetActive(false);

        if (hudItemUncollected != null) hudItemUncollected.SetActive(true);
    }
}