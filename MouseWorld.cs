using System;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    
    private static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    
    public static event EventHandler<Vector3> OnMousePositionChanged;
    private static Vector3 lastMousePosition;

    private void Awake()
    {
        instance = this;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        Vector3 currentPosition = raycastHit.point;

        if (currentPosition != lastMousePosition)
        {
            lastMousePosition = currentPosition;
            OnMousePositionChanged?.Invoke(null, currentPosition);
        }

        return currentPosition;
    }
    
}
