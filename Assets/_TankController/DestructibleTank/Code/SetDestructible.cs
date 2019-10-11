using UnityEngine;public class SetDestructible : MonoBehaviour
{
    Rigidbody[] rbArray;

    // Start is called before the first frame update
    void Start()
    {
        rbArray = GetComponentsInChildren<Rigidbody>();
    }

    int step = 0;
    float time;
    private void Update()
    {
        switch(step)
        {
            case 0:
                time = Time.time;
                step = 1;
                break;

            case 1:
                if(Time.time -time > 5)
                {
                    BlowUp();
                    step = 2;
                }
                break;

            case 2:
                break;
        }
    }


    void BlowUp()
    {
        //yield return new WaitForSeconds(0.2f);
        foreach (var rb in rbArray)
        {
            if(rb.name != "DestructibleTank")
            {
                rb.isKinematic = false;
            }
           //else
           //{
           //    rb.isKinematic = true;
           //}
        }
    }
}
