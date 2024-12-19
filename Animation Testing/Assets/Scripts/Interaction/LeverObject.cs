using UnityEngine;

public class LeverObject : MonoBehaviour
{
    [SerializeField] private MeshRenderer leverMesh;
    [SerializeField] private Material pressedMat;

    private bool toggled = false;

    public bool Toggled { get { return toggled; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!toggled)
            {
                StartCoroutine(AreaPopup.Instance.Display());
            }

            toggled = true;
            leverMesh.material = pressedMat;
        }
    }
}
