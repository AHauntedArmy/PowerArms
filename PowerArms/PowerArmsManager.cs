using HarmonyLib;
using UnityEngine;
using GorillaLocomotion;

using PowerArms.Hands;

namespace PowerArms
{
    internal class PowerArmsManager : MonoBehaviour
    {
        private Rigidbody playerRigidBody = null;

        private LeftHandTracker leftHand = null;
        private RightHandTracker rightHand = null;

        private float acceleration = 3f;
        private float maxSpeed = 6.7f;

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
        }

        private void OnDestroy()
        {
            GameObject.Destroy(leftHand);
            GameObject.Destroy(rightHand);
        }

        private void Update()
        {
            float rightHandSpeed = acceleration * (rightHand.Speed * 0.01f);
            float leftHandSpeed = acceleration * (leftHand.Speed * 0.01f);

            Vector3 lookAssist = Camera.main.transform.forward * 0.2f;

            if (rightHandSpeed > 0f) {
                Debug.Log("PowerArms: Right hand speed = " + rightHandSpeed);
                playerRigidBody.velocity += (rightHand.Direction + lookAssist).normalized * rightHandSpeed;
            }


            if (leftHandSpeed > 0f) {
                Debug.Log("PowerArms: Left hand speed = " + leftHandSpeed);
                playerRigidBody.velocity += (leftHand.Direction + lookAssist).normalized * leftHandSpeed;
            }

            if (playerRigidBody.velocity.magnitude > maxSpeed)
                playerRigidBody.velocity = playerRigidBody.velocity.normalized * maxSpeed;
        }
    }
}