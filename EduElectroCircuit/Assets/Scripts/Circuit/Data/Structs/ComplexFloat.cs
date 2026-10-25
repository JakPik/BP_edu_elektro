using System;

[System.Serializable]
public struct ComplexFloat
{
    public float real;
    public float imaginary;

    public bool infinite {get; private set;}
    private const float Epsilon = 1e-3f;

    public ComplexFloat(float real, float imaginary, bool infinite = false)
    {
        this.real = real;
        this.imaginary = imaginary;
        this.infinite = infinite;
    }

    public void SetNumberAsInf(bool isInfinite)
    {
        infinite = isInfinite;
    }

    public float GetMagnitude() => (float)Math.Sqrt(real * real + imaginary * imaginary);

    public override string ToString()
    {
        return real.ToString() + " + " + imaginary.ToString() + "i";
    }

    public void Refresh()
    {
        real = 0f;
        imaginary = 0f;
    }

    public static ComplexFloat operator *(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real * b.real - a.imaginary * b.imaginary, a.real * b.imaginary + a.imaginary * b.real, a.infinite || b.infinite);

    public static ComplexFloat operator /(ComplexFloat a, ComplexFloat b)
    {
        float denominator = b.real * b.real + b.imaginary * b.imaginary;
        float inv = 1f / denominator;
        if(a.GetMagnitude() == 0 && b.GetMagnitude() == 0)
        {
            return new ComplexFloat(1,0);
        }
        return new ComplexFloat((a.real * b.real + a.imaginary * b.imaginary) * inv, (a.imaginary * b.real - a.real * b.imaginary) * inv);
    }

    public static ComplexFloat operator /(ComplexFloat a, int b)
    {
        return new ComplexFloat(a.real / b, a.imaginary / b);
    }

    public static ComplexFloat operator +(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real + b.real, a.imaginary + b.imaginary, a.infinite || b.infinite);

    public static ComplexFloat operator -(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real - b.real, a.imaginary  - b.imaginary, a.infinite || b.infinite);

    public ComplexFloat Inverse()
    {
        if(real == 0 && imaginary == 0) return this;
        float denominator = real * real + imaginary * imaginary;
        float inv = 1f / denominator;
        return new ComplexFloat(real * inv, -imaginary * inv);
    }

    public override bool Equals(object obj)
    {
        if (obj is not ComplexFloat other)
            return false;
        other = (ComplexFloat)obj;
        var t = Math.Abs(real - other.real);
        return
            Math.Abs(real - other.real) < Epsilon &&
            Math.Abs(imaginary - other.imaginary) < Epsilon;
    }

    public override int GetHashCode()
    {
        int r = (int)(real / Epsilon);
        int i = (int)(imaginary / Epsilon);

        return HashCode.Combine(r, i);
    }

    public static bool operator ==(ComplexFloat a, ComplexFloat b)
        => a.Equals(b);

    public static bool operator !=(ComplexFloat a, ComplexFloat b)
        => !a.Equals(b);

    public static bool operator ==(ComplexFloat a, float b)
        => a.GetMagnitude() == b;

    public static bool operator !=(ComplexFloat a, float b)
        => a.GetMagnitude() != b;
}
