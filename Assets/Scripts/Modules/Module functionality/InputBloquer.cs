public static class InputBlocker
{
    public static bool IsBlocked { get; private set; }

    public static void SetBlocked(bool value)
    {
        IsBlocked = value;
    }
}
