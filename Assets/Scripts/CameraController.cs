using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Скорость перемещения камеры
    public Vector2 panLimitX; // Ограничения по оси X
    public Vector2 panLimitY;  // Ограничения по оси Y
    public Vector2 panLimitZ;  // Ограничения по оси Y

    private Vector3 touchStart;

    void Update()
    {
        if (IsPointerOverUIObject())// || IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            print("down");
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (transform.position.z > panLimitY.x && transform.position.z < panLimitY.y)
            {
                touchStart.z += direction.z;
            }
            
            direction.z *= 2f;
            Vector3 newPosition = transform.position + direction;
            
            newPosition.x = Mathf.Clamp(newPosition.x, panLimitX.x, panLimitX.y);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitY.x, panLimitY.y);
            newPosition.y = 10;

            transform.position = newPosition;
        }
    }

    private bool IsPointerOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsPointerOverGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray);
    }
}