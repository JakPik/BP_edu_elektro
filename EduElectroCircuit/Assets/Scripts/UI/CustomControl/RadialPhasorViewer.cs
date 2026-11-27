using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlObject]
public partial class Phasor
{
    [UxmlAttribute]
    public Color phasorColor;

    [UxmlAttribute]
    public Vector2 phasorPosition;
}

[UxmlElement]
public partial class RadialPhasorViewer: VisualElement
{
    private const float CenterRadius = 0.8f;

    [UxmlObjectReference("phasor")]
    public Phasor phasors;


    [UxmlAttribute]
    public float ThinLine
    {
        get => _guideLineThickness;
        set => _guideLineThickness = value;
    }
    private float _guideLineThickness = 0.5f;
    [UxmlAttribute]
    public float NormalLine
    {
        get => _phasorLineThickness;
        set => _phasorLineThickness = value;
    }
    private float _phasorLineThickness = 1.0f;
    [UxmlAttribute]
    public float ThickLine
    {
        get => _ringLineThickness;
        set => _ringLineThickness = value;
    }
    private float _ringLineThickness = 2.0f;

    [UxmlAttribute]
    public float Radius
    {
        get => _radius;
        set => _radius = value;
    }
    private float _radius;

    [UxmlAttribute]
    public int Rings
    {
        get => _rings;
        set => _rings = value;
    }
    private int _rings;


    [UxmlAttribute]
    public float GuidesOutreach
    {
        get => _guidesOutreach;
        set => _guidesOutreach = value;
    }
    private float _guidesOutreach;

    [UxmlAttribute]
    public float LabelOutreach
    {
        get => _labelOutreach;
        set => _labelOutreach = value;
    }
    private float _labelOutreach;

    [UxmlAttribute]
    public float LabelSize
    {
        get => _labelSize;
        set => _labelSize = value;
    }
    private float _labelSize = 10f;

    [UxmlAttribute]
    public Color LabelColor
    {
        get => _labelColor;
        set => _labelColor = value;
    }
    private Color _labelColor;


    

    [UxmlAttribute]
    public Color RingColor
    {
        get => _ringColor;
        set => _ringColor = value;
    }
    private Color _ringColor;




    [UxmlAttribute]
    public Vector2 VoltagePhasorsPosition
    {
        get => _voltagePhasorsPosition;
        set => _voltagePhasorsPosition = value;
    }
    private Vector2 _voltagePhasorsPosition;

    [UxmlAttribute]
    public Color VoltagePhasorsColor
    {
        get => _voltagePhasorsColor;
        set => _voltagePhasorsColor = value;
    }
    private Color _voltagePhasorsColor;


    [UxmlAttribute]
    public Vector2 CurrentPhasorsPosition
    {
        get => _currentPhasorsPosition;
        set => _currentPhasorsPosition = value;
    }
    private Vector2 _currentPhasorsPosition;

    [UxmlAttribute]
    public Color CurrentPhasorsColor
    {
        get => _currentPhasorsColor;
        set => _currentPhasorsColor = value;
    }
    private Color _currentPhasorsColor;

    [UxmlAttribute]
    public bool Visible
    {
        get => _visible;
        set => _visible = value;
    }
    private bool _visible;
    private List<Vector2> _radialPoints;
    private List<Label> _labels = new List<Label>
    {
        new Label("0°"),
        new Label("90°"),
        new Label("180°"),
        new Label("270°")
    };

    public RadialPhasorViewer()
    {
        
        generateVisualContent += Draw;
        CreatePoints();
        //AddLabels();
    }

    public void UpdateVisibility(bool visible, ComplexFloat voltage, ComplexFloat current)
    {
        _visible = visible;
        _voltagePhasorsPosition = new Vector2(voltage.real, -voltage.imaginary);
        _currentPhasorsPosition = new Vector2(current.real, -current.imaginary);
        MarkDirtyRepaint();
    }

    private void CreatePoints()
    {
        float PI = (float)Math.PI;
        _radialPoints = new List<Vector2>
        {
            new Vector2((float)Math.Cos(0), -(float)Math.Sin(0)),
            new Vector2((float)Math.Cos(PI / 6), -(float)Math.Sin(PI / 6)),
            new Vector2((float)Math.Cos(PI / 4), -(float)Math.Sin(PI / 4)),
            new Vector2((float)Math.Cos(PI / 3), -(float)Math.Sin(PI / 3)),
            new Vector2((float)Math.Cos(PI / 2), -(float)Math.Sin(PI / 2)),
            new Vector2(-(float)Math.Cos(PI / 3), -(float)Math.Sin(PI / 3)),
            new Vector2(-(float)Math.Cos(PI / 4), -(float)Math.Sin(PI / 4)),
            new Vector2(-(float)Math.Cos(PI / 6), -(float)Math.Sin(PI / 6)),

            new Vector2((float)Math.Cos(PI), -(float)Math.Sin(PI)),

            new Vector2(-(float)Math.Cos(PI / 6), (float)Math.Sin(PI / 6)),
            new Vector2(-(float)Math.Cos(PI / 4), (float)Math.Sin(PI / 4)),
            new Vector2(-(float)Math.Cos(PI / 3), (float)Math.Sin(PI / 3)),
            new Vector2(-(float)Math.Cos(PI / 2), (float)Math.Sin(PI / 2)),
            new Vector2((float)Math.Cos(PI / 3), (float)Math.Sin(PI / 3)),
            new Vector2((float)Math.Cos(PI / 4), (float)Math.Sin(PI / 4)),
            new Vector2((float)Math.Cos(PI / 6), (float)Math.Sin(PI / 6)),
        };
    }

