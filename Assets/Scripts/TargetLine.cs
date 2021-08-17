using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLine : MonoBehaviour
{
    // Creates a line renderer that follows a Sin() function
    // and animates it.

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
    public CharacterLogic.Character ParentObject;
    public AnimationCurve Curve;

    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        string ParentObjectName = transform.root.gameObject.name;
        ParentObject = CharacterLogic.playerTeam.Find(e => e.Name == ParentObjectName);
    }

    void Update()
    {

        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        var points = new Vector3[lengthOfLineRenderer];
        var t = Time.time;
        if (ParentObject.CurrentTarget)
        {
            for (int i = 0; i < lengthOfLineRenderer; i++)
            {
                points[i] = new Vector3(
                    (this.transform.position + (i * (1f / lengthOfLineRenderer)) * (ParentObject.CurrentTarget.transform.position - this.transform.position)).x,
                    (this.transform.position + (i * (1f / lengthOfLineRenderer)) * (ParentObject.CurrentTarget.transform.position - this.transform.position)).y + (Curve.Evaluate((float)i / lengthOfLineRenderer)) * 5,
                    (this.transform.position + (i * (1f / lengthOfLineRenderer)) * (ParentObject.CurrentTarget.transform.position - this.transform.position)).z
                );
            }
            lineRenderer.SetPositions(points);
        }

    }
}
