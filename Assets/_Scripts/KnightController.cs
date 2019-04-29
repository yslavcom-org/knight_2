using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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


    static Timer TTimer;
    public Rigidbody projectile;
    public GameObject explosion;
    public Transform bulletSpawn;
    public float projectileForce = 500f;
    public float fireRate = .25f;
    private float nextFireTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        knight = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        ThrowProjectile();
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(left_mouse_index))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
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

    void ThrowProjectile()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFireTime)
        {
            Rigidbody cloneRb = Instantiate(projectile, bulletSpawn.position, Quaternion.identity) as Rigidbody;
            //Vector3 spawn = projectileForce * ( new Vector3(bulletSpawn.position.x, 10f, bulletSpawn.position.z));
            //cloneRb.AddForce(spawn);

            StartCoroutine(ExplosionBoom(cloneRb));
            
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        dispSpeed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        lastPosition = transform.position;
    }


    IEnumerator ExplosionBoom(Rigidbody rigidBody)
    {
        yield return 1000;

        var newRotation = Quaternion.LookRotation(-cam.transform.position) * Quaternion.Euler(0, 0, -90);
        GameObject cloneExplosion = Instantiate(explosion, rigidBody.position, newRotation) as GameObject;

        var texScript = GetComponent<AnimatedTextureUV>();
        if (texScript)
        {
            texScript.SetCamera(cam);
        }
    }
}
