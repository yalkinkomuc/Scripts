using System;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable All

public class UnitActionSystem : MonoBehaviour
{
   public static UnitActionSystem Instance {get; private set;}
   
   [SerializeField] private Unit selectedUnit;
   private BaseAction selectedAction;
   
   private bool isBusy;
   
   private MovementRangeVisualizer currentRangeVisualizer;
   private MoveAction currentMoveAction;
   
   #region EventHandlers

   public event EventHandler OnSelectedUnitChanged;
   public event EventHandler OnSelectedActionChanged;
   public event EventHandler <bool> OnBusyChanged;
   public event EventHandler OnActionStarted;
   

   #endregion
   
   #region LayerMasks

   [SerializeField] private LayerMask unitLayerMask;
   #endregion
 
  
   private void Awake()
   {


      if (Instance != null)
      {
         Debug.LogError("More than one instance of UnitActionSystem found!"+transform+" - " + Instance);
         Destroy(gameObject);
         return;
      }
        
        Instance = this;
   }


   private void Start()
   {
      SetSelectedUnit(selectedUnit);
   }

   private void Update()
   {
      if (isBusy || !TurnSystem.instance.IsPlayerTurn() || EventSystem.current.IsPointerOverGameObject())
      {
         return;
      }

      // Mouse pozisyonunu kontrol et ve path'i güncelle
      if (selectedAction is MoveAction moveAction)
      {
         Vector3 mousePosition = MouseWorld.GetMouseWorldPosition();
         moveAction.ShowPath(mousePosition);
         
         if (currentRangeVisualizer == null)
         {
            currentRangeVisualizer = selectedUnit.GetComponent<MovementRangeVisualizer>();
            if (currentRangeVisualizer == null)
            {
               currentRangeVisualizer = selectedUnit.gameObject.AddComponent<MovementRangeVisualizer>();
            }
         }
         
         float maxRange = moveAction.GetMaxMovementPoints() / moveAction.GetMovementCostPerUnit();
         currentRangeVisualizer.ShowRange(maxRange);
      }

      if (Input.GetMouseButtonDown(0))
      {
         if (TryHandleUnitSelection())
         {
            return;
         }
         HandleSelectedAction();
      }
   }

   #region UnitSelection

   private void HandleSelectedAction()
   {
      if (selectedUnit == null || selectedAction == null)
      {
         return;
      }

      if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
      {
         return;
      }

      SetBusy();
      selectedAction.TakeAction(MouseWorld.GetMouseWorldPosition(), ClearBusy);
      OnActionStarted?.Invoke(this, EventArgs.Empty);
   }
   
   private bool TryHandleUnitSelection() // kameradan ray çizip mouse pozisyonunda unit var mı diye checkliyoruz.
   {
      if (Input.GetMouseButtonDown(0))
      {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
         {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {
               if (unit == selectedUnit)
               {
                  return false;
               }

               if (unit.IsEnemy())
               {
                  return false;
               }
               
               SetSelectedUnit(unit);
               return true;
            }
         }
      }
      
       
      return false;
   }


   
   
   #endregion

   #region SetBusyManagement

   public void SetBusy()
   {
      isBusy = true;
      OnBusyChanged?.Invoke(this, isBusy);
   }

   public void ClearBusy()
   {
      isBusy = false;
      OnBusyChanged?.Invoke(this, isBusy);
   }

   #endregion
   
   #region GetUnitAndAction

   public Unit GetSelectedUnit()
   {
      return selectedUnit;
   }
   
   public BaseAction GetSelectedAction()
   {
      return selectedAction;
   }

   #endregion

   #region SetUnitAndAction

   private void SetSelectedUnit(Unit unit)
   {
      // Önceki unit'in görsellerini temizle
      if (selectedUnit != null)
      {
         if (currentMoveAction != null)
         {
            currentMoveAction.HidePath();
         }
         if (currentRangeVisualizer != null)
         {
            currentRangeVisualizer.HideRange();
         }
      }

      // Yeni unit'i seç
      selectedUnit = unit;
      SetSelectedAction(unit.GetMoveAction());
      
      OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
   }

   
  

   public void SetSelectedAction(BaseAction baseAction)
   {
      selectedAction = baseAction;

      if (selectedAction is MoveAction moveAction)
      {
         currentMoveAction = moveAction;
         
         // Range visualizer'ı kontrol et ve yoksa ekle
         currentRangeVisualizer = selectedUnit.GetComponent<MovementRangeVisualizer>();
         if (currentRangeVisualizer == null)
         {
            currentRangeVisualizer = selectedUnit.gameObject.AddComponent<MovementRangeVisualizer>();
         }

         float maxRange = moveAction.GetMaxMovementPoints() / moveAction.GetMovementCostPerUnit();
         currentRangeVisualizer.ShowRange(maxRange);
      }
      else
      {
         // Başka bir aksiyon seçildiğinde gösterimleri kapat
         if (currentMoveAction != null)
         {
            currentMoveAction.HidePath();
         }
         if (currentRangeVisualizer != null)
         {
            currentRangeVisualizer.HideRange();
         }
      }

      OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
   }

   #endregion
   
   
}
