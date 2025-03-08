using UnityEngine;
using UnityEditor;
using KlutterTools.InspectorUtils;

namespace BurstWig {

[CustomEditor(typeof(WigController))]
sealed class WigControllerEditor : Editor
{
    AutoProperty _source        = null;
    AutoProperty _segmentCount  = null;
    AutoProperty _randomSeed    = null;

    AutoProperty _profile__length           = null;
    AutoProperty _profile__lengthRandomness = null;
    AutoProperty _profile__spring           = null;
    AutoProperty _profile__damping          = null;
    AutoProperty _profile__gravity          = null;
    AutoProperty _profile__noiseAmplitude   = null;
    AutoProperty _profile__noiseFrequency   = null;
    AutoProperty _profile__noiseSpeed       = null;

    static class Labels
    {
        public static LabelString Randomness = "Randomness";
        public static LabelString NoiseField = "Noise Field";
        public static LabelString Frequency  = "Frequency";
        public static LabelString Amplitude  = "Amplitude";
        public static LabelString Speed      = "Speed";
    }

    void OnEnable()
      => AutoProperty.Scan(this);

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_source);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_segmentCount);
        EditorGUILayout.PropertyField(_randomSeed);

        EditorGUILayout.Space();

        EditorGUILayout.Slider(_profile__length, 0.1f, 5);
        EditorGUI.indentLevel++;
        EditorGUILayout.Slider(_profile__lengthRandomness, 0, 1, Labels.Randomness);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_profile__spring);
        EditorGUILayout.PropertyField(_profile__damping);
        EditorGUILayout.PropertyField(_profile__gravity);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(Labels.NoiseField);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_profile__noiseFrequency, Labels.Frequency);
        EditorGUILayout.PropertyField(_profile__noiseAmplitude, Labels.Amplitude);
        EditorGUILayout.PropertyField(_profile__noiseSpeed, Labels.Speed);
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace BurstWig
