using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class InputHandler : MonoBehaviour
    {
        //Variables
        float vertical;
        float horizontal;
        /// <summary>
        /// b_input
        /// </summary>
        bool runInput;

        /// <summary>
        /// Left mouse click
        /// </summary>
        bool rb_input;
        /// <summary>
        /// X
        /// </summary>
        bool x;
        /// <summary>
        /// C
        /// </summary>
        bool a;
        /// <summary>
        /// Y
        /// </summary>
        bool y;
        /// <summary>
        /// B
        /// </summary>
        bool b;
        /// <summary>
        /// R key
        /// </summary>
        bool rt_input;
        /// <summary>
        /// Q key
        /// </summary>
        bool lt_input;
        /// <summary>
        /// Z key
        /// </summary>
        bool lb_input;

        bool toLeft;
        bool locking;

        float run_timer;
        float rt_timer;
        float lt_timer;

        float delta;

        StateManager states;
        CameraManager camManager;
        KeyCode k_run=KeyCode.LeftShift,
                k_x= KeyCode.X,
                k_c= KeyCode.C,
                k_y= KeyCode.Y,
                k_b= KeyCode.B,
                k_r= KeyCode.R,
                k_q= KeyCode.Mouse1,
                k_z= KeyCode.Z,
                k_g= KeyCode.G,
                k_mouse0=KeyCode.Mouse0;

        // Use this for initialization
        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();

            camManager = CameraManager.singleton;
            camManager.Init(states);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();

            states.FixedTick(delta);
            camManager.FixedTick(delta);

        }

        void Update()
        {
            delta = Time.deltaTime;

            states.Tick(delta);

            ResetInputNStates();
        }


        /// <summary>
        /// Get input data
        /// </summary>
        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            runInput = Input.GetKey(k_run);
            rt_input = Input.GetKey(k_r);
            lt_input = Input.GetKey(k_q);
            lb_input = Input.GetKey(k_z);
            rb_input = Input.GetKey(k_mouse0);
            a = Input.GetKey(k_c);
            b = Input.GetKey(k_b);
            y = Input.GetKey(k_y);
            x = Input.GetKey(k_x);

            locking = Input.GetKeyUp(k_g);

            if (runInput)
            {
                run_timer += delta;
            }
        }

        /// <summary>
        /// Update data in StateManager
        /// </summary>
        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = states.vertical * camManager.transform.forward;
            Vector3 h = states.horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;

            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (runInput && run_timer>0.5f)
            {
                if (states.moveAmount > 0)
                {
                    states.run = true;
                }
            }

            if(!runInput && run_timer > 0 && run_timer < .5f)
            {
                states.rollInput = true;
            }

            states.rb = rb_input;
            states.rt = rt_input;
            states.lt = lt_input;
            states.lb = lb_input;

            if (y && states.inventoryManager.curWeapon.switchable)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (locking)
            {
                states.lockOn = !states.lockOn;
                if (states.lockOnTarget == null)
                {
                    states.lockOn = false;
                }

                camManager.lockonTarget = states.lockOnTarget;
                states.lockOnTransform = states.lockOnTarget.targetLook;
                camManager.lockon = !camManager.lockon;
            }
        }

        void ResetInputNStates()
        {
            if (!runInput)
                run_timer = 0;
            if (states.rollInput)
                states.rollInput = false;
            if (states.run)
                states.run = false;
        }
    }
}
