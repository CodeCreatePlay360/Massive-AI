using UnityEngine;


namespace CodeCreatePlay.Utils
{
	public class MouseEdgeDetection : MonoBehaviour
	{
		public enum EdgeDirection_ { left, right, top, bottom, none }
		
		// private
		[SerializeField]
		private EdgeDirection_ edgeDirection = EdgeDirection_.none;
		
		[SerializeField]
		private Vector2 edgeDirectionVector;
		
		// public
		public EdgeDirection_ EdgeDirection { get { return edgeDirection; } }
		public float edgeThreshold = 15f;  // Threshold in pixels to consider as "edge"

		
		private void Start()
		{
			edgeDirection = EdgeDirection_.none;
			edgeDirectionVector = new Vector2();
			IsMouseAtEdge(Input.mousePosition);
		}
		
		private void Update()
		{
			// Check if mouse is at the edge of the screen
			IsMouseAtEdge(Input.mousePosition);
		}

		private bool IsMouseAtEdge(Vector3 mousePosition)
		{
			edgeDirectionVector.x = 0;
			edgeDirectionVector.y = 0;
			
			if (mousePosition.x <= edgeThreshold)
			{
				edgeDirection = EdgeDirection_.left;
				edgeDirectionVector.x = -1;
				return true;
			}
			if (mousePosition.x >= Screen.width - edgeThreshold)
			{
				edgeDirection = EdgeDirection_.right;
				edgeDirectionVector.x = 1;
				return true;
			}
			if (mousePosition.y <= edgeThreshold)
			{
				edgeDirection = EdgeDirection_.bottom;
				edgeDirectionVector.x = -1;
				return true;
			}
			if (mousePosition.y >= Screen.height - edgeThreshold)
			{
				edgeDirection = EdgeDirection_.top;
				edgeDirectionVector.x = 1;
				return true;
			}

			edgeDirection = EdgeDirection_.none;
			return false;
		}

		private bool IsMouseOnScreen(Vector3 mousePosition)
		{
			return mousePosition.x >= 0 && mousePosition.x <= Screen.width &&
				   mousePosition.y >= 0 && mousePosition.y <= Screen.height;
		}
	}
}
