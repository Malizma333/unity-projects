using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    [SerializeField] private Vector3 localDestination;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private bool oneway = false;

    [Space(10)]
    [SerializeField] private LeverObject[] levers;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool moveTowardsEnd;
    private float timer;

    private void Start()
    {
        timer = 0;
        moveTowardsEnd = true;
        startPosition = transform.position;
        endPosition = transform.position + localDestination;
    }

    private void FixedUpdate()
    {
        if (timer == 0 && isActive())
        {
            Move();
        }
        else
        {
            if (!oneway)
            {
                timer += Time.deltaTime / waitTime;
            }

            if(timer >= 1)
            {
                timer = 0;
            }
        }
    }

    private bool isActive()
    {
        foreach(LeverObject lever in levers)
        {
            if(!lever.Toggled)
            {
                return false;
            }
        }

        return true;
    }

    private void Move()
    {
        if (moveTowardsEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime * speed);

            if (transform.position == endPosition)
            {
                moveTowardsEnd = false;

                timer += Time.deltaTime / waitTime;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * speed);

            if (transform.position == startPosition)
            {
                moveTowardsEnd = true;

                timer += Time.deltaTime / waitTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.parent == transform)
            {
                other.transform.SetParent(null);
                other.transform.localScale = Vector3.one;
            }
        }
    }
}
