using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class KnightController__ManualCtrl : MonoBehaviour
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
    ToroidNavigator toroidNavigator;
    private Animator anim;

    int forwardInput;
    int rotationInput;
    bool navigationKeyPressed;

    float navigationToroidalAngle;
    float navigationToroidalGearNum;
    bool navigationToroidalControlActive;

    Transform _transform;
    Rigidbody _rigidBody;

    private void Start()
    {
        var obj = GameObject.Find(HardcodedValues.toroidalNavigationButton);
        if (obj)
        {
            toroidNavigator = obj.GetComponent<ToroidNavigator>();
        }

        anim = GetComponent<Animator>();
        knight = GetComponent<GameObject>();

        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
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
        if (dispSpeed >= 0.1f)
        {
            anim.SetInteger("state", state__walk);
        }
        else
        {
            anim.SetInteger("state", state__idle);
        }

    }

    TorNavCalc.EnNavStat enNavStat = TorNavCalc.EnNavStat.Idle;
    int forward = 0;
    private void HandleUserInputs()
    {
        DoUserInputs.HandleKeyboard(out forwardInput, out rotationInput, out navigationKeyPressed);

        if (null != toroidNavigator)
        {
            DoUserInputs.HandleToroidNavigator(ref toroidNavigator, out navigationToroidalAngle, out navigationToroidalGearNum, out navigationToroidalControlActive);
        }

        if (!navigationToroidalControlActive)
        {
            TorNavCalc.ResetStateMachine(ref enNavStat);
        }
        else
        {
            //PrintDebugLog.PrintDebug(string.Format("In enNavStat = {0} ", (int)enNavStat));

            int rotatioSpeed = 5;
            int moveSpeed = 5;

            TorNavCalc.HandleToroidTouchNavigation__KeepDirection(ref _transform,
                    ref enNavStat,
                    navigationToroidalAngle,
                    forward,
                    out int outForward,
                    out int rotate
                );

            forward = outForward;

            //Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * rotatioSpeed * rotate * Time.deltaTime);
            //_rigidBody.MoveRotation(wantedRotation);
            _transform.Rotate(Vector3.up, rotate);


            //move tank
            Vector3 wantedPosition = _transform.position + (_transform.forward * forward * moveSpeed * navigationToroidalGearNum * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            //PrintDebugLog.PrintDebug(string.Format("forward = {0}, angle = {1}, Position {2} | {3} | {4}, enNavStat = {5} ", forward, rotate, wantedPosition.x, wantedPosition.y, wantedPosition.z, (int)enNavStat));
        }
    }

    public void SetCamera(Camera cam)
    {
        this.cam = cam;
    }

    public void SetActive(bool isActive)
    {
        transform.gameObject.SetActive(isActive);
    }
}
