using UnityEngine;

public static class Utils
{
    //easing helper function
    public static float EaseOut(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
    }
    
    // clamp angle helper function
    public static float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360+from);
        return Mathf.Min(angle, to);
    }
}
