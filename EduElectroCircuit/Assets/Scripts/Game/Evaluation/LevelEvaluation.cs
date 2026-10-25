using System;
public static class LevelEvaluation
{
    public static EvaluationGrade GetRoomGrade(int NOA, int NOR, int minimum) => (2 * (NOA - minimum) + NOR) switch
    {
        0 => EvaluationGrade.S,
        <= 3 => EvaluationGrade.A,
        <= 6 => EvaluationGrade.B,
        <= 9 => EvaluationGrade.C,
        <= 12 => EvaluationGrade.D,
        <= 15 => EvaluationGrade.E,
        _ => EvaluationGrade.F
    };

    public static EvaluationGrade GetGradeAverage(EvaluationGrade[] gradesList, EvaluationGrade? timeGrade = null)
    {
        int total = 0;
        if(timeGrade != null) total = (int)timeGrade;
        foreach(EvaluationGrade value in gradesList)
        {
            total += (int)value;
        }
        float sum = total / (gradesList.Length + (timeGrade == null? 0f:1f));
        if(sum == 1) return EvaluationGrade.S;
        else if(sum <= 1.5) return EvaluationGrade.A;

        int grade = (int)Math.Round(sum, MidpointRounding.AwayFromZero);
        EvaluationGrade evaluationGrade = (EvaluationGrade)grade;
        return evaluationGrade == EvaluationGrade.N_A? EvaluationGrade.F: evaluationGrade;
    }

    public static bool GetGradeImproved(EvaluationGrade gradeOld, EvaluationGrade gradeNew) 
        => (int)gradeOld > (int)gradeNew;
}