using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrenadeThrowingScript))]
public class GrenadeThrowingEditor : Editor
{
    GrenadeThrowingScript grenadeThrowing;
    void OnEnable()
    {
        grenadeThrowing = (GrenadeThrowingScript)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (grenadeThrowing.forceChanging)
        {
            grenadeThrowing.maxForce = EditorGUILayout.FloatField("Max Force", grenadeThrowing.maxForce);
            grenadeThrowing.minForce = EditorGUILayout.FloatField("Min Force", grenadeThrowing.minForce);
            grenadeThrowing.forceMultiply = EditorGUILayout.FloatField("Force Multiply", grenadeThrowing.forceMultiply);
        }
        else
        {
            grenadeThrowing.maxForce = EditorGUILayout.FloatField("Force", grenadeThrowing.maxForce);
        }
    }
}