using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    //use it as a singleton
    public class TankControllerPlayer : MyTankGame.TankControllerAny
    {
        public static TankControllerPlayer Instance { get; private set; }
        private void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                base.Init(null);
            }
        }

        void Awake()
        {
            Initialize();
        }



        //it's a singleton
        private TankControllerPlayer(Camera trackCamera, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
        : base(trackCamera, pos , rot , scale)
        {

        }


    }
}
