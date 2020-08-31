using UnityEngine;

namespace Game.Util.Extensions
{
    public static class Vector2Extension
    {
        public static (float x, float y) ToTuple(this Vector2 vector2)
        {
            return (vector2.x, vector2.y);
        }
        
        public static Vector3 ToVector3FromVector2(this (float x, float y) vector2Tuple)
        {
            return new Vector3(vector2Tuple.x, vector2Tuple.y);
        }
        
        public static Vector2 ToVector2(this (float x, float y) vector2Tuple)
        {
            return new Vector2(vector2Tuple.x, vector2Tuple.y);
        }

        public static Vector3 ToVector3(this Vector2 vector2, float z = 0)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }
    }
}
