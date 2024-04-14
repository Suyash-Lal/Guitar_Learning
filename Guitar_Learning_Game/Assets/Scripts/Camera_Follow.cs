using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    private float follow_speed = 3.5f;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -30f);
        transform.position = Vector3.Slerp(transform.position, newPos, follow_speed*Time.deltaTime);
    }
}
