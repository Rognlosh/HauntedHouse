using UnityEngine;

/// <summary>
/// Сумка с собранным сокровищем, которую игрок бросил на пол.
/// Хранит количество частей и визуально отображает их:
/// при N частях активны первые N объектов из partVisuals.
/// При подборе части возвращаются игроку, объект уничтожается.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DroppedTreasure : Pickupable
{
    [Header("Визуал")]
    [Tooltip("Дочерние объекты со спрайтами частей. Размер массива = максимум частей.")]
    [SerializeField] private GameObject[] partVisuals;

    private int parts;

    /// <summary>
    /// Задаёт количество частей и обновляет визуал.
    /// Вызывается GameManager-ом сразу после Instantiate.
    /// Помечает сумку как только что сброшенную — нельзя подобрать пока игрок не отошёл.
    /// </summary>
    public void Initialize(int partsCount)
    {
        parts = partsCount;

        for (int i = 0; i < partVisuals.Length; i++)
            partVisuals[i].SetActive(i < parts);

        MarkAsDropped();
    }

    // -------------------------------------------------------------------------
    // Pickupable
    // -------------------------------------------------------------------------

    protected override bool CanBeTakenBy(GameObject player)
    {
        return PlayerStatuses.Instance.CanPickUpDroppedBag();
    }

    protected override void TakeBy(GameObject player)
    {
        PlayerStatuses.Instance.AddTreasureParts(parts);
        Destroy(gameObject);
    }
}