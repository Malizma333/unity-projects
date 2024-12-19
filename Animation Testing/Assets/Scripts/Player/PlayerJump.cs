using System.Collections;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeight;

    [Space(10)]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;

    private readonly float gravity = -9.81f;
    private Vector3 gravityVelocity;
    private Vector3 target;

    private float yVelocity;
    private float lastYPos;
    private float moveDist;
    [SerializeField] private bool onGround;
    private bool landRecorded;

    private CharacterController controller;
    private Animator anim;

    public LayerMask GroundMask { get { return groundMask; } }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        lastYPos = transform.position.y;
    }

    private void Update()
    {
        Jump();

        if (!anim.GetBool("IsHanging") && !anim.GetBool("NearLedge"))
        {
            ApplyForce();
        }

        RecordVelocity();
    }

    private void Jump()
    {
        onGround = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (onGround && anim.GetBool("IsFalling"))
        {
            StartCoroutine(LandSequence());
        }
        
        if(anim.GetBool("IsGrounded") && !anim.GetBool("IsJumping") && !anim.GetBool("IsHanging") && !anim.GetBool("NearLedge"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDist = GetComponent<PlayerMove>().MoveSpeed;

                StartCoroutine(JumpSequence());
            }
        }
    }

    private void ApplyForce()
    {
        gravityVelocity.y += gravity * Time.deltaTime;
        controller.Move(gravityVelocity * Time.deltaTime);

        if(!onGround)
        {
            controller.Move(target * Time.deltaTime);
        }
    }

    private void RecordVelocity()
    {
        yVelocity = transform.position.y - lastYPos;
        lastYPos = transform.position.y;

        if (!landRecorded)
        {
            if (Physics.CheckSphere(transform.position, groundCheckDistance + 1f, groundMask))
            {
                float lv = yVelocity * -3;

                if (lv > 1)
                    lv = 1;

                anim.SetFloat("LandVelocity", lv);
                landRecorded = true;
            }
        }
    }

    private IEnumerator JumpSequence()
    {
        anim.SetBool("IsGrounded", false);
        anim.SetBool("IsJumping", true);

        float myTime = 0.543f / 0.75f;

        yield return new WaitForSeconds(myTime);

        gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        target = transform.forward * moveDist;

        anim.SetBool("IsJumping", false);

        yield return new WaitUntil(() => yVelocity < 0);

        anim.SetBool("IsFalling", true);
        landRecorded = false;
    }

    private IEnumerator LandSequence()
    {
        anim.SetBool("IsFalling", false);
        anim.SetBool("IsLanding", true);

        float myTime = 0.4f / 1f;

        yield return new WaitForSeconds(myTime);

        anim.SetBool("IsLanding", false);
        anim.SetBool("IsDropping", false);
        anim.SetBool("IsGrounded", true);
    }
}
