using UnityEngine;

public class GunScript : MonoBehaviour
{
    private Camera fpsCam;
    public float speed = 10f;
    public float range = 100f;
    public int damageAmount = 1;
    public float hitForce = 3000f;

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

    void Shoot()
    {
        var camera_position = fpsCam.transform.position;
        var forward_camera_position = fpsCam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(camera_position, forward_camera_position, out hit, range))
        {
            Debug.DrawLine(camera_position, hit.point);
            Debug.Log(hit.transform.name);


            ShootableBox target = hit.transform.GetComponent<ShootableBox>();
            if (null != target)
            {
                target.Damage(damageAmount);
            }

            if(hit.rigidbody)
            {
                Debug.Log("add force"+ hit.rigidbody.name);
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }
        }

    }

}

