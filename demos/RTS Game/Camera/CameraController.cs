using UnityEngine;


namespace CodeCreatePlay.RTSDemo
{	
	using CodeCreatePlay.Utils;
	

    public class CameraController : MonoBehaviour
    {
        [System.Serializable]
        public class PosSettings
        {
            public bool invertPan = true;
            public bool allowZoom = true;

            public float panSmooth = 7f;

            public float zoomSmooth = 5f;
            public float zoomStep = 5f;
            public float maxZoom = 25f;
            public float minZoom = 80f;

            public float distanceToGround = 40f;

            [HideInInspector]
            public float newDistance = 40f;
        }


        [System.Serializable]
        public class OrbitSettings
        {
            public bool allowOrbit_Y = true;

            public float xRotation = 50f;
            public float yRotation = 0f;

            public float orbitSmooth_Y = 0.5f;
        }


        [System.Serializable]
        public class InputSettings
        {
            public string PAN = "MousePAN";
            public string ORBIT_Y = "MouseTurn";
            public string ZOOM = "Mouse ScrollWheel";
        }


        public LayerMask groundMask;
        public PosSettings posSettings = new();
        public OrbitSettings orbitSettings = new();
        public InputSettings inputSettings = new();
		private MouseEdgeDetection mouseEdgeDetection;

        private Vector3 destination = Vector3.zero;
        private Vector3 cameraVel = Vector3.zero;
        private Vector3 prevMousePos = Vector3.zero;
        private Vector3 currentMousePos = Vector3.zero;

        public Vector2 panInput;
        private float orbitInput, zoomInput;

        private int panDirection = 0;


        private void Start()
        {
			mouseEdgeDetection = GetComponent<MouseEdgeDetection>();
			
            panInput = new();
            orbitInput = 0;
            zoomInput = 0;
        }

        private void Update()
        {
            currentMousePos = Input.mousePosition;

            // get input
            GetInput();

            // zoom
            if (posSettings.allowZoom)
                Zoom();

            // orbit
            if (orbitSettings.allowOrbit_Y)
                Rotate();

            // pan
            PanWorld();

            prevMousePos = currentMousePos;
        }

        private void FixedUpdate()
        {
            HandleCamDistance();
        }

        private void GetInput()
        {
            if (Input.GetKey(KeyCode.W))
                panInput.y = 1f;
            else if (Input.GetKey(KeyCode.S))
                panInput.y = -1f;
            else
                panInput.y = 0f;

            if (Input.GetKey(KeyCode.D))
                panInput.x = 1f;
            else if (Input.GetKey(KeyCode.A))
                panInput.x = -1f;
            else
                panInput.x = 0f;

            orbitInput = Input.GetAxis(inputSettings.ORBIT_Y);
            zoomInput  = Input.GetAxis(inputSettings.ZOOM);
        }

        private void PanWorld()
        {
            Vector3 targetPos = transform.position;

            if (posSettings.invertPan)
                panDirection = -1;
            else
                panDirection = 1;

            targetPos += posSettings.panSmooth * panInput.x * Time.deltaTime *
                transform.right;

            targetPos += posSettings.panSmooth * panInput.y * Time.deltaTime *
                Vector3.Cross(transform.right, Vector3.up);

            transform.position = targetPos;
        }

        private void HandleCamDistance()
        {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 100f, groundMask))
            {
                destination = Vector3.Normalize(transform.position - hitInfo.point) * posSettings.distanceToGround;
                destination += hitInfo.point;

                transform.position = Vector3.SmoothDamp(transform.position, destination, ref cameraVel, 0.35f);
            }
        }

        private void Zoom()
        {
            posSettings.newDistance += posSettings.zoomStep * -zoomInput;
            posSettings.distanceToGround = Mathf.Lerp(posSettings.distanceToGround, posSettings.newDistance, posSettings.zoomSmooth * Time.deltaTime);

            //posSettings.distanceToGround = Mathf.Clamp(posSettings.distanceToGround, posSettings.minZoom, posSettings.maxZoom);
            //posSettings.newDistance = Mathf.Clamp(posSettings.newDistance, posSettings.minZoom, posSettings.maxZoom);
        }

        private void Rotate()
        {
            if(orbitInput > 0)
                orbitSettings.yRotation += (currentMousePos.x - prevMousePos.x) * orbitSettings.orbitSmooth_Y * Time.deltaTime;

            transform.rotation = Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation, 0);
        }
    }
}
