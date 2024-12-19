using System.Collections;
using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    [SerializeField] private float castDistance;
    [SerializeField] private Transform raycastObject;
    [SerializeField] private Transform followObject;

    private Animator anim;
    private Vector3 target;
    private Vector3 initialFollow;

    private bool climbingUp = false;
    private bool climbingDown = false;

    private void Start()
    {
        initialFollow = followObject.transform.localPosition;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(anim.GetBool("IsFalling") && !anim.GetBool("IsDropping") && !anim.GetBool("IsHanging"))
        {
            CheckLedge();
        }

        if (anim.GetBool("IsHanging") && !anim.GetBool("IsClimbing"))
        {
            Climb();
        }

        if(anim.GetBool("NearLedge"))
        {
            GetOffLedge();
        }

        if(anim.GetBool("IsClimbing"))
        {
            followObject.position = 
                Vector3.MoveTowards(followObject.position, target + initialFollow, Time.deltaTime / 1.5f);
        }
    }

    private void CheckLedge()
    {
        RaycastHit hit;
        Ray ray = new Ray(raycastObject.position, raycastObject.forward);
        LayerMask layerMask = GetComponent<PlayerJump>().GroundMask;

        if (Physics.Raycast(ray, out hit, 0.45f, layerMask))
        {
            if (!hit.transform.CompareTag("MovingPlatform"))
            {
                if (!Physics.Raycast(raycastObject.position + Vector3.up * 0.1f, raycastObject.forward, 0.3f, layerMask))
                {
                    anim.SetBool("IsHanging", true);
                    target = raycastObject.position + raycastObject.forward * 0.6f;
                }
            }
        }
    }

    private void Climb()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!climbingUp) {
                climbingUp = true;
                StartCoroutine(ClimbUp());
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetBool("IsHanging", false);
            anim.SetBool("IsDropping", true);
        }
    }

    private void GetOffLedge()
    {
        transform.position += Time.deltaTime * 0.1f * transform.forward;

        if (!climbingDown)
        {
            climbingDown = true;
            StartCoroutine(ClimbDown());
        }
    }

    private IEnumerator ClimbUp()
    {
        anim.SetBool("IsClimbing", true);

        yield return new WaitForSeconds(3.5f);

        anim.SetFloat("Speed", 0);

        anim.SetBool("IsClimbing", false);

        transform.position = target;
        followObject.localPosition = initialFollow;

        yield return new WaitForSeconds(0.1f);

        anim.SetBool("IsHanging", false);
        climbingUp = false;
    }

    private IEnumerator ClimbDown()
    {
        transform.position += Vector3.down * 0.75f;

        yield return new WaitForSeconds(1.5f);

        anim.SetBool("NearLedge", false);
        anim.SetBool("IsGrounded", false);
        anim.SetBool("IsFalling", true);

        climbingDown = false;
    }
}
