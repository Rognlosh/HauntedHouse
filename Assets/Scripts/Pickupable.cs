using UnityEngine;

/// <summary>
/// Базовый класс для всего что игрок может подобрать с пола.
/// Решает общую задачу: только что брошенный предмет нельзя подобрать обратно,
/// пока игрок не сошёл с него и не вернулся.
///
/// Наследники переопределяют:
///   • CanBeTakenBy — условие подбора (пуст ли слот, включён ли фонарик и т.п.)
///   • TakeBy       — что произойдёт при подборе
/// и при необходимости вызывают MarkAsDropped() сразу после "сброса".
/// </summary>
public abstract class Pickupable : MonoBehaviour
{
    private bool wasDropped;

    // Эти методы Unity дёргает сама — приватные, чтобы наследники
    // не пытались их переопределить и не ломали общую логику.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (wasDropped) return;
        if (!CanBeTakenBy(collision.gameObject)) return;

        TakeBy(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Игрок отошёл — снова можно подобрать
        if (collision.CompareTag("Player"))
            wasDropped = false;
    }

    /// <summary>
    /// Помечает объект как только что сброшенный.
    /// Вызывается из DropItem() в Item или из Initialize() в DroppedTreasure.
    /// </summary>
    protected void MarkAsDropped()
    {
        wasDropped = true;
    }

    /// <summary>Можно ли сейчас подобрать этот объект (например, есть ли место в слоте).</summary>
    protected abstract bool CanBeTakenBy(GameObject player);

    /// <summary>Что происходит при подборе.</summary>
    protected abstract void TakeBy(GameObject player);
}