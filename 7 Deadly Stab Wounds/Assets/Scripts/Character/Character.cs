using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField]
    public CharacterType characterType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public enum CharacterType
    {
        HUMAN,
        MONSTER
    }
}
