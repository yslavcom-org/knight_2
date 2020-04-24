using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyTankGame
{
    public class PlayerTurretControl : MonoBehaviour
    {
        enum TurretState{
            ManualMode,
            SetToIdle, // bring the turret automatically back to idle mode
        };

        [SerializeField]
        TurretState turretState = TurretState.ManualMode;

        #region Turret Variables
        private float rotSpeed = 0.25f;
        #endregion

        #region Barrel Variables
        private float rotSpeedBarrel = 0.25f;

        float maxBarrelUp = 20f;
        float maxBarrelDown = -10f;
        float counter = 0f;

        bool barrelUp = false;
        bool barrelDown = false;
        bool left = false;
        bool right = false;

        public GameObject barrel;
        #endregion

        #region CrossHair
        CrossHairControl crossHair;
        #endregion

        #region Event Listeners
        private UnityAction<object> dismissTurretListener;
        readonly string dissmiss_turret_event_name = HardcodedValues.evntName__dismissTurret;
        #endregion

        #region Dissmiss turret control
        TankControllerPlayer parent;
        float roateSpeedWhenDismissed = 0.001f;
        GameObject dissmissTurretBtn;
        #endregion

        #region Built-in methods
        private void Awake()
        {
            dismissTurretListener = new UnityAction<object>(OnDismissTurret);

            parent = GetComponentInParent<TankControllerPlayer>();

            var buttons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var btn in buttons)
            {
                if (btn.name == "DismissTurretBtn")
                {
                    dissmissTurretBtn = btn.gameObject;
                    break;
                }
            }
        }

        void OnEnable()
        {
            EventManager.StartListening(dissmiss_turret_event_name, OnDismissTurret);
        }

        void OnDisable()
        {
            EventManager.StopListening(dissmiss_turret_event_name, OnDismissTurret);
        }

        void RotateToPosition(ref Quaternion rotation)
        {
            if (transform.rotation != rotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.time * roateSpeedWhenDismissed);
            }
            if (barrel.transform.rotation != rotation)
            {
                barrel.transform.rotation = Quaternion.Lerp(barrel.transform.rotation, rotation, Time.time * roateSpeedWhenDismissed);
            }
        }

        void HumanPointGun()
        {
            if (null != crossHair)
            {
                float navAngle;
                float navRelativeDistance;
                bool boPressed = crossHair.GetPressedDirection(out navAngle, out navRelativeDistance);

                bool temp_left = false;
                bool temp_right = false;
                bool temp_barrelUp = false;
                bool temp_barrelDown = false;

                /*
                 code below will likely change after testing on smartphone
                 */
                if (boPressed && navRelativeDistance >= 0.6 /*0.67*/)
                {
                    turretState = TurretState.ManualMode;

                    //PrintDebugLog.PrintDebug(string.Format("navAngle = {0}, navRelativeDistance = {1}", navAngle, navRelativeDistance));
                    FPSDisplay fpsDisplay = FindObjectOfType<FPSDisplay>();
                    fpsDisplay.SetDebugFloatValue(navRelativeDistance);

                    //left / right
                    if (navAngle >= 15 && navAngle < 165)
                    {
                        temp_right = true;
                    }
                    else if (navAngle >= 165 && navAngle < 345)
                    {
                        temp_left = true;
                    }

                    //up / down
                    if (navAngle >= 285 || navAngle < 75)
                    {
                        temp_barrelUp = true;
                    }
                    else if (navAngle >= 105 && navAngle < 255)
                    {
                        temp_barrelDown = true;
                    }

                }
                left = temp_left;
                right = temp_right;
                barrelUp = temp_barrelUp;
                barrelDown = temp_barrelDown;
            }
        }

        public void SetAutomaticGunPoint( Transform targetTransform)
        {
#if false
            transform.LookAt(transform);
            barrel.transform.LookAt(transform);
#else
            //find the vector pointing from our position to the target
            Vector3 _direction = (targetTransform.position - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation = _lookRotation * Quaternion.Euler(0, 90, 0);

            //rotate us over time according to speed until we are in the required rotation
            const float RotationSpeed = 50f;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _lookRotation , Time.deltaTime * RotationSpeed);


            barrel.transform.localRotation = Quaternion.identity;
#endif

        }


        private void Update()
        {
            HumanPointGun();
        }

        private void FixedUpdate()
        {
            switch (turretState)
            {
                case TurretState.ManualMode:
                {
                    RotateTurret();
                    BarrelUpDown();
                }break;

                case TurretState.SetToIdle:
                default:
                {
                    TurretAndBarrelToIdle();
                }
                break;
            }
        }
        #endregion

        #region Custom methods
        #region Turret Methods
        void RotateTurret()
        {
            if (left)
            {
                EnableDismissBtn(true);
                transform.Rotate(0, -rotSpeed, 0);
            }

            if (right)
            {
                EnableDismissBtn(true);
                transform.Rotate(0, rotSpeed, 0);
            }
        }
        #endregion

        #region Barrel Methods
        public void BarrelUpSetter(bool val) { barrelUp = val; }

        void BarrelUpDown()
        {
            if (barrelUp && counter < maxBarrelUp)
            {
                EnableDismissBtn(true);
                barrel.transform.Rotate(0, 0, -rotSpeedBarrel);
                counter += rotSpeedBarrel;
            }

            if (barrelDown && counter > maxBarrelDown)
            {
                EnableDismissBtn(true);
                barrel.transform.Rotate(0, 0, rotSpeedBarrel);
                counter -= rotSpeedBarrel;
            }
        }
        #endregion

        void TurretAndBarrelToIdle()
        {
            if (null == parent)
            {
                turretState = TurretState.ManualMode;
            }
            else
            {
                Quaternion parent_rotation = GetParentRotation();

                RotateToPosition(ref parent_rotation);
                if (transform.rotation == parent_rotation
                    && barrel.transform.rotation == parent_rotation)
                {
                    turretState = TurretState.ManualMode;
                    EnableDismissBtn(false);

                }
            }
        }

        void EnableDismissBtn(bool boActive)
        {
            if (null == dissmissTurretBtn) return;
            dissmissTurretBtn.SetActive(boActive);
        }

        Quaternion GetParentRotation()
        {
            Quaternion parent_rotation = parent.transform.rotation * Quaternion.AngleAxis(90, Vector3.up);
            return parent_rotation;
        }

        public void SetCrosshair(GameObject crossHair)
        {
            if (null == crossHair) return;
            this.crossHair = crossHair.GetComponent<CrossHairControl>();
        }


        #endregion

        #region Events
        private void OnDismissTurret(object arg)
        {
            turretState = TurretState.SetToIdle;
        }
        #endregion
    }
}
