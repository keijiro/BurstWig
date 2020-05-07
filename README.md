Burst Wig
---------

![gif](https://i.imgur.com/FWMHNfQ.gif)
![gif](https://i.imgur.com/z1HkqM2.gif)

This project is an attempt to refurbish my old Wig effect (KvantWig) with the
latest technology stack of Unity.

The old implementation uses an outdated GPGPU method, which was the most
portable approach in the Unity 5.x era.

Now Unity has the C# Job System and the Burst compiler, which can handle vertex
animation very performantly. The latest version of VFX Graph supports particle
strips that are convenient to draw hair strands.

So, now the effect is split into two parts:

- Dynamics (CPU): Runs the hair simulation using the C# Job System and bakes
  the result into a texture.
- Rendering (GPU): Receives the vertex positions via the baked texture and
  renders the hair strands using particle strips in VFX Graph.

The result is not too bad. If you have lots of cores on the CPU, it works very
performantly. On my Windows system (Ryzen 7 3700X), it completes the entire
dynamics jobs in 0.2ms.

I don't think it's the best design choice for mobile platforms where the merit
of CPU-size parallelism is limited compared to desktop platforms.
