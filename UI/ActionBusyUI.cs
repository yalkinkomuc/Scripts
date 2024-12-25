using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] private GameObject actionBusyImage;
    void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_onBusyChanged;
        
        HideActionBusyImage();
    }

   

    private void ShowActionBusyImage()
    {
        actionBusyImage.SetActive(true);
    }

    private void HideActionBusyImage()
    {
        actionBusyImage.SetActive(false);
    }

    private void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            ShowActionBusyImage();
        }
        else
        {
            HideActionBusyImage();
        }
    }
}
