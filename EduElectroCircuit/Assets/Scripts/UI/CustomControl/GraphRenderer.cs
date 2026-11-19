using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GraphRenderer: VisualElement
{
    [UxmlAttribute]
    public float Time
    {
        get => time;
        set => time = value;
    }

    [UxmlAttribute]
    public float Amplitude
    {
        get => amplitude;
        set => amplitude = value;
    }

    [UxmlAttribute]
    public int PointDensity
    {
        get => density;
        set => density = value;
    }

    [UxmlAttribute]
    public float Frequency
    {
        get => frequency;
        set => frequency = value;
    }

    private int density;

    private float time;

    private float amplitude;
    private float frequency;

    public GraphRenderer()
    {
        generateVisualContent += Draw;
    }

    private List<Vector2> points;

    public void SetPoints(
        Vector2 start,
        Vector2 end,
        Vector2 control1,
        Vector2 control2)
    {


        MarkDirtyRepaint();
    }

    private void Draw(MeshGenerationContext context)
    {
        if (density < 2)
            return;

        Rect rect = context.visualElement.contentRect;

        float width = rect.width;
        float height = rect.height;


        points = new List<Vector2>(density);


        float circle = 2 * Mathf.PI;
        float waveRation = width /(circle * frequency);
        int waves = (int)(width / frequency);
        float step = circle / (density*frequency-2);
        float position = 0;
        int pointOffset = 1;
        float amplitudeRatio = (amplitude / 450);
        float amplitdueMaxHeight = height / 2;

        for(int j = 0; j < waves -1; j++) {
            for(int i = 0; i< density*frequency -1; i++)
            {
                if(position * waveRation > width + step) {
                    points.Add(new Vector2(position * waveRation, (Mathf.Sin(time*frequency + position)* amplitdueMaxHeight * amplitudeRatio) + amplitdueMaxHeight) );
                    pointOffset = 2;
                    break;
                }
                points.Add(new Vector2(position* waveRation, (Mathf.Sin(time*frequency + position)*amplitdueMaxHeight * amplitudeRatio) + amplitdueMaxHeight) );
                position += step;
            }
            if(position* waveRation > width+step) break;
        }

        var painter = context.painter2D;

        painter.lineWidth = 3;
        painter.strokeColor = Color.cyan;
        painter.lineCap = LineCap.Round;
        painter.lineJoin = LineJoin.Round;

        painter.BeginPath();
        painter.MoveTo(points[0]);

        if (density == 2)
        {
            painter.LineTo(points[1]);
        }
        else
        {
            for (int i = 0; i < points.Count - pointOffset; i++)
            {
                
                Vector2 p0 = points[Mathf.Max(i - 1, 0)];
                Vector2 p1 = points[i];
                Vector2 p2 = points[i + 1];
                Vector2 p3 = points[Mathf.Min(i + 2, points.Count - 1)];

                

                // Catmull-Rom segment converted to cubic Bezier
                Vector2 control1 = p1 + (p2 - p0) / 6f;
                Vector2 control2 = p2 - (p3 - p1) / 6f;

                painter.BezierCurveTo(
                    control1,
                    control2,
                    p2
                );
            }
        }

        painter.Stroke();

    }

    private void DrawPoint(Painter2D painter, Vector2 position)
    {
        painter.fillColor = Color.white;
        
        position = new Vector2(position.x, 0);
        painter.BeginPath();
        painter.Arc(position, 5, 0, 360);
        painter.Fill();
    }
}