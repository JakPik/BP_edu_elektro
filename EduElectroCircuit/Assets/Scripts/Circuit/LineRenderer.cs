/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: LineRenderer
 */

using System;
using Unity.VisualScripting;
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
    Z_LINE,
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
    public static void BuildLine(Node refference, PortData outPort, PortData inPort)
    {
        if (outPort.portLocalPosition == null)
        {
            Logger.Log(outPort, "Line Builder", "Out port does not have local position set", LogType.ERROR);
        }
        SplineContainer splineContainer = CreateLine(refference, outPort.portLocalPosition ?? Vector3.zero).GetComponent<SplineContainer>();
        Spline spline = splineContainer.Spline;
        PortOrientation orientation = CheckNodeOrientation(outPort, inPort);

        switch (orientation)
        {
            case PortOrientation.INLINE:
                InLineConstruction(spline, outPort.portPosition, inPort.portPosition);
                break;
            case PortOrientation.Z_LINE:
                ZLineConstruction(spline, outPort, inPort);
                break;
            case PortOrientation.CORNER:
                CornerLineConstruction(spline, outPort, inPort);
                break;
        }
    }

    private static void InLineConstruction(Spline spline, Vector3 outPort, Vector3 inPort)
    {
        AddKnot(spline, new Vector3(0, 0, 0));
        AddKnot(spline, inPort - outPort);
    }

    private static void ZLineConstruction(Spline spline, PortData outPort, PortData inPort)
    {
        Vector3 A = inPort.portPosition - outPort.portPosition;
        float length = ProjB_A(A, outPort.portDirection).magnitude / 2;

        AddKnot(spline, new Vector3(0, 0, 0));
        AddKnot(spline, outPort.portDirection * length);
        AddKnot(spline, A + inPort.portDirection * length);
        AddKnot(spline, inPort.portPosition - outPort.portPosition);
    }

    private static Vector3 ProjB_A(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b) * b;
    }

    private static void CornerLineConstruction(Spline spline, PortData outPort, PortData inPort)
    {
        Vector2 A1 = new Vector2(inPort.portDirection.x, inPort.portDirection.z);
        Vector2 A2 = new Vector2(-outPort.portDirection.x, -outPort.portDirection.z);
        Vector2 B = new Vector2(outPort.portPosition.x, outPort.portPosition.z) - new Vector2(inPort.portPosition.x, inPort.portPosition.z);
        float DetA = CalculateDet(A1, A2);
        if (DetA == 0)
        {
            InLineConstruction(spline, outPort.portPosition, inPort.portPosition);
            return;
        }
        Vector2 X = -A2 * (CalculateDet(A1, B) / DetA);
        AddKnot(spline, new Vector3(0, 0, 0));
        AddKnot(spline, new Vector3(X.x, outPort.portPosition.y, X.y));
        AddKnot(spline, inPort.portPosition - outPort.portPosition);
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

    private static PortOrientation CheckNodeOrientation(PortData outPort, PortData inPort)
    {
        if (outPort.portDirection.y != 0 || inPort.portDirection.y != 0) return PortOrientation.INLINE;
        float b = Math.Clamp(Math.Abs(Vector3.Dot(inPort.portDirection, outPort.portDirection)), -1f, 1f);
        if (b != 1)
        {
            return PortOrientation.CORNER;
        }
        Vector3 v = (inPort.portPosition - outPort.portPosition).normalized;
        float a = Math.Clamp(Vector3.Dot(v, outPort.portDirection), -1f, 1f);
        if (Math.Abs(a) == 1)
        {
            return PortOrientation.INLINE;
        }
        else
        {
            return PortOrientation.Z_LINE;
        }
    }
}