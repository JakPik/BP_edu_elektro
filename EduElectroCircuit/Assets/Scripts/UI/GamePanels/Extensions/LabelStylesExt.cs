public static class LabelStylesExt
{
    public static string ToClassString(LabelStyles labelStyle)
    {
        switch (labelStyle)
        {
            case LabelStyles.ERROR:
                return "text-error";
            case LabelStyles.SUCCESS:
                return "text-success";
            case LabelStyles.PARAMETER:
                return "text-parameter";
            case LabelStyles.INFO:
            default:
                return "text-info";
        }
    }
}