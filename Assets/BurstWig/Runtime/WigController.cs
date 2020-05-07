using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VisualEffect = UnityEngine.VFX.VisualEffect;

namespace BurstWig
{
    public sealed partial class WigController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] MeshRenderer _source = null;
        [SerializeField] VisualEffect _target = null;
        [SerializeField, Range(8, 256)] int _segmentCount = 64;
        [SerializeField] uint _randomSeed = 0;
        [SerializeField] WigProfile _profile = WigProfile.DefaultProfile;

        #endregion

        #region Private variables

        NativeArray<RootPoint> _rootPoints;
        NativeArray<float4> _positionBuffer;
        NativeArray<float3> _velocityBuffer;
        Texture2D _positionMap;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            var mesh = _source.GetComponent<MeshFilter>().sharedMesh;
            var vcount = mesh.vertexCount;

            _rootPoints = new NativeArray<RootPoint>
              (vcount, Allocator.Persistent);

            _positionBuffer = new NativeArray<float4>
              (vcount * _segmentCount, Allocator.Persistent);

            _velocityBuffer = new NativeArray<float3>
              (vcount * _segmentCount, Allocator.Persistent);

            var vertices = mesh.vertices;
            var normals = mesh.normals;

            for (var vi = 0; vi < vcount; vi++)
                _rootPoints[vi] = new RootPoint
                  { position = vertices[vi], normal = normals[vi] };

            _positionMap = new Texture2D
              (_segmentCount, vcount, TextureFormat.RGBAFloat, false);

            _target.SetTexture("PositionMap", _positionMap);
            _target.SetUInt("VertexCount", (uint)vcount);
            _target.SetUInt("SegmentCount", (uint)_segmentCount);
        }

        void OnDestroy()
        {
            if (_rootPoints.IsCreated) _rootPoints.Dispose();
            if (_positionBuffer.IsCreated) _positionBuffer.Dispose();
            if (_velocityBuffer.IsCreated) _velocityBuffer.Dispose();
            if (_positionMap != null) Destroy(_positionMap);
        }

        void LateUpdate()
        {
            var job = new UpdateJob
            {
                // Buffers
                R = _rootPoints, P = _positionBuffer, V = _velocityBuffer,

                // Settings
                prof = _profile, seed = _randomSeed,

                // Current state
                tf = (float4x4)_source.transform.localToWorldMatrix,
                t = Time.time, dt = Time.deltaTime
            };

            job.Schedule(_rootPoints.Length, 1).Complete();

            _positionMap.LoadRawTextureData(_positionBuffer);
            _positionMap.Apply();
        }

        #endregion
    }
}
