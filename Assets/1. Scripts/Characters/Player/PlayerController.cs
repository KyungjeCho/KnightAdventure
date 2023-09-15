using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG.Character
{
    /// <summary>
    /// PlayerController �� ���
    /// 1. �÷��̾��� �̵�
    /// 2. �÷��̾��� ���� [�ִϸ��̼�, Manual Collision]
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        private float h = 0f;
        private float v = 0f;
        private Vector3 moveDir;
        private int animHashHorizontal  = Animator.StringToHash(AnimKey.Horizontal);
        private int animHashVertical    = Animator.StringToHash(AnimKey.Vertical);

        public float moveSpeed = 10f;
        #endregion Variables

        #region Caching
        private Transform myTransform;
        private Rigidbody myRigidbody;
        private Transform cameraTr;
        private Animator myAnimator;
        #endregion Caching

        void Start()
        {
            // Cashing
            myTransform = transform;
            myAnimator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody>();
            cameraTr = Camera.main.transform;
            // Cashing
        }
        void Update()
        {
            // Input
            h = Input.GetAxis(InputKey.Horizontal);
            v = Input.GetAxis(InputKey.Vertical);
            // Input

            // ĳ���� �̵� ���� ���
            moveDir = (Vector3.forward * v) + (Vector3.right * h);
            moveDir = moveDir.normalized;
            // ĳ���� �̵� ���� ���

            // ĳ���� �ü� ���� ���
            Vector3 forward = cameraTr.TransformDirection(Vector3.forward);
            forward.y = 0.0f;
            forward = forward.normalized;

            myTransform.rotation = Quaternion.LookRotation(forward);
            // ĳ���� �ü� ���� ���

            // �ִϸ��̼�
            if (MathHelper.FloatZeroChecker(v))
                myAnimator.SetInteger(animHashVertical, 0);
            else if (v > 0)
                myAnimator.SetInteger(animHashVertical, 1);
            else
                myAnimator.SetInteger(animHashVertical, -1);

            if (MathHelper.FloatZeroChecker(h))
                myAnimator.SetInteger(animHashHorizontal, 0);
            else if (h > 0)
                myAnimator.SetInteger(animHashHorizontal, 1);
            else
                myAnimator.SetInteger(animHashHorizontal, -1);
            // �ִϸ��̼�
        }

        private void FixedUpdate()
        {
            moveDir = myTransform.TransformDirection(moveDir);

            myRigidbody.velocity = moveDir * moveSpeed;
        }
    }

}
