using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 _cameraLimitX;
    [SerializeField] private Vector2 _cameraLimitY;

    private Vector3 _touchStartPoint;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (IsPointerOverUIObject())// || IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0)) _touchStartPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = _touchStartPoint - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (transform.position.z > _cameraLimitY.x && transform.position.z < _cameraLimitY.y)
            {
                _touchStartPoint.z += direction.z;
            }
            
            direction.z *= 2f;
            Vector3 newPosition = transform.position + direction;
            
            newPosition.x = Mathf.Clamp(newPosition.x, _cameraLimitX.x, _cameraLimitX.y);
            newPosition.z = Mathf.Clamp(newPosition.z, _cameraLimitY.x, _cameraLimitY.y);
            newPosition.y = 10;

            transform.position = newPosition;
        }
    }

    private bool IsPointerOverUIObject() => EventSystem.current.IsPointerOverGameObject();

    private bool IsPointerOverGameObject()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray);
    }
}