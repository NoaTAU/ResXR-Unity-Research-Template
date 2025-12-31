using Unity.Mathematics;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private Quaternion originalOrientation;
    private bool isRotated = false;

    private void Awake()
    {
        originalOrientation = transform.rotation;
    }

    public void Rotate180Degrees()
    {

        transform.rotation = transform.rotation * Quaternion.Euler(0, math.radians(180f), 0);
        isRotated = !isRotated;
    }

}
