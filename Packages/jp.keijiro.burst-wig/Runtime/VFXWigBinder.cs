using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace BurstWig {

[AddComponentMenu("VFX/Property Binders/BurstWig/Wig Binder")]
[VFXBinder("Wig")]
public sealed class VFXWigBinder : VFXBinderBase
{
    public WigController Source = null;

    public string PositionMapProperty
      { get => (string)_positionMapProperty;
        set => _positionMapProperty = value; }

    public string VertexCountProperty
      { get => (string)_vertexCountProperty;
        set => _vertexCountProperty = value; }

    public string SegmentCountProperty
      { get => (string)_segmentCountProperty;
        set => _segmentCountProperty = value; }

    [VFXPropertyBinding("UnityEngine.Texture"), SerializeField]
    ExposedProperty _positionMapProperty = "PositionMap";

    [VFXPropertyBinding("System.UInt32"), SerializeField]
    ExposedProperty _vertexCountProperty = "VertexCount";

    [VFXPropertyBinding("System.UInt32"), SerializeField]
    ExposedProperty _segmentCountProperty = "SegmentCount";

    public override bool IsValid(VisualEffect component)
      => Source != null &&
         component.HasTexture(_positionMapProperty) &&
         component.HasUInt(_vertexCountProperty) &&
         component.HasUInt(_segmentCountProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetTexture(_positionMapProperty, Source.PositionMap);
        component.SetUInt(_vertexCountProperty, (uint)Source.VertexCount);
        component.SetUInt(_segmentCountProperty, (uint)Source.SegmentCount);
    }

    public override string ToString() => $"Wig : {_positionMapProperty}";
}

} // namespace BurstWig
