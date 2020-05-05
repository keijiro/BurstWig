using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using VisualEffect = UnityEngine.VFX.VisualEffect;

public class StripManager : MonoBehaviour
{
    #region Editor-only parameters

    [SerializeField] MeshRenderer _source = null;
    [SerializeField] VisualEffect _target = null;
    [SerializeField, Range(8, 256)] int _segmentCount = 64;
    [SerializeField] uint _randomSeed = 0;

    #endregion

    #region Dynamics parameters

    [SerializeField, Range(0.01f, 5)] float _length = 1;

    public float length {
        get => _length;
        set => _length = value;
    }

    [SerializeField, Range(0, 1)] float _lengthRandomness = 0.5f;

    public float lengthRandomness {
        get => _lengthRandomness;
        set => _lengthRandomness = value;
    }

    [SerializeField] float _spring = 600;

    public float spring {
        get => _spring;
        set => _spring = value;
    }

    [SerializeField] float _damping = 30;

    public float damping {
        get => _damping;
        set => _damping = value;
    }

    [SerializeField] Vector3 _gravity = new Vector3(0, -8, 2);

    public Vector3 gravity {
        get => _gravity;
        set => _gravity = value;
    }

    [SerializeField] float _noiseAmplitude = 5;

    public float noiseAmplitude {
        get => _noiseAmplitude;
        set => _noiseAmplitude = value;
    }

    [SerializeField] float _noiseFrequency = 1;

    public float noiseFrequency {
        get => _noiseFrequency;
        set => _noiseFrequency = value;
    }

    [SerializeField] float _noiseSpeed = 0.1f;

    public float noiseSpeed {
        get => _noiseSpeed;
        set => _noiseSpeed = value;
    }

    #endregion

    #region Data buffer

    NativeArray<float4> _position;
    NativeArray<float3> _velocity;
    Texture2D _positionMap;

    #endregion

    #region Template data structure

    [StructLayout(LayoutKind.Sequential)]
    struct Template
    {
        public float3 position;
        public float3 normal;
    }

    NativeArray<Template> _template;

    void InitializeTemplate(Mesh mesh)
    {
        var vertices = mesh.vertices;
        var normals = mesh.normals;

        for (var vi = 0; vi < vertices.Length; vi++)
        {
            var p = vertices[vi];
            var n = normals[vi];

            _template[vi] = new Template { position = p, normal = n };

            var i = vi * _segmentCount;

            var seglen = SegmentLength(vi);

            for (var si = 0; si < _segmentCount; si++)
            {
                _position[i++] = math.float4(p, 1);
                p += n * seglen;
            }
        }
    }

    #endregion

    #region Private functions

    float SegmentLength(int index)
      => (1 - Util.Random(_randomSeed, (uint)index) * _lengthRandomness)
         * _length / _segmentCount;

    void RunDynamics()
    {
        var vcount = _template.Length;
        var scount = _segmentCount;
        var dt = Time.deltaTime;
        var tf = (float4x4)_source.transform.localToWorldMatrix;

        // Position update
        for (var vi = 0; vi < vcount; vi++)
        {
            var i = vi * scount;

            var seglen = SegmentLength(vi);

            // The first vertex
            var p = _template[vi].position;
            var v = float3.zero;

            p = math.mul(tf, math.float4(p, 1)).xyz;

            _position[i++] = math.float4(p, 1);
            var p_prev = p;

            // The second vertex

            p += _template[vi].normal * seglen;

            _position[i++] = math.float4(p, 1);
            p_prev = p;

            for (var si = 2; si < scount; si++)
            {
                p = _position[i].xyz;
                v = _velocity[i];

                // Newtonian motion
                p += v * dt;

                // Segment length constraint
                p = p_prev + math.normalize(p - p_prev) * seglen;

                _position[i++] = math.float4(p, 1);
                p_prev = p;
            }
        }

        // Vertex update
        for (var vi = 0; vi < vcount; vi++)
        {
            var i = vi * scount;

            var seglen = SegmentLength(vi);

            var p_prev = _position[i].xyz;
            var p_his2 = p_prev;
            var p_his3 = p_prev;
            var p_his4 = p_prev;

            i++;

            for (var si = 1; si < scount; si++)
            {
                var p = _position[i].xyz;
                var v = _velocity[i];

                // Damping
                v *= math.exp(-_damping * dt);

                // Target position
                var p_t = p_prev + math.normalizesafe(p_prev - p_his4) * seglen;

                // Acceleration (spring model)
                v += (p_t - p) * dt * _spring;

                // Gravity
                v += (float3)_gravity * dt;

                _velocity[i++] = v;
                p_his4 = p_his3;
                p_his3 = p_his2;
                p_his2 = p_prev;
                p_prev = p;
            }
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var mesh = _source.GetComponent<MeshFilter>().sharedMesh;
        var vcount = mesh.vertexCount;

        _template = Util.NewBuffer<Template>(vcount, 1);
        _position = Util.NewBuffer<float4>(vcount, _segmentCount);
        _velocity = Util.NewBuffer<float3>(vcount, _segmentCount);

        InitializeTemplate(mesh);

        _positionMap = new Texture2D(_segmentCount, vcount,
                                     TextureFormat.RGBAFloat, false);

        _target.SetTexture("PositionMap", _positionMap);
        _target.SetUInt("VertexCount", (uint)vcount);
        _target.SetUInt("SegmentCount", (uint)_segmentCount);
    }

    void OnDestroy()
    {
        if (_template.IsCreated) _template.Dispose();
        if (_position.IsCreated) _position.Dispose();
        if (_velocity.IsCreated) _velocity.Dispose();
        if (_positionMap != null) Destroy(_positionMap);
    }

    void Update()
    {
        RunDynamics();
        _positionMap.LoadRawTextureData(_position);
        _positionMap.Apply();
    }

    #endregion
}
