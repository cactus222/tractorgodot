public static class ViewSettings
{
    private static float bidDelay = 2.0f;
    private static float trickDelay = 3.0f;

    public static void SetBidDelay(float f) {
        if (f > 0) {
            bidDelay = f;
        }
    }
    public static void SetTrickDelay(float f) {
        if (f > 0) {
            trickDelay = f;
        }
    }

    public static float GetTrickDelay() {
        return trickDelay;
    }

    public static float GetBidDelay() {
        return bidDelay;
    }
}