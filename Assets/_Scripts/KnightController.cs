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
    public GameObject projectile;
    public GameObject explosion;
    public Transform bulletSpawn;
    public float projectileForce = 500f;
    public float fireRate = .25f;
    private float nextFireTime;


    public const int array_of_explosions__size = 5;
    private GameObject[] array_of_explosions = new GameObject[array_of_explosions__size];
    private int array_of_explosions__idx;

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

    void FixedUpdate()
    {
        dispSpeed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        for(int idx = 0; idx < array_of_explosions.Length; idx++)
        {
            if (array_of_explosions[idx])
            {
                var texScript = array_of_explosions[idx].GetComponent<AnimatedTextureUV>();
                if (texScript)
                {
                    texScript.TrackCamera(cam);
                }
            }
        }
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
            var cloneRb = Instantiate(projectile, bulletSpawn.position, Quaternion.identity) as GameObject;
            //Vector3 spawn = projectileForce * ( new Vector3(bulletSpawn.position.x, 10f, bulletSpawn.position.z));
            //cloneRb.AddForce(spawn);

            StartCoroutine(ExplosionBoom(cloneRb));

            nextFireTime = Time.time + fireRate;
        }
    }


    IEnumerator ExplosionBoom(GameObject gameObject)
    {
        yield return 1000;

        var newRotation = Quaternion.identity;// Quaternion.LookRotation(-cam.transform.position) * Quaternion.Euler(0, 0, -90);
        GameObject cloneExplosion = Instantiate(explosion, gameObject.transform.position, newRotation) as GameObject;

        if(cloneExplosion)
        {
            if(array_of_explosions__idx >= array_of_explosions.Length)
            {
                array_of_explosions__idx = 0;
            }
            array_of_explosions[array_of_explosions__idx++] = cloneExplosion;
        }
    }
}
