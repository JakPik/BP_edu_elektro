using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class GraphRenderer: VisualElement
{
    [UxmlAttribute]
    public float Time
    {
        get => _time;
        set => _time = value;
    }

    [UxmlAttribute]
    public float Amplitude
    {
        get => _amplitude;
        set => _amplitude = value;
    }

    [UxmlAttribute]
    public int PointDensity
    {
        get => _density;
        set => _density = value;
    }

    [UxmlAttribute]
    public float Frequency
    {
        get => _frequency;
        set => _frequency = value;
    }

    [UxmlAttribute]
    public bool Show
    {
        get => _show;
        set => _show = value;
    }

    private int _density;
    private float _time;
    private float _amplitude;
    private float _frequency;
    private bool _show;
    private Coroutine _curAnimation;

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

    public void VisualUpdate(bool show, float frequency, float amplitude, MonoBehaviour coroutineRunner)
    {
        _show = show;
        _frequency = frequency;
        _amplitude = amplitude;
        if(_show && _curAnimation == null)
        {
            _curAnimation = coroutineRunner.StartCoroutine(AnimateGraph());
        }
        if(!_show && _curAnimation != null)
        {
            coroutineRunner.StopCoroutine(_curAnimation);
            _curAnimation = null;
            MarkDirtyRepaint();
        }
    }

    private IEnumerator AnimateGraph()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            _time += 0.2f;
            MarkDirtyRepaint();
        }
    }

    private void Draw(MeshGenerationContext context)
    {
        if(!_show || _density < 2 || _frequency == 0) return;

        Rect rect = context.visualElement.contentRect;

        float width = rect.width;
        float height = rect.height;

        if(width == 0 || height == 0) return;


        points = new List<Vector2>(_density);


        float circle = 2 * Mathf.PI;
        float waveRation = width /(circle * _frequency);
        int waves = (int)(width / _frequency);
        float step = circle / (_density*_frequency-2);
        float position = 0;
        int pointOffset = 1;
        float amplitudeRatio = (_amplitude / 450);
        float amplitdueMaxHeight = height / 2;

        for(int j = 0; j < waves -1; j++) {
            for(int i = 0; i< _density*_frequency -1; i++)
            {
                if(position * waveRation > width + step) {
                    points.Add(new Vector2(position * waveRation, (Mathf.Sin(_time*_frequency + position)* amplitdueMaxHeight * amplitudeRatio) + amplitdueMaxHeight) );
                    pointOffset = 2;
                    break;
                }
                points.Add(new Vector2(position* waveRation, (Mathf.Sin(_time*_frequency + position)*amplitdueMaxHeight * amplitudeRatio) + amplitdueMaxHeight) );
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

        if (_density == 2)
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