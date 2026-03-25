using System;
using UnityEngine;
using UnityEngine.Splines;

public enum Port_Orientation
{
    INLINE,
    STRAIGHT,
    CORNER
}
public static class LineRenderer
{
    public static void BuildLine(Node refference, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        SplineContainer splineContainer = CreateLine(refference, outPort.Item3).GetComponent<SplineContainer>();
        Spline spline = splineContainer.Spline;
        Port_Orientation orientation = CheckNodeOrientation(outPort, inPort);

        switch (orientation)
        {
            case Port_Orientation.INLINE:
                InLineConstruction(spline, outPort.Item2, inPort.Item2);
                break;
            case Port_Orientation.STRAIGHT:
                StraightLineConstruction(spline, outPort, inPort);
                break;
            case Port_Orientation.CORNER:
                CornerLineConstruction(spline, outPort, inPort);
                break;
        }
    }

    private static void InLineConstruction(Spline spline, Vector3 outPort, Vector3 inPort)
    {
        BezierKnot knot = new BezierKnot(new Vector3(0,0,0));
        spline.Add(knot);
        knot = new BezierKnot(inPort - outPort);
        spline.Add(knot);
    }

    private static void StraightLineConstruction(Spline spline, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        Vector2 B = new Vector2(inPort.Item2.x, inPort.Item2.z) - new Vector2(outPort.Item2.x, outPort.Item2.z);
        B = B/ 2;
        Vector2 A2 = new Vector2(outPort.Item1.x, outPort.Item1.z);
         Vector2 A1 = new Vector2(inPort.Item1.x, inPort.Item1.z);
        Vector2 X = B * A2;
        BezierKnot knot = new BezierKnot(new Vector3(0,0,0));
        spline.Add(knot);
        if(X.y == 0) {
            knot = new BezierKnot(new Vector3(X.x, outPort.Item2.y, X.y));
            spline.Add(knot);
            knot = new BezierKnot(new Vector3(X.x, outPort.Item2.y, B.y*2));
            spline.Add(knot);
        }
        else
        {
            knot = new BezierKnot(new Vector3(X.x, outPort.Item2.y, X.y));
            spline.Add(knot);
            knot = new BezierKnot(new Vector3(B.x * 2, outPort.Item2.y, X.y));
            spline.Add(knot);
        }
        knot = new BezierKnot(inPort.Item2 - outPort.Item2);
        spline.Add(knot);
    }

    private static void CornerLineConstruction(Spline spline, (Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        Vector2 A1 = new Vector2(inPort.Item1.x, inPort.Item1.z);
        Vector2 A2 = new Vector2(-outPort.Item1.x, -outPort.Item1.z);
        Vector2 B = new Vector2(inPort.Item2.x, inPort.Item2.z) - new Vector2(outPort.Item2.x, outPort.Item2.z);
        float DetA = CalculateDet(A1, A2);
        if(DetA == 0)
        {
            InLineConstruction(spline,outPort.Item2, inPort.Item2);
            return;
        }
        Vector2 X = new Vector2(CalculateDet(B,A2)/DetA, CalculateDet(A1,B)/DetA);
        X *= A2;
        BezierKnot knot = new BezierKnot(new Vector3(0,0,0));
        spline.Add(knot);
        knot = new BezierKnot(new Vector3(X.x, outPort.Item2.y, X.y));
        spline.Add(knot);
        knot = new BezierKnot(inPort.Item2 - outPort.Item2);
        spline.Add(knot);
        
    }

    private static float CalculateDet(Vector2 M1, Vector2 M2)
    {
        return M1.x * M2.y - M1.y * M2.x;
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

    private static Port_Orientation CheckNodeOrientation((Vector3, Vector3, Vector3) outPort, (Vector3, Vector3) inPort)
    {
        if(Math.Abs(Vector3.Dot(inPort.Item1, outPort.Item1)) != 1)
        {
            return Port_Orientation.CORNER;
        }
        Vector3 v = (inPort.Item2 - outPort.Item2).normalized;
        float a = Vector3.Dot(v, outPort.Item1);
        if(Math.Abs(a) == 1)
        {
            return Port_Orientation.INLINE;
        }
        else {
            return Port_Orientation.STRAIGHT;
        }
    }
}