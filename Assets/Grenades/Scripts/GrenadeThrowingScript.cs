using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrowingScript : MonoBehaviour {

    const string BUTTON_THROW = "GrenadeThrowing";
    const int POSITIONS_COUNT = 100;

    public GameObject prefab;
    public GameObject spawn;
    public LineRenderer predictedTrajectory;
    public GameObject intersectionPrefab;
    [Range(1, 1000)]
    public float forceMultiply = 1;

    public float maxForce = 100f;
    public float minForce = 1f;
    float force;
    GrenadeScript grenadeScript;

    public float Force
    {
        get { return force; }
        set { force = value > maxForce ? maxForce : value;  }
    }
    
    bool trowing;

    void OnEnabled()
    {
        Force = minForce;
        trowing = false;
    }

	void Update () {
        if (Input.GetButtonDown(BUTTON_THROW))
        {
            trowing = true;
            Force = minForce;
            grenadeScript = prefab.GetComponent<GrenadeScript>();
        }

        if (Input.GetButton(BUTTON_THROW))
        {
            Force += Time.deltaTime * forceMultiply;
            PredictTrajectory();
        }
		
        if (Input.GetButtonUp(BUTTON_THROW))
        {
            Throw();
            trowing = false;
            Force = minForce;
            if(intersectionPrefab) intersectionPrefab.SetActive(trowing);
            predictedTrajectory.SetPositions(new Vector3[POSITIONS_COUNT]);
        }

        
    }

    GameObject Throw()
    {
        GameObject grenade = Instantiate(prefab, spawn.transform);
        Rigidbody rigidbody = grenade.GetComponent<Rigidbody>();
        rigidbody.velocity = GrenadeVelosity();

        return grenade;
    }

    Vector3 GrenadeVelosity()
    {
        return (Vector3.forward + Vector3.up) * Force;
    }

    void PredictTrajectory()
    {
        predictedTrajectory.transform.position = spawn.transform.position;
        predictedTrajectory.positionCount = POSITIONS_COUNT;

        Vector3[] points = new Vector3[POSITIONS_COUNT];
        int arrayCount = 0;
        bool hittedCollider = false;

        for (int i = 0; i < points.Length; i++)
        {
            float time = (i + 1) * 0.05f;

            points[i] = (spawn.transform.position + GrenadeVelosity() * time + Physics.gravity * time * time / 2f);
            arrayCount = i + 1;

            if (i == 0) continue;
            RaycastHit hit;
            if (Physics.Linecast(points[i - 1], points[i], out hit))
            {
                if (hit.collider.GetComponent<GrenadeScript>()) continue;
                hittedCollider = true;
                points[i] = hit.point;
                if (intersectionPrefab)
                {

                    intersectionPrefab.transform.position = hit.point;
                    intersectionPrefab.transform.localRotation = Quaternion.LookRotation(hit.point, hit.normal);
                    intersectionPrefab.transform.localScale = IntersectionPrefabScale();
                }
                break;
            }
        }
        if (intersectionPrefab) intersectionPrefab.SetActive(hittedCollider);

        Vector3[] newArray = new Vector3[arrayCount];
        System.Array.Copy(points, newArray, arrayCount);

        predictedTrajectory.positionCount = arrayCount;
        predictedTrajectory.SetPositions(newArray);
    }

    Vector3 IntersectionPrefabScale()
    {
        float radius = grenadeScript.forceRadius * 2f;
        return new Vector3(radius, radius, radius);
    }
}