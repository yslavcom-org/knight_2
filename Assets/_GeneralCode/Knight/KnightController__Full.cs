using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class KnightController__Full : MonoBehaviour
{
    private int left_mouse_index = 0;

    private int state__idle = 0;
    private int state__walk = 1;

    
    private Camera cam;
    private GameObject knight;

    private Vector3 lastPosition;
    [SerializeField]
    private float dispSpeed;


    //player controls
    private UnityEngine.AI.NavMeshAgent agent;
    ToroidNavigator toroidNavigator;
    private Animator anim;

    int forwardInput;
    int rotationInput;
    bool navigationKeyPressed;

    float navigationToroidalAngle;
    float navigationToroidalGearNum;
    bool navigationToroidalControlActive;

    private void Start()
    {
        var obj = GameObject.Find(HardcodedValues.toroidalNavigationButton);
        if (obj)
        {
            toroidNavigator = obj.GetComponent<ToroidNavigator>();
        }

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        knight = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        HandleUserInputs();
    }

    void FixedUpdate()
    {
        //Move();

        dispSpeed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        lastPosition = transform.position;
    }


    private void Move()
    {
        EventSystem eventSystem = EventSystem.current;

        bool boAndroidOrIphone = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
        if (boAndroidOrIphone)
        {
            //touch screen touching
            if (!eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            //        && eventSystem.currentSelectedGameObject != null) /*for android, touch screen*/
            {
                if (Input.touchCount > 0 && Input.touchCount < 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        RaycastHit hit;
                        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                        if (Physics.Raycast(ray, out hit))
                        {
                            agent.SetDestination(hit.point);
                        }
                    }
                }
            }
        }
        else if (!boAndroidOrIphone
            && !EventSystem.current.IsPointerOverGameObject() /*non-android, mouse*/)
        {
            //mouse click
            if (Input.GetMouseButtonDown(left_mouse_index))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);

                }
            }

        }


        if (dispSpeed >= 0.1f)
        {
            anim.SetInteger("state", state__walk);
        }
        else
        {
            anim.SetInteger("state", state__idle);
        }

    }

    private void HandleUserInputs()
    {
        DoUserInputs.HandleKeyboard(out forwardInput, out rotationInput, out navigationKeyPressed);

        if (null == toroidNavigator)
        {
            DoUserInputs.HandleToroidNavigator(ref toroidNavigator, out navigationToroidalAngle, out navigationToroidalGearNum, out navigationToroidalControlActive);
        }
    }

    public void AssignCamera(Camera cam)
    {
        this.cam = cam;
    }
}
