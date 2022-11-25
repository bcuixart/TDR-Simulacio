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

    float holdingShiftSpeedMultiplier;
    bool holdingMouseWheel;

    private void Start()
    {
        // Initialize the correct initial rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        gameMana = GameManager.instance;
    }

    private void Update()
    {
        holdingShiftSpeedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        holdingMouseWheel = Input.GetMouseButton(2);

        //Girar
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeedH * Input.GetAxis("Mouse X");
            pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        //Moure
        float axisSpeedH = (holdingMouseWheel ? 1 : 0) * -Input.GetAxis("Mouse X") + Input.GetAxisRaw("Horizontal") * 0.5f;
        transform.Translate(axisSpeedH * Time.unscaledDeltaTime * dragSpeed * holdingShiftSpeedMultiplier, 
            (holdingMouseWheel ? 1 : 0) * -Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime * dragSpeed * holdingShiftSpeedMultiplier, 
            0);

        //Avançar
        float axisSpeed = Input.GetAxis("Mouse ScrollWheel") + Input.GetAxisRaw("Vertical") * 0.035f;
        transform.Translate(0, 0, Time.unscaledDeltaTime * axisSpeed * zoomSpeed * holdingShiftSpeedMultiplier, Space.Self);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (gameMana.individuSeleccionat == null)
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