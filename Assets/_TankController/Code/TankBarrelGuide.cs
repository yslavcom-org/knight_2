using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankBarrelGuide : MonoBehaviour
    {
        PlayerTurretControl m_PlayerTurretControl;

        private void Start()
        {
            var parent = transform.parent;

            while (null != parent)
            {
                if (null != parent.GetComponent<PlayerTurretControl>())
                {
                    m_PlayerTurretControl = parent.GetComponent<PlayerTurretControl>();
                    if (null != m_PlayerTurretControl) break; // break the while loop
                }
                else
                {
                    parent = parent.transform.parent;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                m_PlayerTurretControl.BarrelUpSetter(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                m_PlayerTurretControl.BarrelUpSetter(false);
            }
        }
    }
}
