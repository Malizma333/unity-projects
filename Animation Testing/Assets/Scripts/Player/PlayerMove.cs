using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotateSpeed;

    [Space(10)]
    [SerializeField] private Transform raycastObject;

    private float moveSpeed;
    private int ledgeCallCount;
    private Vector3 moveDir;

    private CharacterController controller;
    private Animator anim;

    public float MoveSpeed { get { return moveSpeed; } }

    private void Start()
    {
        ledgeCallCount = 0;
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (anim.GetBool("IsGrounded") && !anim.GetBool("IsHanging") && !anim.GetBool("NearLedge"))
        {
            Move();
            Rotate();
        }
    }

    private void Move()
    {
        float moveZ = Input.GetAxis("Vertical");

        moveDir = transform.TransformDirection(moveZ * Vector3.forward);

        if (moveZ != 0)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Run();
            }
            else
            {
                Walk();
            }
        }
        else
        {
            Idle();
        }

        moveDir *= moveSpeed;

        CheckEdge(moveZ);

        controller.Move(moveDir * Time.deltaTime);
    }

    private void CheckEdge(float movement)
    {
        LayerMask layerMask = GetComponent<PlayerJump>().GroundMask;
        Vector3 castDir = Vector3.down;

        if (!Physics.Raycast(raycastObject.position, castDir, 1f, layerMask))
        {
            if(movement > 0)
            {
                ledgeCallCount++;
                
                if(ledgeCallCount > 1)
                {
                    anim.SetBool("NearLedge", true);
                    ledgeCallCount = 0;
                    return;
                }
            }

            moveDir *= 0;
        }
    }

    private void Rotate()
    {
        float moveX = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up,moveX);
    }

    private void Idle()
    {
        moveSpeed = 0;
        anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }
}
