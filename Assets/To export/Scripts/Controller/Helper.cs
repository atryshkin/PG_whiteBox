using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class Helper : MonoBehaviour
    {
        [Range(-1, 1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;

        public string[] oh_attacks;
        public string[] th_attacks;
        public bool playThisAnim;

        public bool twoHanded;
        public bool enableRM;//enable root motion
        public bool blocking;
        public bool interacting;
        public bool lockon;
       

        Animator anim;


        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            enableRM = !anim.GetBool("canMove");
            //for attacks
            anim.applyRootMotion = enableRM;

            interacting = anim.GetBool("interacting");

            if (lockon==false)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }

            anim.SetBool("lockon", lockon);

            if (enableRM)
            {
                return;
            }

            if (blocking)
            {

                anim.Play("Blocking");

                blocking = false;

            }

            if (interacting)
            {
                playThisAnim = false;
                vertical = Mathf.Clamp(vertical, 0, .5f);
            }

            if (playThisAnim)
            {
                string targetAnim;

                if (!twoHanded)
                {
                    //using two handed weapon
                    int r = Random.Range(0, oh_attacks.Length);
                    targetAnim = oh_attacks[r];

                }
                else
                {
                    int r = Random.Range(0, th_attacks.Length);
                    targetAnim = th_attacks[r];
                }


                vertical = 0;
                anim.CrossFade(targetAnim, .2f);
                //anim.SetBool("canMove", false);
                //enableRM = true;
                playThisAnim = false;
            }

            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);
            anim.SetBool("Two handed", twoHanded);

        }
    }
}