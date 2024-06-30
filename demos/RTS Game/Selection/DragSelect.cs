using UnityEngine;


namespace CodeCreatePlay.Selection
{
    public class DragSelect : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;  // visual
        private Rect selectionBox;            // actual bounding test to test if objects are contained within it
        private Vector2 startPos, endPos;     // 


        private void Start()
        {
            startPos = Vector2.zero;
            endPos = Vector2.zero;

            DrawVisual();
        }

        private void Update()
        {
            // when clicked
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                selectionBox = new();
            }

            // when dragging
            if (Input.GetMouseButton(0))
            {
                endPos = Input.mousePosition;
                DrawVisual();
                DrawSelectionRect();
            }

            // when released
            if (Input.GetMouseButtonUp(0))
            {
                SelectUnits();
                startPos = endPos = Vector2.zero;
                DrawVisual();
            }
        }

        private void DrawVisual()
        {
            Vector2 boxStart = startPos;
            Vector2 boxEnd = endPos;

            Vector2 boxCenter = (startPos + endPos) / 2f;
            Vector2 boxSize = new(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            rectTransform.position = boxCenter;
            rectTransform.sizeDelta = boxSize;
        }

        private void DrawSelectionRect()
        {
            // do x calculations
            if (Input.mousePosition.x < startPos.x)
            {
                // dragging left
                selectionBox.xMin = Input.mousePosition.x;
                selectionBox.xMax = startPos.x;
            }
            else
            {
                // dragging right
                selectionBox.xMin = startPos.x;
                selectionBox.xMax = Input.mousePosition.x;
            }

            // do y calculations
            if (Input.mousePosition.y < startPos.y)
            {
                // dragging down
                selectionBox.yMin = Input.mousePosition.y;
                selectionBox.yMax = startPos.y;
            }
            else
            {
                // dragging up
                selectionBox.yMin = startPos.y;
                selectionBox.yMax = Input.mousePosition.y;
            }
        }

        private void SelectUnits()
        {
            Selection selection = GetComponent<Selection>();
			
            for (int i = 0; i < selection.allEntities.Count; i++)
                if (selectionBox.Contains(selection.mainCam.WorldToScreenPoint(selection.allEntities[i].transform.position)))
                    selection.DragSelect(selection.allEntities[i]);
        }
    }
}
