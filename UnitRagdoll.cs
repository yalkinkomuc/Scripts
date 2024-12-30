using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, transform.position, 350f, 10f);
    }

    private void MatchAllChildTransforms(Transform root , Transform clone )
    {
        foreach (Transform child in root)
        {
          Transform cloneChild = clone.Find(child.name);

          if (cloneChild != null)
          {
              cloneChild.position = child.position;
              cloneChild.rotation = child.rotation;
              
              MatchAllChildTransforms(child, cloneChild);
          }
        }
    }

private void ApplyExplosionToRagdoll(Transform root,Vector3 explosionPosition, float explosionForce, float explosionRadius)
{
    foreach (Transform child in root)
    {
        if (child.TryGetComponent(out Rigidbody childRigidbody))
        {
            childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        }

        ApplyExplosionToRagdoll(child, explosionPosition, explosionForce, explosionRadius);
    }
}

}
