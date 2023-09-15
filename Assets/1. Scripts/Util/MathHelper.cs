using UnityEngine;

public class MathHelper
{
    public static bool FloatZeroChecker(float val)
    {
        return (-Mathf.Epsilon < val) && (val < Mathf.Epsilon);
    }
}
