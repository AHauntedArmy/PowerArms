using UnityEngine;
using UnityEngine.XR;

namespace PowerArms.Hands
{
    public struct InputState
    {
        public bool isActive;
        public bool wasPressed;
        public bool wasReleased;
    }

    public abstract class HandTracker : MonoBehaviour
    {
        private float speed = 0f;
        private Vector3 lastPosition = Vector3.zero;
        private Vector3 direction = Vector3.zero;
        private Vector3 rawDirection = Vector3.zero;

        private static Transform bodyOffset = null;

        private AverageDirection smoothedDirection = AverageDirection.Zero;

        private InputDevice controller;
        private InputState triggerButton;
        private InputState primaryButton;

        protected abstract XRNode controllerNode { get; }

        public float Speed { get { return speed; } }

        public Vector3 Direction { get { return direction; } }

        public Vector3 RawDirection { get { return rawDirection; } }

        public InputDevice inputDevice {
            get {
                if (!controller.isValid) {
                    return controller = InputDevices.GetDeviceAtXRNode(controllerNode);
                }

                return controller;
            }
        }

        public InputState TriggerButton { get => triggerButton; }
        public InputState PrimaryButton { get => primaryButton; }

        public void Awake()
        {
            if(bodyOffset == null) { 
                bodyOffset = GorillaLocomotion.Player.Instance.turnParent.gameObject.transform; 
            }

            Debug.Log("PowerArms: " + controllerNode.ToString());
        }

        private void OnEnable()
        {
            lastPosition = gameObject.transform.position - bodyOffset.position;
        }

        private void OnDisable()
        {
            lastPosition = Vector3.zero;
            rawDirection = Vector3.zero;
            direction = Vector3.zero;
            speed = 0f;

            smoothedDirection = AverageDirection.Zero;
        }

        private void Update()
        {
            UpdateButtonState(CommonUsages.triggerButton, ref triggerButton);
            UpdateButtonState(CommonUsages.primaryButton, ref primaryButton);

            // looking at isPRessed in dnspy, it returns false if no input or no device
            bool buttonState;
            inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out buttonState);

            /**
            if (buttonState) {
                if (!isActive) {
                    // Debug.Log("InputController: button was pressed");
                    wasPressed = true;

                } else {
                    // Debug.Log("InputController: button is held");
                    wasPressed = false;
                }

            } else {
                if (isActive) {
                    wasReleased = true;
                    wasPressed = false;

                } else {
                    wasReleased = false;
                    wasPressed = false;
                }
            }

            isActive = buttonState;
            */

        }

       private void LateUpdate()
        {
            // Debug.Log("SwimHandTracker: LateUpdate was called");
            Vector3 currentHandPos = gameObject.transform.position - bodyOffset.position;
            rawDirection = lastPosition - currentHandPos;
            lastPosition = currentHandPos;
            speed = 0f;

            // Debug.Log(string.Format("SwimHandTracker: \nwasPressed: {0} \nisActive: {1}, wasReleased: {2}", inputState.wasPressed, inputState.isActive, inputState.wasReleased));

            if (triggerButton.wasPressed) {
                // Debug.Log("SwimHandTracker: TriggerWasPressed");
                speed = rawDirection.magnitude;
                speed = speed > 0f ? speed / Time.deltaTime : 0f;
                smoothedDirection += new AverageDirection(rawDirection, 0f);
                direction = rawDirection.normalized;

                // Debug.Log("SwimHandTracker: Speed: " + speed);

            } else if (triggerButton.isActive) {
                // Debug.Log("SwimHandTracker: TriggerIsHeld");
                speed = rawDirection.magnitude;
                speed = speed > 0f ? speed / Time.deltaTime : 0f;
                smoothedDirection += new AverageDirection(rawDirection * 0.5f, 0f);
                direction = smoothedDirection.Vector.normalized;

                // Debug.Log("SwimHandTracker: Speed: " + speed);

            } else if (triggerButton.wasReleased) {
                // Debug.Log("TriggerWasReleased");
                speed = 0f;
                smoothedDirection = AverageDirection.Zero;
                direction = Vector3.zero;
            }
                
        }

        private void UpdateButtonState(InputFeatureUsage<bool> button, ref InputState buttonState)
        {
            // looking at isPRessed in dnspy, it returns false if no input or no device
            bool input = false;
            inputDevice.TryGetFeatureValue(button, out input);

            if (input) {
                if (!buttonState.isActive) {
                    // Debug.Log("InputController: button was pressed");
                    buttonState.wasPressed = true;

                } else {
                    // Debug.Log("InputController: button is held");
                    buttonState.wasPressed = false;
                }

            } else {
                if (buttonState.isActive) {
                    buttonState.wasReleased = true;
                    buttonState.wasPressed = false;

                } else {
                    buttonState.wasReleased = false;
                    buttonState.wasPressed = false;
                }
            }

            buttonState.isActive = input;
        }
    }
}