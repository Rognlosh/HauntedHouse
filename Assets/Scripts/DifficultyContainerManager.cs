using UnityEngine;

/// <summary>
/// Управляет видимостью контейнеров на сцене в зависимости
/// от текущей сложности.
///
/// Каждый "слой" — это пустой GameObject в иерархии (например, Difficulty_All,
/// Difficulty_Medium+, Difficulty_Hard), в который сложены объекты, специфичные
/// для определённых сложностей. В инспекторе для каждого слоя ставятся флажки
/// "на каких сложностях он активен". В Awake манагер пробегает по слоям и
/// либо включает их, либо выключает через SetActive.
///
/// [DefaultExecutionOrder(-500)] гарантирует, что Awake этого компонента
/// сработает раньше, чем Awake объектов внутри контейнеров — иначе они успеют
/// инициализироваться и подписаться на события, прежде чем их выключат.
/// </summary>
[DefaultExecutionOrder(-500)]
public class DifficultyContainerManager : MonoBehaviour
{
    [System.Serializable]
    public struct Layer
    {
        [Tooltip("Корень контейнера в иерархии — будет включён/выключен")]
        public GameObject root;

        [Tooltip("Активен на сложности Easy")]
        public bool onEasy;

        [Tooltip("Активен на сложности Medium")]
        public bool onMedium;

        [Tooltip("Активен на сложности Hard")]
        public bool onHard;
    }

    [Tooltip("Список контейнеров. Difficulty_All обычно включён на всех трёх. " +
             "Difficulty_Medium+ — на Medium и Hard. Difficulty_Hard — только на Hard.")]
    [SerializeField] private Layer[] layers;

    private void Awake()
    {
        var difficulty = GameSettings.SelectedDifficulty;

        if (layers == null) return;

        foreach (var layer in layers)
        {
            if (layer.root == null) continue;
            layer.root.SetActive(IsLayerActiveOn(layer, difficulty));
        }
    }

    private static bool IsLayerActiveOn(Layer layer, Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:   return layer.onEasy;
            case Difficulty.Medium: return layer.onMedium;
            case Difficulty.Hard:   return layer.onHard;
            default:                return false;
        }
    }
}
