using UnityEngine;

public class GunScript : MonoBehaviour
{
    private Camera fpsCam;
    public float speed = 10f;
    public float range = 100f;
    public int damageAmount = 1;
    public float hitForce = 3000f;

    public Transform gunEnd;


    //recoil
    public Transform RecoilStart;
    public Transform RecoilEnd;
    float recoilSpeed = 1;
    float fireRate = 0.1f;

    float timer;

    enum ShootState{
        enShootState_Idle,
        enShootState_Fire,
        enShootState_Recoil,
    };

    ShootState shootState = ShootState.enShootState_Idle;


    private void Start()
    {
        fpsCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        transform.Translate(straffe, 0, translation);

        if(Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void LateUpdate()
    {
        timer += Time.deltaTime;
        if(shootState == ShootState.enShootState_Fire)
        {
            transform.position = Vector3.MoveTowards(transform.position, RecoilEnd.position, recoilSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, RecoilEnd.rotation, recoilSpeed * Time.deltaTime);
            var goal_rotation = Quaternion.Euler(100, 0, 200);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, goal_rotation, recoilSpeed * Time.deltaTime);

            shootState = ShootState.enShootState_Recoil;
        }
        if (shootState == ShootState.enShootState_Recoil && timer > fireRate)
        {
            shootState = ShootState.enShootState_Idle;

            transform.position = Vector3.MoveTowards(transform.position, RecoilStart.position, recoilSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, RecoilStart.rotation, recoilSpeed * Time.deltaTime);
            var goal_rotation = Quaternion.Euler(0, 0, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, goal_rotation, recoilSpeed * Time.deltaTime);
        }
    }

    void Shoot()
    {
        shootState = ShootState.enShootState_Fire;
        timer = 0;

        var shooter_position = gunEnd.position;//fpsCam.transform.position;
        var forward_shooter_position = gunEnd.forward;//fpsCam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(shooter_position, forward_shooter_position, out hit, range))
        {
            Debug.DrawLine(shooter_position, hit.point);
            Debug.Log(hit.transform.name);


            IDamageable damage = hit.transform.GetComponent<IDamageable>();
            if (null != damage)
            {
                damage.Damage(damageAmount);
            }

            if(hit.rigidbody)
            {
                Debug.Log("add force"+ hit.rigidbody.name);
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }

        }

    }

}

