using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaultable : MonoBehaviour
{

    [SerializeField]
    public VaultType type;

    public enum VaultType
    {
        LOW,
        MEDIUM,
        HIGH,
        TEAM
    }
}
