using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GrenadeScript : MonoBehaviour {
    public enum GrenadeType { Explosive, Chemical };
    public GrenadeType grenadeType = GrenadeType.Explosive;
    public GameObject explosionPrefab;
    public float force = 1f;
    public float forceRadius = 1f;
    [HideInInspector]
    public float chemicalDamageDuration = 5f;
    [Range(0,100)]
    public float explosionLiveTime = 1f;
    public float damage = 100f;

    void Explode() {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        AddExplosionForce();

        Destroy(explosion, explosionLiveTime);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void AddExplosionForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, forceRadius);

        for (int i = 0; i < colliders.Length; i++)
        {

            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody) continue;
            targetRigidbody.AddExplosionForce(force, transform.position, forceRadius);

            Combat combat = targetRigidbody.GetComponent<Combat>();
            if (!combat) continue;

            MakeDamage(combat, targetRigidbody.position);
        }
    }

    void MakeDamage(Combat combat, Vector3 position)
    {
        switch (grenadeType)
        {
            case GrenadeType.Explosive:
                float calculatedDamage = CalculateDamage(position);
                combat.MakeDamage(calculatedDamage);
                break;
            case GrenadeType.Chemical:
                combat.MakePeriodDamage(damage, chemicalDamageDuration);
                break;
        }
        
    }

    float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (forceRadius - explosionDistance) / forceRadius;
        float calculatedDamage = relativeDistance * damage;
        calculatedDamage = Mathf.Max(0f, calculatedDamage);

        return calculatedDamage;
    }
}

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
        if(grenadeScript.grenadeType == GrenadeScript.GrenadeType.Chemical) {
            grenadeScript.chemicalDamageDuration = EditorGUILayout.FloatField("Chemical Damage Duration:", grenadeScript.chemicalDamageDuration);
        }
    }
}