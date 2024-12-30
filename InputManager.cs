using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event EventHandler<Vector3> OnMousePositionChanged;
    private Vector3 lastMousePosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = MouseWorld.GetMouseWorldPosition();
        
        if (mouseWorldPosition != lastMousePosition)
        {
            lastMousePosition = mouseWorldPosition;
            OnMousePositionChanged?.Invoke(this, mouseWorldPosition);
        }
    }
} 