using UnityEngine;

namespace BurstWig
{
    public class WigProfile : ScriptableObject
    {
        public float length = 1;
        public float lengthRandomness = 0.5f;
        public float spring = 600;
        public float damping = 30;
        public Vector3 gravity = new Vector3(0, -8, 2);
        public float noiseAmplitude = 5;
        public float noiseFrequency = 1;
        public float noiseSpeed = 0.1f;
    }
}
