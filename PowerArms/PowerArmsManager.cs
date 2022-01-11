using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;

using PowerArms.Hands;

namespace PowerArms
{
    internal class PowerArmsManager : MonoBehaviour
    {
        private Rigidbody playerRigidBody = null;

        private LeftHandTracker leftHand = null;
        private RightHandTracker rightHand = null;

        private const float defaultAcceleration = 2.5f;
        private const float defaultMaxSpeed = 6.7f;
        private float acceleration = 2.5f;
        private float maxSpeed = 6.7f;
        private Vector3 AccelVector = Vector3.zero;

        private void Start()
        {
            leftHand = Player.Instance.leftHandFollower.gameObject.AddComponent<LeftHandTracker>();
            rightHand = Player.Instance.rightHandFollower.gameObject.AddComponent<RightHandTracker>();

            playerRigidBody = (Rigidbody)AccessTools.Field(typeof(GorillaLocomotion.Player), "playerRigidBody").GetValue(Player.Instance);
        }

        private void OnEnable()
        {
            if (leftHand != null)
                leftHand.enabled = true;
            
            if(rightHand != null)
                rightHand.enabled = true;

        }

        private void OnDisable()
        {
            if (leftHand != null)
                leftHand.enabled = false;

            if (rightHand != null)
                rightHand.enabled = false;

            if (playerRigidBody != null)
                playerRigidBody.useGravity = true;

            acceleration = defaultAcceleration;
            maxSpeed = defaultMaxSpeed;
           
        }

        private void OnDestroy()
        {
            GameObject.Destroy(leftHand);
            GameObject.Destroy(rightHand);
        }

        // avoid framerate dependent movement
        private void FixedUpdate()
        {
            playerRigidBody.velocity += AccelVector;

        }

        private void Update()
        {
            Swim();
            Gravity();
        }

        private void Swim()
        {
            float rightHandSpeed = acceleration * (rightHand.Speed * 0.01f);
            float leftHandSpeed = acceleration * (leftHand.Speed * 0.01f);

            AccelVector = Vector3.zero;

            if (rightHandSpeed > 0f || leftHandSpeed > 0f) {
                // Debug.Log("PowerArms: Right hand speed = " + rightHandSpeed);
                // Debug.Log("PowerArms: Left hand speed = " + leftHandSpeed);

                Vector3 lookAssist = Camera.main.transform.forward * 0.2f;
                AccelVector += (rightHand.Direction + lookAssist).normalized * rightHandSpeed;
                AccelVector += (leftHand.Direction + lookAssist).normalized * leftHandSpeed;

                if (playerRigidBody.velocity.magnitude > maxSpeed || (playerRigidBody.velocity + AccelVector).magnitude > maxSpeed) {
                    playerRigidBody.velocity = (playerRigidBody.velocity + AccelVector).normalized * maxSpeed;
                    AccelVector = Vector3.zero;
                }

            }
        }

        private void Gravity()
        {
            if (rightHand.PrimaryButton.wasPressed || leftHand.PrimaryButton.wasPressed) {
                playerRigidBody.useGravity = !playerRigidBody.useGravity;

                if (!playerRigidBody.useGravity) {
                    acceleration *= 0.5f;
                    maxSpeed *= 0.5f;
                
                } else {
                    acceleration = defaultAcceleration;
                    maxSpeed = defaultMaxSpeed;
                }

            }
        }
    }
}