using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using XXHash = Klak.Math.XXHash;

namespace BurstWig {

[Unity.Burst.BurstCompile(CompileSynchronously = true)]
struct UpdateJob : IJobParallelFor
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

    // Noise field variables
    float3 noffs1;
    float3 noffs2;

    // Divergence-free noise field function
    float3 NoiseField(float3 p)
    {
        p *= prof.noiseFrequency;

        float3 grad1, grad2;
        noise.snoise(p.xyz + noffs1, out grad1);
        noise.snoise(p.yzx + noffs2, out grad2);

        return math.cross(grad1, grad2) * prof.noiseAmplitude;
    }

    public void Execute(int vi)
    {
        // Number of the segments
        var scount = P.Length / R.Length;

        // Per-filament random segment length
        var seg = new XXHash(seed).Float((uint)vi);
        seg = (1 - seg * prof.lengthRandomness) * prof.length / scount;

        // Noise field settings
        noffs1 = math.float3(0, 1, 0) * prof.noiseSpeed * t;
        noffs2 = math.float3(3, 1, 7) * math.PI - noffs1.zyx;

        // Offset in the position/velocity buffer.
        var offs = vi * scount;

        // The first vertex (root point, transform only)
        var p = math.mul(tf, math.float4(R[vi].position, 1)).xyz;
        P[offs++] = math.float4(p, 1);

        // The second vertex (no dynamics)
        p += math.mul(tf, math.float4(R[vi].normal, 0)).xyz * seg;
        P[offs++] = math.float4(p, 1);

        // Following vertices
        for (var si = 2; si < scount; si++, offs++)
        {
            // Target position
            var p4 = P[offs - math.min(si, 4)].xyz;
            var p_t = p + math.normalizesafe(p - p4) * seg;

            // -- Position update

            // Newtonian motion
            var p_n = P[offs].xyz + V[offs] * dt;

            // Segment length constraint
            p += math.normalize(p_n - p) * seg;

            // -- Velocity Update

            // Damping
            var v = V[offs] * math.exp(-prof.damping * dt);

            // Acceleration (spring model)
            v += (p_t - p) * dt * prof.spring;

            // Gravity
            v += (float3)prof.gravity * dt;

            // Noise field
            v += NoiseField(p) * dt;

            // Output
            P[offs] = math.float4(p, 1);
            V[offs] = v;
        }
    }
}

} // namespace BurstWig {
