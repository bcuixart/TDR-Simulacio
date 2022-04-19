using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float lookSpeedH = 2f;

    [SerializeField]
    private float lookSpeedV = 2f;

    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float dragSpeed = 3f;

    public float focusSpeed = 0.125f;
    public Vector3 offset;

    private float yaw = 0f;
    private float pitch = 0f;

    GameManager gameMana;

    private void Start()
    {
        // Initialize the correct initial rotation
        this.yaw = this.transform.eulerAngles.y;
        this.pitch = this.transform.eulerAngles.x;

        gameMana = GameManager.instance;
    }

    private void Update()
    {
        // Only work with the Left Alt pressed
        //if (Input.GetKey(KeyCode.LeftControl))
        //{
            if (Input.GetMouseButton(1))
            {
                this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
                this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

                this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
            }

            //drag camera around with Middle Mouse
            if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            //if (Input.GetMouseButton(1))
            //{
                //Zoom in and out with Right Mouse
               // this.transform.Translate(0, 0, Input.GetAxisRaw("Mouse X") * this.zoomSpeed * .07f, Space.Self);
            //}

            //Zoom in and out with Mouse Wheel
            this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed, Space.Self);
        //}

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(gameMana.individuSeleccionat == null)
            {
                return;
            }

            ZoomTo(gameMana.individuSeleccionat.gameObject);
        }
    }

    public void ZoomTo(GameObject selection)
    {
        transform.position = selection.transform.position + offset;
        transform.LookAt(selection.transform);

        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;
    }
}