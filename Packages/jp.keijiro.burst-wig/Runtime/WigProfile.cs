using UnityEngine;

namespace BurstWig {

[System.Serializable]
public struct WigProfile
{
    public float length;
    public float lengthRandomness;
    public float spring;
    public float damping;
    public Vector3 gravity;
    public float noiseAmplitude;
    public float noiseFrequency;
    public float noiseSpeed;

    static public WigProfile DefaultProfile => new WigProfile
    {
        length = 1,
        lengthRandomness = 0.5f,
        spring = 600,
        damping = 30,
        gravity = new Vector3(0, -8, 2),
        noiseAmplitude = 5,
        noiseFrequency = 1,
        noiseSpeed = 0.1f
    };
}

} // namespace BurstWig
