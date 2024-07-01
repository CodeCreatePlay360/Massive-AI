using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CodeCreatePlay.Utils
{
	public class MouseUtils : MonoBehaviour
	{
		public CursorLockMode cursorLockMode = CursorLockMode.Confined;
		
		
		void Start()
		{
			Cursor.lockState = cursorLockMode;
		}

		void Update()
		{
			if(Cursor.lockState != cursorLockMode)
				Cursor.lockState = cursorLockMode;
		}
	}
}
