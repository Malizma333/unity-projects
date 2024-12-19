using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField]
    public float moveSpeed = 5.0f;

    private Vector2 cameraVelocity = new Vector2(4f, 0.25f);

    [SerializeField]
    private CharacterController controller;

    private Vector2 previousInput;

    [SerializeField]
    private Character character;

    public GameObject mesh;

    public Transform lookObject;

    private Controls Controls;

    public CinemachineVirtualCamera virtualCamera;

    private CinemachineTransposer transposer;

    private Controls controls
    {
        get
        {
            if (Controls != null) { return Controls; }
            return Controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        virtualCamera.gameObject.SetActive(true);

        enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();

        controls.Player.Camera.performed += ctx => Look(ctx.ReadValue<Vector2>());

        controls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => CancelMove();

        controls.Player.Vault.performed += ctx => CheckVault();
    }

    [Client]
    private void Move(Vector2 move)
    {
        previousInput = move;
    }

    [Client]
    private void CheckVault()
    {
        Debug.Log("Check if vaulting is available, if so teleport player to top of box");

        GameObject[] Cubes = new GameObject[] {
            GameObject.Find("Cube Small"),
            GameObject.Find("Cube Medium"),
            GameObject.Find("Cube Large"),
            GameObject.Find("Cube Huge")
        };
        
        Vector3 SMTop = Vector3.zero;
        float SMDistx, SMDistz, minDist = 1.2f;
        int g = 0;

        for(int i = 0; i < 4; i++) {
            if(controller.transform.position.y < Cubes[i].transform.localScale.y) {
                float cubePlayerDistance = Vector3.Distance(Cubes[i].transform.position, controller.transform.position)/(i+4);
                if(cubePlayerDistance < minDist) {
                    minDist = cubePlayerDistance;
                    g = i;
                }
            }
        }

        if(minDist < 0.5) {
            SMDistx = Cubes[g].transform.position.x - controller.transform.position.x;
            SMDistz = Cubes[g].transform.position.z - controller.transform.position.z;
            SMTop = new Vector3(SMDistx, Cubes[g].transform.localScale.y, SMDistz);
        }

        controller.Move(SMTop);
    }

    [Client]
    private void CancelMove()
    {
        previousInput = Vector2.zero;
    }

    [ClientCallback]
    private void Update()
    {
        MovePlayer();
    }

    [Client]
    private void MovePlayer()
    {
        Vector3 right = virtualCamera.transform.right;

        Vector3 forward = virtualCamera.transform.forward;

        right.y = 0;
        forward.y = 0;


        Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y;
        Vector3 gravity = new Vector3(0, -9.8f, 0);

        mesh.transform.LookAt(transform.position + movement, Vector3.up);

        controller.Move(movement * moveSpeed * Time.deltaTime);
        controller.Move(gravity * Time.deltaTime);

        if(movement.sqrMagnitude >= 0.25f)
        {
            GetComponentInChildren<Animator>().SetBool("Walking", true);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("Walking", false);
        }
    }

    [ClientCallback]    
    private void OnEnable()
    {
        controls.Enable();        
    }

    [ClientCallback]
    private void OnDisable()
    {
        controls.Disable();
    }

    private void Look(Vector2 look)
    {

        float deltatime = Time.deltaTime;

        //transposer.m_FollowOffset.y = Mathf.Clamp(transposer.m_FollowOffset.y - (look.y * 0.25f * deltatime),-1f,6f);

    }

 
}

