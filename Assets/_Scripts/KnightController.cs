using UnityEngine;
using UnityEngine.AI;


public class KnightController : MonoBehaviour
{
    private int left_mouse_index = 0;

    private int state__idle = 0;
    private int state__walk = 1;

    public Camera cam;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private GameObject knight;

    private Vector3 lastPosition;
    [SerializeField]
    private float dispSpeed;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        knight = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(left_mouse_index))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        if(dispSpeed >= 0.1f)
        {
            anim.SetInteger("state", state__walk);
        }
        else
        {
            anim.SetInteger("state", state__idle);
        }
    }

    void FixedUpdate()
    {
        dispSpeed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        lastPosition = transform.position;
    }
}
