using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace BurstWig {

[AddComponentMenu("VFX/Property Binders/BurstWig/Wig Binder")]
[VFXBinder("Wig")]
public sealed class VFXWigBinder : VFXBinderBase
{
    public WigController Source = null;

    [VFXPropertyBinding("UnityEngine.Texture")]
    public ExposedProperty PositionMapProperty = "PositionMap";

    [VFXPropertyBinding("System.UInt32")]
    public ExposedProperty VertexCountProperty = "VertexCount";

    [VFXPropertyBinding("System.UInt32")]
    public ExposedProperty SegmentCountProperty = "SegmentCount";

    public override bool IsValid(VisualEffect component)
      => Source != null &&
         component.HasTexture(PositionMapProperty) &&
         component.HasUInt(VertexCountProperty) &&
         component.HasUInt(SegmentCountProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetTexture(PositionMapProperty, Source.PositionMap);
        component.SetUInt(VertexCountProperty, (uint)Source.VertexCount);
        component.SetUInt(SegmentCountProperty, (uint)Source.SegmentCount);
    }

    public override string ToString() => $"Wig : {PositionMapProperty}";
}

} // namespace BurstWig
