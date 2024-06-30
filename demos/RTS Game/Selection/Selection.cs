using System.Collections.Generic;
using UnityEngine;


namespace CodeCreatePlay.Selection
{	
    public class Selection : MonoBehaviour
    {
		public LayerMask selectableMask;
        public LayerMask groundMask;
		
        public Camera mainCam;
		
		public List<GameObject> allEntities { get; private set; } = new();
		
		[SerializeField]
		private List<GameObject> selectedEntities = new();
		
        public List<GameObject> SelectedEntities
		{
			get{ return selectedEntities; }
			set{ selectedEntities = value; }
		}


        private void Start()
        {
            Entity[] foundEntities = FindObjectsOfType<Entity>();
			
            for (int i = 0; i < foundEntities.Length; i++)
                allEntities.Add(foundEntities[i].gameObject);
        }

        public void ClickSelect(GameObject objSelected)
        {
            DeselectAll();
            selectedEntities.Add(objSelected);
        }

        public void ShiftClickSelect(GameObject entityToSelect)
        {
            if (!selectedEntities.Contains(entityToSelect))
            {
                selectedEntities.Add(entityToSelect);
            }
            else
            {
                selectedEntities.Remove(entityToSelect);
            }
        }

        public void DragSelect(GameObject itemToAdd)
        {
            if (!selectedEntities.Contains(itemToAdd))
            {
                selectedEntities.Add(itemToAdd);
            }
        }

        public void SetSelected(GameObject entity = null, List<GameObject> entities = null)
        {
        }

        public void DeselectAll()
        {
            selectedEntities.Clear();
        }

        public void Deselect(GameObject entity=null, List<GameObject> entities=null)
        {
            for (int i = 0; i < allEntities.Count; i++)
            {
                if (entity != null && allEntities[i] == entity)
                {
                    allEntities.RemoveAt(i);
                    continue;
                }

                if(entities != null && i < entities.Count && allEntities[i] == entities[i])
                {
                    allEntities.RemoveAt(i);
                    continue;
                }

                if (i > entities.Count && entity == null)
                    break;
            }
        }
    }
}
