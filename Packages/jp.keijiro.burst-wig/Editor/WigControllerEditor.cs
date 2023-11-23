using UnityEngine;
using UnityEditor;

namespace BurstWig {

[CustomEditor(typeof(WigController))]
sealed class WigControllerEditor : Editor
{
    SerializedProperty _source;
    SerializedProperty _segmentCount;
    SerializedProperty _randomSeed;

    SerializedProperty _length;
    SerializedProperty _lengthRandomness;
    SerializedProperty _spring;
    SerializedProperty _damping;
    SerializedProperty _gravity;
    SerializedProperty _noiseAmplitude;
    SerializedProperty _noiseFrequency;
    SerializedProperty _noiseSpeed;

    static class Labels
    {
        public static Label Randomness = "Randomness";
        public static Label NoiseField = "Noise Field";
        public static Label Frequency  = "Frequency";
        public static Label Amplitude  = "Amplitude";
        public static Label Speed      = "Speed";
    }

    void OnEnable()
    {
        var finder = new PropertyFinder(serializedObject);

        _source       = finder["_source"];
        _segmentCount = finder["_segmentCount"];
        _randomSeed   = finder["_randomSeed"];

        _length           = finder["_profile.length"];
        _lengthRandomness = finder["_profile.lengthRandomness"];
        _spring           = finder["_profile.spring"];
        _damping          = finder["_profile.damping"];
        _gravity          = finder["_profile.gravity"];
        _noiseAmplitude   = finder["_profile.noiseAmplitude"];
        _noiseFrequency   = finder["_profile.noiseFrequency"];
        _noiseSpeed       = finder["_profile.noiseSpeed"];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_source);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_segmentCount);
        EditorGUILayout.PropertyField(_randomSeed);

        EditorGUILayout.Space();

        EditorGUILayout.Slider(_length, 0.1f, 5);
        EditorGUI.indentLevel++;
        EditorGUILayout.Slider(_lengthRandomness, 0, 1, Labels.Randomness);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_spring);
        EditorGUILayout.PropertyField(_damping);
        EditorGUILayout.PropertyField(_gravity);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(Labels.NoiseField);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_noiseFrequency, Labels.Frequency);
        EditorGUILayout.PropertyField(_noiseAmplitude, Labels.Amplitude);
        EditorGUILayout.PropertyField(_noiseSpeed, Labels.Speed);
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace BurstWig
