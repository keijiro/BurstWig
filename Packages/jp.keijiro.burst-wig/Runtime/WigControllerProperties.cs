using UnityEngine;

namespace BurstWig {

public sealed partial class WigController
{
    #region Wig profile

    public float Length
    {
        get => _profile.length;
        set => _profile.length = value;
    }

    public float LengthRandomness
    {
        get => _profile.lengthRandomness;
        set => _profile.lengthRandomness = value;
    }

    public float Spring
    {
        get => _profile.spring;
        set => _profile.spring = value;
    }

    public float Damping
    {
        get => _profile.damping;
        set => _profile.damping = value;
    }

    public Vector3 Gravity
    {
        get => _profile.gravity;
        set => _profile.gravity = value;
    }

    public float NoiseAmplitude
    {
        get => _profile.noiseAmplitude;
        set => _profile.noiseAmplitude = value;
    }

    public float NoiseFrequency
    {
        get => _profile.noiseFrequency;
        set => _profile.noiseFrequency = value;
    }

    public float NoiseSpeed
    {
        get => _profile.noiseSpeed;
        set => _profile.noiseSpeed = value;
    }

    #endregion

    #region VFX properties

    public Texture PositionMap => _positionMap;
    public uint VertexCount => (uint)_rootPoints.Length;
    public uint SegmentCount => (uint)_segmentCount;

    #endregion
}

} // namespace BurstWig
