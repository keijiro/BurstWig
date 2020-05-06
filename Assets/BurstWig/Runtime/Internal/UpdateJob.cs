using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BurstWig
{
    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    struct UpdateJob : IJob
    {
        // Buffers
        public NativeArray<RootPoint> R;
        public NativeArray<float4> P;
        public NativeArray<float3> V;

        // Settings
        public WigProfile prof;
        public uint seed;

        // Current state
        public float4x4 tf;
        public float t;
        public float dt;

        float SegmentLength(int index)
          => (1 - Utility.Random(seed, (uint)index)
                  * prof.lengthRandomness)
             * prof.length / (P.Length / R.Length);

        float3 NoiseField(float3 p)
        {
            var pos = p * prof.noiseFrequency;

            var offs1 = math.float3(0, 1, 0) * prof.noiseSpeed * t;
            var offs2 = math.float3(3, 1, 7) * math.PI - offs1.zyx;

            float3 grad1, grad2;
            noise.snoise(pos + offs1, out grad1);
            noise.snoise(pos + offs2, out grad2);

            return math.cross(grad1, grad2) * prof.noiseAmplitude;
        }

        public void Execute()
        {
            var vcount = R.Length;
            var scount = P.Length / vcount;

            // Position update
            for (var vi = 0; vi < vcount; vi++)
            {
                var i = vi * scount;
                var seg = SegmentLength(vi);

                // The first vertex
                var p = R[vi].position;
                var v = float3.zero;

                p = math.mul(tf, math.float4(p, 1)).xyz;

                P[i++] = math.float4(p, 1);
                var p_prev = p;

                // The second vertex

                p += R[vi].normal * seg;

                P[i++] = math.float4(p, 1);
                p_prev = p;

                for (var si = 2; si < scount; si++)
                {
                    p = P[i].xyz;
                    v = V[i];

                    // Newtonian motion
                    p += v * dt;

                    // Segment length constraint
                    p = p_prev + math.normalize(p - p_prev) * seg;

                    P[i++] = math.float4(p, 1);
                    p_prev = p;
                }
            }

            // Velocity
            for (var vi = 0; vi < vcount; vi++)
            {
                var i = vi * scount;
                var seg = SegmentLength(vi);

                var p_prev = P[i].xyz;
                var p_his2 = p_prev;
                var p_his3 = p_prev;
                var p_his4 = p_prev;

                i++;

                for (var si = 1; si < scount; si++)
                {
                    var p = P[i].xyz;
                    var v = V[i];

                    // Damping
                    v *= math.exp(-prof.damping * dt);

                    // Target position
                    var p_t = p_prev + math.normalizesafe(p_prev - p_his4) * seg;

                    // Acceleration (spring model)
                    v += (p_t - p) * dt * prof.spring;

                    // Gravity
                    v += (float3)prof.gravity * dt;

                    // Noise field
                    v += NoiseField(p) * dt;

                    V[i++] = v;
                    p_his4 = p_his3;
                    p_his3 = p_his2;
                    p_his2 = p_prev;
                    p_prev = p;
                }
            }
        }
    }
}
