using UnityEngine.XR;

namespace PowerArms.Hands
{
    public class LeftHandTracker : HandTracker
    {
        protected override XRNode controllerNode => XRNode.LeftHand;
    }

    public class RightHandTracker : HandTracker
    {
        protected override XRNode controllerNode => XRNode.RightHand;
    }
}