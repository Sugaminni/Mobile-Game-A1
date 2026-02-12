using UnityEngine;

public class TopDownFollow : MonoBehaviour
{
    public Transform target;
    public float height = 18f;

    // Follows player
    void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, height, target.position.z);
    }
}
