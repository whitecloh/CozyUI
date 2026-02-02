using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI_Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GridLayoutGroup))]
    public sealed class AdaptiveSquareGridColumns : MonoBehaviour
    {
        [Header("Columns")]
        [SerializeField, Min(1)] private int phoneColumns = 2;
        [SerializeField, Min(1)] private int tabletColumns = 3;

        [Header("Tablet detection")]
        [Tooltip("If min screen size in dp is >= this value, use tabletColumns.")]
        [SerializeField, Min(1f)] private float tabletMinDp = 600f;

        [Tooltip("Fallback when DPI is unknown (0). If min side px >= this value, treat as tablet.")]
        [SerializeField, Min(1)] private int tabletMinSidePxFallback = 1200;

        private GridLayoutGroup _grid;
        private RectTransform _rectTransform;

        private int _lastColumns = -1;
        private float _lastWidth = -1f;

        private void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rectTransform = transform as RectTransform;

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        }

        private void OnEnable()
        {
            Apply();
        }

        private void OnRectTransformDimensionsChange()
        {
            Apply();
        }

        private void Apply()
        {
            if (_grid == null || _rectTransform == null)
                return;

            int columns = IsTablet() ? tabletColumns : phoneColumns;
            columns = Mathf.Max(1, columns);

            float width = _rectTransform.rect.width;
            if (width <= 0.01f)
                return;

            if (_lastColumns == columns && Mathf.Abs(_lastWidth - width) < 0.5f)
                return;

            _lastColumns = columns;
            _lastWidth = width;

            _grid.constraintCount = columns;

            float available = width - _grid.padding.left - _grid.padding.right - _grid.spacing.x * (columns - 1);
            float cell = available / columns;

            if (cell < 1f)
                cell = 1f;

            _grid.cellSize = new Vector2(cell, cell);

            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
        }

        private bool IsTablet()
        {
            float dpi = Screen.dpi;

            if (dpi > 0f)
            {
                float minPixels = Mathf.Min(Screen.width, Screen.height);
                float minDp = (minPixels / dpi) * 160f;
                return minDp >= tabletMinDp;
            }

            int minSidePx = Mathf.Min(Screen.width, Screen.height);
            return minSidePx >= tabletMinSidePxFallback;
        }
    }
}
