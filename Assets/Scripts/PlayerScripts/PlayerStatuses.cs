using UnityEngine;

/// <summary>
/// Состояние игрока: единый слот инвентаря (сокровище или предмет),
/// иммунитет от врагов, обработка кнопки сброса.
/// Слот всегда в одном из трёх состояний:
///   • пустой
///   • содержит N частей сокровища
///   • содержит один Item (палку, ключ и т.п.)
/// </summary>
public class PlayerStatuses : MonoBehaviour
{
    public static PlayerStatuses Instance { get; private set; }

    [Header("Управление")]
    [SerializeField] private KeyCode dropKey = KeyCode.RightShift;

    // -------------------------------------------------------------------------
    // Состояние слота
    // -------------------------------------------------------------------------
    public int TreasurePartsHeld { get; private set; }
    public Item HeldItem { get; private set; }

    public bool SlotIsEmpty => TreasurePartsHeld == 0 && HeldItem == null;
    public bool IsHoldingTreasure => TreasurePartsHeld > 0;
    public bool IsHoldingItem => HeldItem != null;

    /// <summary>Иммунитет даётся предметом (палкой). Без предмета — уязвим.</summary>
    public bool IsImmune => IsHoldingItem && HeldItem.GrantsImmunity;

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
    }

    private void OnEnable() => GameEvents.OnGameStarted += ResetState;
    private void OnDisable() => GameEvents.OnGameStarted -= ResetState;

    private void Update()
    {
        if (!GameManager.Instance.GameIsActive) return;

        if (Input.GetKeyDown(dropKey))
            DropWhateverIsHeld();
    }

    // -------------------------------------------------------------------------
    // API: подбор
    // -------------------------------------------------------------------------

    /// <summary>Может ли игрок принять одну часть сокровища с пола.</summary>
    public bool CanPickUpTreasurePart() => !IsHoldingItem;

    /// <summary>Может ли игрок принять предмет (Wand, Key и т.п.).</summary>
    public bool CanPickUpItem() => SlotIsEmpty;

    /// <summary>Может ли игрок поднять сброшенную сумку с N частями.</summary>
    public bool CanPickUpDroppedBag() => !IsHoldingItem;

    /// <summary>Добавить одну часть сокровища в слот.</summary>
    public void AddTreasurePart()
    {
        TreasurePartsHeld++;
        GameEvents.PlayerBagChanged(TreasurePartsHeld);
    }

    /// <summary>Добавить N частей в слот (при подборе сброшенной сумки).</summary>
    public void AddTreasureParts(int count)
    {
        TreasurePartsHeld += count;
        GameEvents.PlayerBagChanged(TreasurePartsHeld);
    }

    /// <summary>Положить предмет в слот.</summary>
    public void EquipItem(Item item)
    {
        HeldItem = item;
    }

    // -------------------------------------------------------------------------
    // Сброс по кнопке
    // -------------------------------------------------------------------------
    private void DropWhateverIsHeld()
    {
        if (IsHoldingItem)
        {
            HeldItem.DropItem();
            HeldItem = null;
        }
        else if (IsHoldingTreasure)
        {
            GameManager.Instance.SpawnDroppedTreasure(transform.position, TreasurePartsHeld);
            TreasurePartsHeld = 0;
            GameEvents.PlayerBagChanged(0);
        }
    }

    // -------------------------------------------------------------------------
    // Сброс при старте/рестарте игры
    // -------------------------------------------------------------------------
    private void ResetState()
    {
        TreasurePartsHeld = 0;
        HeldItem = null;
        GameEvents.PlayerBagChanged(0);
    }
}