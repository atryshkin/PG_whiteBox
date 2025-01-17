﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;

        //Speed of camera movement to target
        public float followSpeed = 9;

        public float mouseSpeed = 2;

        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;

        [HideInInspector]
        public Transform pivot;
        public Transform camTrans;
        StateManager states;

        float turnSmoothing = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        float smoothX;
        float smoothY;
        float smoothX_velocity;
        float smoothY_velocity;


        public float lookAngle;
        public float tiltAngle;


        public void Init(StateManager st)
        {
            target = st.transform;
            states = st;

            pivot = camTrans.parent;
            if (lockonTransform == null){
                lockonTransform = lockonTarget.targetLook;
                states.lockOnTransform = lockonTransform;
            }
        }

        public void FixedTick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            
            float targetSpeed = mouseSpeed;

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        void FollowTarget(float d)
        {
            float speed = followSpeed * d;

            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);

            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {

            if (turnSmoothing > 0)
            {

                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothX_velocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothY_velocity, turnSmoothing);

            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);


            //if (lockon && lockonTarget!=null)
            //{
            //    Vector3 targetDir = lockonTransform.position - transform.position;
            //    targetDir.Normalize();
            //    //targetDir.y = 0;

            //    if (targetDir == Vector3.zero)
            //    {
            //        targetDir = transform.forward;
            //    }

            //    Quaternion targetRot = Quaternion.LookRotation(targetDir);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
            //    lookAngle = transform.eulerAngles.y;
            //    return;
            //}

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);


        }

        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
