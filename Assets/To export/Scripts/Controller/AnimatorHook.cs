﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;

        public float rm_multiplier=1;
        bool rolling;
        float roll_t;



        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }

        public void CloseRoll()
        {
            if (!rolling)
                return;

            rm_multiplier = 1;
            roll_t = 0;
            rolling = false;
        }

        void OnAnimatorMove()
        {
            if (states.canMove)
                return;

            states.rigid.drag = 0;

            if (rm_multiplier == 0)
            {
                rm_multiplier = 1;
            }

            if (!rolling)
            {
                Vector3 delta = anim.deltaPosition;
                delta.y = 0;
                Vector3 v = (delta * rm_multiplier) / states.delta;
                states.rigid.velocity = v;
            }
            else
            {
                roll_t += states.delta / .5f;
                if (roll_t > 1)
                {
                    roll_t = 1;
                }
                float zValue = states.roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multiplier);// / states.delta;
                states.rigid.velocity = v2;
            }

        }


        void LateTick()
        {
            
        }
    }
}