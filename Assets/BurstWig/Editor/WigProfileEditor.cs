using UnityEngine;
using UnityEditor;

namespace BurstWig
{
    [CustomEditor(typeof(WigProfile))]
    sealed class WigProfileEditor : Editor
    {
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

            _length           = finder["length"];
            _lengthRandomness = finder["lengthRandomness"];
            _spring           = finder["spring"];
            _damping          = finder["damping"];
            _gravity          = finder["gravity"];
            _noiseAmplitude   = finder["noiseAmplitude"];
            _noiseFrequency   = finder["noiseFrequency"];
            _noiseSpeed       = finder["noiseSpeed"];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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

        [MenuItem("Assets/Create/BurstWig/Wig Profile")]
        public static void CreateWigProfileAsset()
        {
            var asset = ScriptableObject.CreateInstance<WigProfile>();
            ProjectWindowUtil.CreateAsset(asset, "New Wig Profile.asset");
        }
    }
}
