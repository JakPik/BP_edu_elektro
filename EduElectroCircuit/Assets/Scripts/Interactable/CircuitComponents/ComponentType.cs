public enum ComponentType
{
    RESISTOR,
    CAPACITOR,
    INDUCTOR
}

public enum MeterType
{
    OHMMETER,
    VOLTMETER,
    AMPMETER

    
}

public static class EnumTransformer
{
    public static string MeterTypeToString(this MeterType meterType)
    {
        return meterType switch
        {
            MeterType.OHMMETER => "Ohmmeter",
            MeterType.VOLTMETER => "Voltmeter",
            MeterType.AMPMETER => "Ampermeter",
            _ => ""
        };
    }
}
