﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PieGame
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        //Variables
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;

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


        [Header("Stats")]
        public float moveSpeed=2;
        public float runSpeed=3.5f;
        public float rotateSpeed = 5;
        public float distanceToGround =.5f;
        /// <summary>
        /// Weight for rolling
        /// </summary>
        public float rollSpeed=1;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;


        float _actionDelay;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        /// <summary>
        /// Start
        /// </summary>
        public void Init()
        {
            SetupAnimator();

            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init();

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook =  activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            //set player layer
            gameObject.layer = 8;

            ignoreLayers = ~(1 << 9);

            anim.SetBool("onGroud", true);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                //didn't assign model
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    //can't find animator
                    Debug.Log("No model player found!");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }

            if (anim == null)
            {
                anim = activeModel.GetComponent<Animator>();
            }

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;
            DetectAction();


            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > .3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }

            }

            canMove = anim.GetBool("canMove");

            if (!canMove)
                return;

            //a_hook.rm_multiplier = 1;
            a_hook.CloseRoll();
            HandleRolls();

            anim.applyRootMotion = false;
            if (moveAmount > 0 || onGround==false)
            {
                //we are moving
                rigid.drag = 0;
            }
            else
            {
                //we are not moving
                rigid.drag = 4;
            }

            float targetSpeed=moveSpeed;

            if (run)
            {
                targetSpeed = runSpeed;
                
            }

            if (onGround)
            {
                rigid.velocity = moveDir * (targetSpeed * moveAmount);
            }


            Vector3 targetDir = (lockOn==false) ? moveDir:
                (lockOnTransform != null)?
                lockOnTransform.position - transform.position:
                moveDir;

            targetDir.y = 0;
            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }
            Quaternion tR = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tR, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool("lockon", lockOn);

            if (lockOn)
            {
                HandleLockOnAnimations(moveDir);
            }
            else
            {
                HandleMovementAnimations();
            }
        }

        public void DetectAction()
        {
            if (!rb && !rt && !lt && !lb)
            {
                return;
            }
            string targetAnim="";

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
            {
                return;
            }

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;
            inAction = true;
            canMove = false;
            if (anim.GetCurrentAnimatorStateInfo(5).IsName(targetAnim) ||
                anim.GetAnimatorTransitionInfo(5).IsName(targetAnim))
                return;
            anim.CrossFade(targetAnim, .04f);
        }

        public void Tick(float d)
        {
            delta = d;

            onGround = OnGround();

            anim.SetBool("onGround", onGround);
        }

        void HandleRolls()
        {
            if (!rollInput)
            {
                return;
            }

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > .3f) ? 1 : 0;
            h = 0;


            //if (!lockOn)
            //{
            //    v = (moveAmount>.3f)?1:0;
            //    h = 0;
            //}
            //else
            //{
            //    if (Mathf.Abs(v) < .3f)
            //    {
            //        v = 0;
            //    }

            //    if (Mathf.Abs(h) < .3f)
            //    {
            //        h = 0;
            //    }
            //}

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                {
                    moveDir = transform.forward;
                }
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multiplier = rollSpeed;
            }
            else
            {
                a_hook.rm_multiplier = 1.3f;
            }


            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            inAction = true;
            canMove = false;
            anim.CrossFade("Rolls", .05f);
        }

        void HandleMovementAnimations()
        {
            if(Mathf.Abs(horizontal)>0 || Mathf.Abs(vertical) > 0)
            {
                anim.SetBool("running", run);
            }
            else
            {
                anim.SetBool("running", false);
            }
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);

            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, .2f, delta);
            anim.SetFloat("horizontal", h, .2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + Vector3.up * distanceToGround;
            Vector3 dir = -Vector3.up;
            //minus .1f to raycasting a bit further
            float dis = distanceToGround + 0.3f;
            RaycastHit hit;
            Debug.DrawRay(origin, dir * dis);

            if(Physics.Raycast(origin,dir,out hit, dis))
            {
                r = true;

                Vector3 targetPosition = hit.point;

                transform.position = targetPosition;
            }


            return r;
        }

        public void HandleTwoHanded()
        {
            anim.SetBool("Two handed", isTwoHanded);

            if (isTwoHanded)
            {
                actionManager.UpdateActionsTwoHanded();
            }
            else
            {
                actionManager.UpdateActionsOneHanded();
            }
        }
    }
}