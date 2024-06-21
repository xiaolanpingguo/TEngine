using Lockstep.Framework;
using TEngine;
using UnityEngine;


namespace Lockstep.Game
{
    [Update]
    public class OldInputManager : BehaviourSingleton<OldInputManager>
    {
        private int _floorMask;

        public bool IsReplay = false;
        public float camRayLength = 100;

        public bool hasHitFloor;
        public LVector2 mousePos;
        public LVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        public static PlayerInput1 CurGameInput = new PlayerInput1();

        public override void Start()
        {
            _floorMask = LayerMask.GetMask("Floor");
        }

        public override void Update()
        {
            if (World.Instance != null && !IsReplay)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                inputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                isInputFire = Input.GetButton("Fire1");
                hasHitFloor = Input.GetMouseButtonDown(1);
                if (hasHitFloor)
                {
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    if (Physics.Raycast(camRay, out floorHit, camRayLength, _floorMask))
                    {
                        mousePos = floorHit.point.ToLVector2XZ();
                    }
                }

                skillId = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (Input.GetKey(KeyCode.Keypad1 + i))
                    {
                        skillId = i + 1;
                    }
                }

                isSpeedUp = Input.GetKeyDown(KeyCode.Space);
                CurGameInput = new PlayerInput1()
                {
                    mousePos = mousePos,
                    inputUV = inputUV,
                    isInputFire = isInputFire,
                    skillId = skillId,
                    isSpeedUp = isSpeedUp,
                };
            }
        }
    }
}