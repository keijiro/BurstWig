using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using VisualEffect = UnityEngine.VFX.VisualEffect;

namespace BurstWig
{
    public class WigController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] MeshRenderer _source = null;
        [SerializeField] VisualEffect _target = null;
        [SerializeField, Range(8, 256)] int _segmentCount = 64;
        [SerializeField] WigProfile _profile = null;
        [SerializeField] uint _randomSeed = 0;

        #endregion

        #region Internal buffers

        NativeArray<float4> _position;
        NativeArray<float3> _velocity;
        Texture2D _positionMap;

        #endregion

        #region Private function

        float SegmentLength(int index)
          => (1 - Utility.Random(_randomSeed, (uint)index)
                  * _profile.lengthRandomness)
             * _profile.length / _segmentCount;

        float3 NoiseField(float3 p)
        {
            var pos = p * _profile.noiseFrequency;

            var offs1 = math.float3(0, 1, 0) * _profile.noiseSpeed * Time.time;
            var offs2 = math.float3(3, 1, 7) * math.PI - offs1.zyx;

            float3 grad1, grad2;
            noise.snoise(pos + offs1, out grad1);
            noise.snoise(pos + offs2, out grad2);

            return math.cross(grad1, grad2) * _profile.noiseAmplitude;
        }

        #endregion

        #region Template data structure

        [StructLayout(LayoutKind.Sequential)]
        struct Template
        {
            public float3 position;
            public float3 normal;
        }

        NativeArray<Template> _template;

        void SetUpTemplate(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var normals = mesh.normals;

            for (var vi = 0; vi < vertices.Length; vi++)
            {
                // Initialize a template vertex.
                var p = vertices[vi];
                var n = normals[vi];
                _template[vi] = new Template { position = p, normal = n };

                // Initialize vertices under this template vertex.
                var i = vi * _segmentCount;
                var seg = SegmentLength(vi);
                for (var si = 0; si < _segmentCount; si++)
                {
                    _position[i++] = math.float4(p, 1);
                    p += n * seg;
                }
            }
        }

        #endregion

        #region Wig dynamics

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
                var seg = SegmentLength(vi);

                // The first vertex
                var p = _template[vi].position;
                var v = float3.zero;

                p = math.mul(tf, math.float4(p, 1)).xyz;

                _position[i++] = math.float4(p, 1);
                var p_prev = p;

                // The second vertex

                p += _template[vi].normal * seg;

                _position[i++] = math.float4(p, 1);
                p_prev = p;

                for (var si = 2; si < scount; si++)
                {
                    p = _position[i].xyz;
                    v = _velocity[i];

                    // Newtonian motion
                    p += v * dt;

                    // Segment length constraint
                    p = p_prev + math.normalize(p - p_prev) * seg;

                    _position[i++] = math.float4(p, 1);
                    p_prev = p;
                }
            }

            // Vertex update
            for (var vi = 0; vi < vcount; vi++)
            {
                var i = vi * scount;

                var seg = SegmentLength(vi);

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
                    v *= math.exp(-_profile.damping * dt);

                    // Target position
                    var p_t = p_prev + math.normalizesafe(p_prev - p_his4) * seg;

                    // Acceleration (spring model)
                    v += (p_t - p) * dt * _profile.spring;

                    // Gravity
                    v += (float3)_profile.gravity * dt;

                    // Noise field
                    v += NoiseField(p) * dt;

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

            _template = Utility.NewBuffer<Template>(vcount, 1);
            _position = Utility.NewBuffer<float4>(vcount, _segmentCount);
            _velocity = Utility.NewBuffer<float3>(vcount, _segmentCount);

            SetUpTemplate(mesh);

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
}
