using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitRagdollSpawner : MonoBehaviour
{
   [SerializeField] private Transform ragdollPrefab;
   [FormerlySerializedAs("ragdollOriginalRootBone")] [SerializeField] private Transform originalRootBone;
   
   private HealthSystem healthSystem;

   private void Awake()
   {
      healthSystem = GetComponent<HealthSystem>();
      
      healthSystem.OnDead += HealthSystemOnOnDead;
   }

   private void HealthSystemOnOnDead(object sender, EventArgs e)
   {
     Transform ragdollTransform =  Instantiate(ragdollPrefab, transform.position, transform.rotation);
     
     UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
     unitRagdoll.Setup(originalRootBone);
   }
}
