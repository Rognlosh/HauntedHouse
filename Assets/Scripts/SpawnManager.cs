using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Отвечает за подготовку позиций и размещение частей сокровища на сцене.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Тайлмапы")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap stairsTilemap;

    [Header("Настройки")]
    [SerializeField] private float minDistanceBetweenParts = 5f;

    private const float FloorOffsetX = 17f;

    private List<Vector3> preparedPositions = new List<Vector3>();

    /// <summary>
    /// Собирает все пригодные позиции на сцене и перемешивает их.
    /// Вызывается GameManager-ом перед спавном сокровищ.
    /// </summary>
    public void PrepareSpawnPositions()
    {
        var allPositions = new List<Vector3>();
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin + (int)FloorOffsetX; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);

                bool isGround = groundTilemap.HasTile(cell);
                bool isWall = wallsTilemap.HasTile(cell);
                bool isStairs = stairsTilemap.HasTile(cell);

                if (isGround && !isWall && !isStairs)
                {
                    Vector3 worldPos = groundTilemap.CellToWorld(cell) + groundTilemap.tileAnchor;
                    allPositions.Add(worldPos);
                }
            }
        }

        Shuffle(allPositions);

        preparedPositions.Clear();
        foreach (var pos in allPositions)
        {
            bool tooClose = false;
            foreach (var selected in preparedPositions)
            {
                if (Vector3.Distance(pos, selected) < minDistanceBetweenParts)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose) preparedPositions.Add(pos);
        }

        Debug.Log($"[SpawnManager] Подготовлено {preparedPositions.Count} позиций для спавна.");
    }

    /// <summary>Размещает заданное количество частей сокровища на подготовленных позициях.</summary>
    public void SpawnTreasureParts(int numberOfParts)
    {
        for (int i = 0; i < numberOfParts; i++)
        {
            if (preparedPositions.Count == 0)
            {
                Debug.LogWarning("[SpawnManager] Нет свободных позиций для спавна сокровища.");
                return;
            }

            int index = Random.Range(0, preparedPositions.Count);
            Vector3 spawnPos = preparedPositions[index];
            preparedPositions.RemoveAt(index);

            GameManager.Instance.TreasureSetAndActivate(i, spawnPos);
            Debug.Log($"[SpawnManager] Сокровище {i} размещено на {spawnPos}");
        }
    }

    /// <summary>
    /// Телепортирует все части сокровища в ряд — для отладки.
    /// Вызывается кнопкой в редакторе.
    /// </summary>
    public void TP_Treasures()
    {
        GameObject[] parts = GameManager.Instance.PartsOfTreasure;
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].transform.position = new Vector3(i, -2.45f, 0);
            Debug.Log($"[SpawnManager] Сокровище {i} → {parts[i].transform.position}");
        }
    }

    /// <summary>Перемешивает список случайным образом (алгоритм Фишера-Йейтса).</summary>
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}