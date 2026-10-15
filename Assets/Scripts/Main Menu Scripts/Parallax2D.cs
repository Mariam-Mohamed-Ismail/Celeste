using UnityEngine;

public class Parallax2D : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
