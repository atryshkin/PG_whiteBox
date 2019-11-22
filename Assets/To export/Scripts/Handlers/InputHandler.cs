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

        bool e;
        /// <summary>
        /// X
        /// </summary>
        bool x;
        /// <summary>
        /// C
        /// </summary>
        bool c;
        /// <summary>
        /// Y
        /// </summary>
        bool y;
        /// <summary>
        /// B
        /// </summary>
        bool b;
        /// <summary>
        /// R
        /// </summary>
        bool r;
        /// <summary>
        /// Q key
        /// </summary>
        bool q;
        /// <summary>
        /// Z key
        /// </summary>
        bool z;
        /// <summary>
        /// F key
        /// </summary>
        bool f;

        bool mouse0;
        bool mouse1;
        bool mouse2;

        bool toLeft;
        bool locking;

        float run_timer;
        float m0_timer;
        float m1_timer;
        float m2_timer;
        float y_timer;

        float delta;

        StateManager states;
        CameraManager camManager;
        KeyCode k_run=KeyCode.LeftShift,
                k_x= KeyCode.X,
                k_c= KeyCode.C,
                k_y= KeyCode.Y,
                k_b= KeyCode.B,
                k_r = KeyCode.R,
                k_f = KeyCode.F,
                k_q = KeyCode.Q,
                k_mouse2 = KeyCode.Mouse2,
                k_z = KeyCode.Z,
                k_g = KeyCode.G,
                k_mouse1 = KeyCode.Mouse1,
                k_mouse0 =KeyCode.Mouse0;

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
            //rb_input = Input.GetKey(k_mouse0);
            mouse0 = Input.GetKey(k_mouse0);
            //locking = Input.GetKey(k_mouse2);
            mouse1 = Input.GetKey(k_mouse1);
            //rt_input = Input.GetKey(k_mouse2);
            mouse2 = Input.GetKey(k_mouse2);
            q = Input.GetKey(k_q);
            z = Input.GetKey(k_z);
            c = Input.GetKey(k_c);
            b = Input.GetKey(k_b);
            y = Input.GetKey(k_y);
            x = Input.GetKey(k_x);
            r = Input.GetKey(k_r);
            f = Input.GetKey(k_f);


            if (runInput)
            {
                run_timer += delta;
            }
            if (mouse0)
            {
                m0_timer += delta;
            }
            if (mouse1)
            {
                m1_timer += delta;
            }
            if (mouse2)
            {
                m2_timer += delta;
            }
            if (y)
            {
                y_timer += delta;
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

            if (WasThisKeyClick(mouse2, m2_timer))
            {
                //click middle mouse
                locking = true;
                states.mouse2 = true;
            }
            else
            {
                locking = false;
                states.mouse2 = false;
            }

            if (locking)
            {
                states.lockOn = !states.lockOn;
                if (states.lockOnTarget == null)
                {
                    states.lockOn = false;
                }
                if(states.lockOn)
                    states.anim.SetBool("running", false);

                camManager.lockonTarget = states.lockOnTarget;
                states.lockOnTransform = states.lockOnTarget.targetLook;
                camManager.lockon = !camManager.lockon;
            }

            if (!locking)
            {
                if (runInput && run_timer > 0.5f)
                {
                    if (states.moveAmount > 0)
                    {
                        states.run = true;
                    }
                }
            }

            if(WasThisKeyClick(runInput, run_timer))
            {
                states.rollInput = true;
            }


            if (WasThisKeyClick(mouse1, m1_timer, .05f))
            {
                states.mouse1 = true;
            }
            else
            {
                states.mouse1 = false;
            }

            if (WasThisKeyClick(mouse0, m0_timer, .05f))
            {
                states.mouse0 = true;
            }
            else
            {
                states.mouse0 = false;
            }

            states.R = r;
            states.Q = q;
            states.E = e;
            states.F = f;

            if (!y && y_timer > 0 && y_timer < .5f)
            {
                if (states.inventoryManager.curWeapon.switchable)
                {
                    states.isTwoHanded = !states.isTwoHanded;
                    states.HandleTwoHanded();
                }
            }

        }

        bool WasThisKeyClick(bool keyFlag, float keyTimer, float min=0f, float max=.5f)
        {
            bool r;

            r = !keyFlag && keyTimer > min && keyTimer < max;

            return r;
        }

        void ResetInputNStates()
        {
            if (!runInput)
                run_timer = 0;
            if (!mouse2)
            {
                m2_timer = 0;
            }
            if (!mouse0)
            {
                m0_timer = 0;
            }
            if (!mouse1)
            {
                m1_timer = 0;
            }
            if (!y)
            {
                y_timer = 0;
            }
            if (states.rollInput)
                states.rollInput = false;
            if (states.run)
                states.run = false;
        }
    }
}
