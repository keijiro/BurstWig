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
        length = 1.7f,
        lengthRandomness = 0.5f,
        spring = 150,
        damping = 20,
        gravity = new Vector3(0, -2, 2),
        noiseAmplitude = 0.4f,
        noiseFrequency = 1.3f,
        noiseSpeed = 0.1f
    };
}

} // namespace BurstWig
