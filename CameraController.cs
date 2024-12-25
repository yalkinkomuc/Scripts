using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; // Kameranın hareket hızı
    [SerializeField] private float rotationSpeed = 100f; // Kameranın döndürme hızı

    private bool isRightClickHeld = false; // Sağ tık basılı mı kontrolü

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Sağ tık basılı tutulduğunda hareketi etkinleştir
        if (Input.GetMouseButtonDown(1))
        {
            isRightClickHeld = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightClickHeld = false;
        }

        if (isRightClickHeld)
        {
            MoveCamera();
            RotateCamera();
        }
    }


    #region CameraController
    
    private void MoveCamera()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A ve D tuşları
        float vertical = Input.GetAxis("Vertical");     // W ve S tuşları

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection); // Kameranın yönüne göre hareket et
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X"); // Mouse'un X ekseni
        float mouseY = Input.GetAxis("Mouse Y"); // Mouse'un Y ekseni

        // Yalnızca sağ tık basılıyken kamerayı döndür
        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World); // Y ekseni etrafında döndür
        transform.Rotate(Vector3.left, mouseY * rotationSpeed * Time.deltaTime, Space.Self); // X ekseni etrafında döndür
    }
    #endregion
   
}