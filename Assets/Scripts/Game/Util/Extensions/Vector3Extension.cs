using UnityEngine;

namespace Game.Util.Extensions
{
    public static class Vector3Extension
    {
        public static (float x, float y, float z) ToTuple(this Vector3 vector3)
        {
            return (vector3.x, vector3.y, vector3.y);
        }

        public static Vector3 ToVector3(this (float x, float y, float z) vector3Tuple)
        {
            return new Vector3(vector3Tuple.x, vector3Tuple.y, vector3Tuple.z);
        }
        
        public static Vector2 ToVector2FromVector3(this (float x, float y, float z) vector2Tuple)
        {
            return new Vector2(vector2Tuple.x, vector2Tuple.y);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
    }
}
