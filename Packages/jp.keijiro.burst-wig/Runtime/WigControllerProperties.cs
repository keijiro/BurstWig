using UnityEngine;

namespace BurstWig
{
    public sealed partial class WigController
    {
        public float length
        {
            get => _profile.length;
            set => _profile.length = value;
        }

        public float lengthRandomness
        {
            get => _profile.lengthRandomness;
            set => _profile.lengthRandomness = value;
        }

        public float spring
        {
            get => _profile.spring;
            set => _profile.spring = value;
        }

        public float damping
        {
            get => _profile.damping;
            set => _profile.damping = value;
        }

        public Vector3 gravity
        {
            get => _profile.gravity;
            set => _profile.gravity = value;
        }

        public float noiseAmplitude
        {
            get => _profile.noiseAmplitude;
            set => _profile.noiseAmplitude = value;
        }

        public float noiseFrequency
        {
            get => _profile.noiseFrequency;
            set => _profile.noiseFrequency = value;
        }

        public float noiseSpeed
        {
            get => _profile.noiseSpeed;
            set => _profile.noiseSpeed = value;
        }
    }
}
