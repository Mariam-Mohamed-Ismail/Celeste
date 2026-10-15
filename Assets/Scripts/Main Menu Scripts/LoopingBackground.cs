using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    private float width;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (transform.position.x < -width)
        {
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}
