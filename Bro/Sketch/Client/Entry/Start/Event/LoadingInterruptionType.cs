namespace Bro.Sketch.Client
{
    public enum LoadingInterruptionType
    {
        Undefined = 0,
        NeedNewVersion,
        ConnectionProblems,
        EnableNetwork,
        WaitForAuthQueue,
        Maintenance,
        SystemValidationFailed,
    }
}