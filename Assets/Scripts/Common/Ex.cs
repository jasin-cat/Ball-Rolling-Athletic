using UnityEngine;

public static class Ex
{
    public static Vector3 Multiply
    (
        Vector3 target,
        Vector3 multVector
    )
    {
        var x = target.x * multVector.x;
        var y = target.y * multVector.y;
        var z = target.z * multVector.z;

        return new Vector3(x, y, z);
    }
}