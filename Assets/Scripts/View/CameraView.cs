using UnityEngine;

public class CameraView : MonoBehaviour
{
    public float dragSpeed = 3.5f;
    public float zoomSpeed = 3.5f;

    private Camera _main;
    private Vector3 _dragOrigin;
    private void Awake()
    {
        _main = Camera.main;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0.0f)
        {
            _main.orthographicSize += Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        }


        if (Input.GetMouseButtonDown(1))
        {
            _dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = -_main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
        _dragOrigin = Input.mousePosition;

        transform.Translate(move, Space.World);
    }

}