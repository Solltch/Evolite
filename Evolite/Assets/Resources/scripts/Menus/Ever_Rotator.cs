using UnityEngine;

public class Ever_Rotator : MonoBehaviour
{
    RectTransform part;

    void Start()
    {
        part = GetComponent<RectTransform>();
    }

    void Update()
    {
        // rotação global
        part.Rotate(0, 100f * Time.deltaTime, 0, Space.World);
    }
}
