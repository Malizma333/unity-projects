using UnityEngine;

public class CenterCam : MonoBehaviour
{
    [SerializeField] private GenerateGrid generator;

    private float margin = 3.5f;

    private void Start()
    {
        int totalCells = generator.GridLength;

        transform.position = new Vector3(totalCells / 2, -totalCells / 2, -10);
        GetComponent<Camera>().orthographicSize = totalCells / 2 + margin;
    }
}
