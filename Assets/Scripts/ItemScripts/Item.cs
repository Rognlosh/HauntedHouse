using UnityEngine;

/// <summary>
/// Носимый предмет (Wand, Key и т.п.).
/// Подбирается только если слот игрока пуст. Может давать иммунитет.
/// Логика "только что брошенного" — в базовом Pickupable.
/// </summary>
public class Item : Pickupable
{
    [Header("Свойства предмета")]
    [Tooltip("Индекс иконки в массиве Hud Items (HUDManager). Wand = 0, Key = 1, ...")]
    [SerializeField] private int itemIndex;

    [Tooltip("Защищает ли этот предмет игрока от врагов")]
    [SerializeField] private bool grantsImmunity = false;

    public bool GrantsImmunity => grantsImmunity;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // -------------------------------------------------------------------------
    // Pickupable
    // -------------------------------------------------------------------------

    protected override bool CanBeTakenBy(GameObject player)
    {
        return PlayerStatuses.Instance.CanPickUpItem();
    }

    protected override void TakeBy(GameObject player)
    {
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
        spriteRenderer.enabled = false;

        PlayerStatuses.Instance.EquipItem(this);
        HUDManager.Instance.ShowItemOnHUD(itemIndex);
    }

    // -------------------------------------------------------------------------
    // Сброс — вызывается из PlayerStatuses
    // -------------------------------------------------------------------------

    public void DropItem()
    {
        transform.SetParent(null);
        spriteRenderer.enabled = true;
        MarkAsDropped();

        HUDManager.Instance.HideItemFromHUD(itemIndex);
    }
}