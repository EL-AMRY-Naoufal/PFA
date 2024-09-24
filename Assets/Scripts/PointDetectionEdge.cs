using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDetectionEdge : MonoBehaviour
{
    public bool connected;
    public float radius = 0.6f;

    public Collider[] hitCollider;

    private void OnDisable()
    {
        connected = false;
    }

    public void ChekOverlap()
    {
        connected = false;

        hitCollider = Physics.OverlapSphere(transform.position, radius);

        if(hitCollider.Length > 0)
        {
            foreach (var collider in hitCollider)
            {
                if(collider.CompareTag("Point") || collider.CompareTag("Terrain"))
                {
                    connected = true;
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
