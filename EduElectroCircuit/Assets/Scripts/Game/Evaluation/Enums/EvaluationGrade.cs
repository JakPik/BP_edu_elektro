public enum EvaluationGrade
{
    S = 1,
    A = 2,
    B = 3,
    C = 4,
    D = 5,
    E = 6,
    F = 7,
    N_A = 8
}

public static class EvaluationGradeExtensions
{
    public static string ToDisplayString(this EvaluationGrade grade)
    {
        return grade switch
        {
            EvaluationGrade.S => "S",
            EvaluationGrade.A => "A",
            EvaluationGrade.B => "B",
            EvaluationGrade.C => "C",
            EvaluationGrade.D => "D",
            EvaluationGrade.E => "E",
            EvaluationGrade.F => "F",
            EvaluationGrade.N_A => "N/A",
            _ => ""
        };
    }
}