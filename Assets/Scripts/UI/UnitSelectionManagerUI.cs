using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] RectTransform selectionAreaRectTransform;
    [SerializeField] Canvas canvas;

    void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager__OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd +=UnitSelectionManager__OnSelectionAreaEnd;

        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    void Update()
    {
        if(selectionAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void UnitSelectionManager__OnSelectionAreaStart(object sender,System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }

    private void UnitSelectionManager__OnSelectionAreaEnd(object sender,System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectedAreaRect();

        float canvasScale = canvas.transform.localScale.x;
        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x,selectionAreaRect.y) / canvasScale;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width,selectionAreaRect.height) / canvasScale;
    }

}
