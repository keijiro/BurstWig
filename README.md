Burst Wig
---------

![gif](https://i.imgur.com/FWMHNfQ.gif)
![gif](https://i.imgur.com/z1HkqM2.gif)

This project is an attempt to rebuild my old Wig effect (KvantWig) with
Unity's current technology stack.

The old implementation uses an outdated GPGPU method, which was the most
portable approach in the Unity 5.x era.

Unity now has the C# Job System and the Burst compiler, which can handle vertex
animation very efficiently. The latest version of VFX Graph supports particle
strips, which are useful for drawing hair strands.

So, now the effect is split into two parts:

- Dynamics (CPU): Runs the hair simulation using the C# Job System and bakes
  the result into a texture.
- Rendering (GPU): Receives the vertex positions via the baked texture and
  renders the hair strands using particle strips in VFX Graph.

The result is not too bad. If your CPU has plenty of cores, it runs very
efficiently. On my Windows system (Ryzen 7 3700X), it completes all dynamics
jobs in 0.2 ms.

I don't think it's the best design choice for mobile platforms, where the
benefits of CPU-side parallelism are limited compared to desktop platforms.
