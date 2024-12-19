using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private Material colorMat;
    [SerializeField] private LeverObject triggerL;

    private bool set = false;

    private void Update()
    {
        if(triggerL.Toggled && !set)
        {
            set = true;
            GetComponent<MeshRenderer>().material = colorMat;
        }
    }
}
