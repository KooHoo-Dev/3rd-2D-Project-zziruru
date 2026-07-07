using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private int sortingOffset;
    [SerializeField] private int precision = 100;

    private void Awake()
    {
        if (sortingGroup == null)
            sortingGroup = GetComponentInChildren<SortingGroup>();

        if (sortingGroup != null) return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        int order = Mathf.RoundToInt(-transform.position.y * precision) + sortingOffset;
        
        if (sortingGroup != null)
            sortingGroup.sortingOrder = order;
        else if (spriteRenderer != null)
            spriteRenderer.sortingOrder = order;
    }
}
