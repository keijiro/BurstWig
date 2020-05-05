using Unity.Mathematics;
using Unity.Collections;

namespace BurstWig
{
    static class Utility
    {
        public static NativeArray<T> NewBuffer<T>(int size, int count)
          where T : unmanaged
          => new NativeArray<T>(size * count, Allocator.Persistent);

        public static float Random(uint seed, uint id)
          => math.frac((seed + id * 0.012817f) * 632.8133f);
    }
}
