using UnityEngine;

namespace PowerArms.Hands
{
    public readonly struct AverageDirection
    {

        private readonly uint directionAmount; // stores how many directions have been added
        private readonly Vector3 vector;
        private readonly float speed;

        public uint Amount { 
            get { return directionAmount; } 
            private set { } 
        }

        public float Speed {
            get { return ((directionAmount > 1 && speed != 0f) ? speed / directionAmount : speed); } 
            private set { } 
        }
        
        public Vector3 Direction { 
            get { return vector.normalized; } 
            private set { } 
        }
        
        // don't know if this will get used, thought it would be good to have
        public Vector3 Vector {
            get {
                // unity engine doesn't gaurd against dividing by zero and c# returns infinite when we want 0f
                if (directionAmount < 2) return vector;
                return new Vector3(vector.x != 0f ? vector.x / directionAmount : 0f,
                                   vector.y != 0f ? vector.y / directionAmount : 0f,
                                   vector.z != 0f ? vector.z / directionAmount : 0f);

            } 
            private set { } 
        }

        static public AverageDirection Zero { get { return new AverageDirection(Vector3.zero, 0f, 0); } private set { } }

        public AverageDirection(Vector3 dir, float speedVal, uint dirAmount = 1)
        {
            vector = dir;
            speed = speedVal;
            directionAmount = dirAmount;
        }

        public static AverageDirection operator +(AverageDirection first, AverageDirection secound)
        {
            return new AverageDirection(first.vector + secound.vector, 
                                        first.speed + secound.speed, 
                                        first.directionAmount + 1);
        }

        public static AverageDirection operator -(AverageDirection first, AverageDirection secound)
        {
            return new AverageDirection(first.vector - secound.vector, 
                                        first.speed - secound.speed, 
                                        first.directionAmount - 1);
        }
    }

}