    private void AddLabels()
    {
        foreach(Label label in _labels)
        {
            label.style.position = Position.Absolute;
            label.style.paddingBottom = 0;
            label.style.paddingLeft = 0;
            label.style.paddingRight = 0;
            label.style.paddingTop = 0;
            label.style.marginBottom = 0;
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            label.style.marginTop = 0;

            label.style.translate = new Translate(
                Length.Percent(-50),
                Length.Percent(-50)
            );

            Add(label);
        }
    }

    private void Draw(MeshGenerationContext context)
    {
        Rect rect = context.visualElement.contentRect;

        float width = rect.width;
        float height = rect.height;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        Vector2 center = new Vector2(halfWidth, halfHeight);

        var painter = context.painter2D;

        DrawRadialGuides(painter, center, _radius);

        DrawRadialCircles(painter, center, _radius);
        
        DrawPhasors(painter, center, _radius);

        painter.strokeColor = Color.grey;
        painter.fillColor = Color.grey;
        painter.BeginPath();
        painter.Arc(center, CenterRadius, 0f, 360f);
        painter.Fill();

    }

    private void DrawPhasors(Painter2D painter, Vector2 center, float radius)
    {
        if(!_visible) return;
        painter.lineWidth = _phasorLineThickness * 2;
        
        Vector2 direction = _voltagePhasorsPosition.normalized;
        Vector2 tangent = new Vector2(direction.y, -direction.x);
   
        painter.strokeColor = _voltagePhasorsColor;
        painter.fillColor = _voltagePhasorsColor;
        painter.BeginPath();
        painter.MoveTo(center - tangent * CenterRadius);
        painter.LineTo(center + tangent * CenterRadius);
        painter.LineTo(center + _voltagePhasorsPosition.normalized * radius + tangent * _phasorLineThickness);
        painter.LineTo(center + _voltagePhasorsPosition.normalized * radius - tangent * _phasorLineThickness);
        painter.Fill();


        direction = _currentPhasorsPosition.normalized;
        tangent = new Vector2(direction.y, -direction.x);
        painter.lineWidth = _phasorLineThickness;
        float offsetBaseSize = CenterRadius - (CenterRadius / 2);
        float offsetTipSize = _phasorLineThickness - (_phasorLineThickness / 2);

        painter.strokeColor = _currentPhasorsColor;
        painter.fillColor = _currentPhasorsColor;
        painter.BeginPath();
        painter.MoveTo(center - tangent * offsetBaseSize);
        painter.LineTo(center + tangent * offsetBaseSize);
        painter.LineTo(center + _currentPhasorsPosition.normalized * radius + tangent * offsetTipSize);
        painter.LineTo(center + _currentPhasorsPosition.normalized * radius - tangent * offsetTipSize);
        painter.Fill();
    }

    private void DrawRadialCircles(Painter2D painter, Vector2 center, float radius)
    {
        float step = radius / _rings;
        painter.strokeColor = _ringColor;
        for(int i = 0; i < _rings; i++) {
            painter.BeginPath();
            painter.lineWidth = _ringLineThickness;
            painter.Arc(
                center,
                _radius - i * step,
                0f,
                360f
            );

            painter.ClosePath();
            painter.Stroke();
        }
    }

    private void DrawRadialGuides(Painter2D painter, Vector2 center, float radius)
    {
        painter.lineWidth = _guideLineThickness;
        painter.strokeColor = Color.gray;
        painter.BeginPath();
        float lenght = radius + _guidesOutreach;
        int id = 0;

        foreach(Vector2 point in _radialPoints)
        {
            painter.MoveTo(center);
            painter.LineTo(center + point * lenght);
           /* if(id % 4 == 0) {
                _labels[id/4].style.color = _labelColor;
                _labels[id/4].style.fontSize = _labelSize;

                Vector2 labelPosition = point * (_labelOutreach + radius);

                _labels[id/4].style.left = center.x + labelPosition.x;
                _labels[id/4].style.top = center.y + labelPosition.y;
            }
            id++;*/
        }
        painter.Stroke();

    }
}
