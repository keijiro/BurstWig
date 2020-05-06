using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BurstWig
{
    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    struct UpdateJob : IJobFor
    {
        // Buffers
        [NativeDisableParallelForRestrictionAttribute]
        public NativeArray<RootPoint> R;

        [NativeDisableParallelForRestrictionAttribute]
        public NativeArray<float4> P;

        [NativeDisableParallelForRestrictionAttribute]
        public NativeArray<float3> V;

        // Settings
        public WigProfile prof;
        public uint seed;

        // Current state
        public float4x4 tf;
        public float t;
        public float dt;

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

        public void Execute(int vi)
        {
            var scount = P.Length / R.Length;

            // Segment length
            var seg = math.frac((seed + vi * 0.012817f) * 632.8133f); // PRNG
            seg = (1 - seg * prof.lengthRandomness) * prof.length / scount;

            //
            // Position update
            //

            var i = vi * scount;

            // The first vertex (root point, transform only)
            var p = math.mul(tf, math.float4(R[vi].position, 1)).xyz;
            P[i++] = math.float4(p, 1);

            // The second vertex (no dynamics)
            p += R[vi].normal * seg;
            P[i++] = math.float4(p, 1);

            // Following vertices
            for (var si = 2; si < scount; si++)
            {
                // Newtonian motion
                var p_n = P[i].xyz + V[i] * dt;
                // Segment length constraint
                p += math.normalize(p_n - p) * seg;
                P[i++] = math.float4(p, 1);
            }

            //
            // Velocity Update
            //

            i = vi * scount + 2; // Starts from the third vertex.

            for (var si = 2; si < scount; si++)
            {
                var v = V[i];

                // Vertex references
                var p0 = P[i].xyz;
                var p1 = P[i - 1].xyz;
                var p4 = P[i - math.min(si, 4)].xyz;

                // Damping
                v *= math.exp(-prof.damping * dt);

                // Target position
                var p_t = p1 + math.normalizesafe(p1 - p4) * seg;

                // Acceleration (spring model)
                v += (p_t - p0) * dt * prof.spring;

                // Gravity
                v += (float3)prof.gravity * dt;

                // Noise field
                v += NoiseField(p0) * dt;

                V[i++] = v;
            }
        }
    }
}
