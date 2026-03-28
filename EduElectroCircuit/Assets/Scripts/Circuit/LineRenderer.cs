using System;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Describes the relative orientation of two ports based on their direction and position.
/// </summary>
public enum PortOrientation
{
    /// <summary>
    /// Ports have collinear direction vectors and are aligned.
    /// </summary>
    INLINE,
    /// <summary>
    /// Ports have collinear direction vectors and are not aligned.
    /// </summary>
    STRAIGHT,
    /// <summary>
    /// Ports have non-collinear direction vectors, forming an angle.
    /// </summary>
    CORNER
}

/// <summary>
/// Building visible connection between connected nodes.
/// It creates GameObject called 'Line' and sets it as a child of the caller node.
/// </summary>
/// <remarks>
/// Assumes the existence of OutPort and InPort in the defined PortOrientation.
/// Handles some edge cases automatically, but manual adjustment may be required
/// to shape the created line in certain situations.
/// </remarks>
public static class LineRenderer
{
    public static void BuildLine(Node refference, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        SplineContainer splineContainer = CreateLine(refference, outPort.Item3).GetComponent<SplineContainer>();
        Spline spline = splineContainer.Spline;
        PortOrientation orientation = CheckNodeOrientation(outPort, inPort);

        switch (orientation)
        {
            case PortOrientation.INLINE:
                InLineConstruction(spline, outPort.Item2, inPort.Item2);
                break;
            case PortOrientation.STRAIGHT:
                StraightLineConstruction(spline, outPort, inPort);
                break;
            case PortOrientation.CORNER:
                CornerLineConstruction(spline, outPort, inPort);
                break;
        }
    }

    private static void InLineConstruction(Spline spline, Vector3 outPort, Vector3 inPort)
    {
        AddKnot(spline, new Vector3(0,0,0));
        AddKnot(spline, inPort - outPort);
    }

    private static void StraightLineConstruction(Spline spline, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        Vector2 B = new Vector2(inPort.Item2.x, inPort.Item2.z) - new Vector2(outPort.Item2.x, outPort.Item2.z);
        Vector2 A2 = new Vector2(outPort.Item1.x, outPort.Item1.z);

        B = B/ 2;
        Vector2 X = B * A2;
        AddKnot(spline, new Vector3(0,0,0));
        AddKnot(spline, new Vector3(X.x, outPort.Item2.y, X.y));
        if(X.y == 0) {
            AddKnot(spline, new Vector3(X.x, outPort.Item2.y, B.y*2));
        }
        else
        {
            AddKnot(spline, new Vector3(B.x * 2, outPort.Item2.y, X.y));
        }
        AddKnot(spline, inPort.Item2 - outPort.Item2);
    }

    private static void CornerLineConstruction(Spline spline, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        Vector2 A1 = new Vector2(inPort.Item1.x, inPort.Item1.z);
        Vector2 A2 = new Vector2(-outPort.Item1.x, -outPort.Item1.z);
        Vector2 B = new Vector2(outPort.Item2.x, outPort.Item2.z) - new Vector2(inPort.Item2.x, inPort.Item2.z);
        float DetA = CalculateDet(A1, A2);
        if(DetA == 0)
        {
            InLineConstruction(spline,outPort.Item2, inPort.Item2);
            return;
        }
        Vector2 X = -A2 * (CalculateDet(A1, B)/DetA);
        AddKnot(spline, new Vector3(0,0,0));
        AddKnot(spline, new Vector3(X.x, outPort.Item2.y, X.y));
        AddKnot(spline, inPort.Item2 - outPort.Item2);
    }

    private static float CalculateDet(Vector2 M1, Vector2 M2)
    {
        return M1.x * M2.y - M1.y * M2.x;
    }

    private static void AddKnot(Spline spline, Vector3 vector)
    {
        BezierKnot knot = new BezierKnot(vector);
        spline.Add(knot);
    }

    private static GameObject CreateLine(Node refference, Vector3 startPos)
    {
        GameObject line = new GameObject();
        line.AddComponent<SplineContainer>();
        line.AddComponent<SplineExtrude>();
        SplineExtrude extrude = line.GetComponent<SplineExtrude>();
        extrude.Sides = 32;
        extrude.RebuildFrequency = 1;
        extrude.Radius = 0.05f;
        extrude.SegmentsPerUnit = 16;
        line.transform.parent = refference.transform;
        line.transform.localPosition = startPos;
        line.transform.rotation = Quaternion.identity;
        line.name = "Line";
        return line;
    }

    private static PortOrientation CheckNodeOrientation((Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        float b = Math.Clamp(Math.Abs(Vector3.Dot(inPort.Item1, outPort.Item1)), -1f,1f);
        if(b != 1)
        {
            return PortOrientation.CORNER;
        }
        Vector3 v = (inPort.Item2 - outPort.Item2).normalized;
        float a = Math.Clamp(Vector3.Dot(v, outPort.Item1), -1f,1f);
        if(Math.Abs(a) == 1)
        {
            return PortOrientation.INLINE;
        }
        else {
            return PortOrientation.STRAIGHT;
        }
    }
}