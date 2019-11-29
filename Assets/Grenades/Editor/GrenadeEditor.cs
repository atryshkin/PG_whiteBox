using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrenadeScript))]
public class GrenadeEditor : Editor
{
    GrenadeScript grenadeScript;
    void OnEnable()
    {
        grenadeScript = (GrenadeScript)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (grenadeScript.grenadeType == GrenadeScript.GrenadeType.Chemical)
        {
            grenadeScript.chemicalDamageDuration = EditorGUILayout.FloatField("Chemical Damage Duration:", grenadeScript.chemicalDamageDuration);
        }
    }
}