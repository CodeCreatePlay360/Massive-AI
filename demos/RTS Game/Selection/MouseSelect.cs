using UnityEngine;


namespace CodeCreatePlay.Selection
{
    public class MouseSelect : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
				Selection selection = GetComponent<Selection>();
				
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
									out RaycastHit hitInfo,
									Mathf.Infinity,
									selection.selectableMask))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        selection.ShiftClickSelect(hitInfo.collider.gameObject);
                    else
                        selection.ClickSelect(hitInfo.collider.gameObject);
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                        selection.DeselectAll();
                }
            }
        }
    }
}
