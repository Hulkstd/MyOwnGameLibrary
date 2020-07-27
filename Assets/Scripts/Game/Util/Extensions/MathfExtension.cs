using UnityEngine;

namespace Game.Util.Extensions
{
    public static class MathfExtension
    {
        public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (value - inMin) * (inMax - inMin) / (outMax - outMin);
        }

        public static int MapRoundFloat(float value, float inMin, float inMax, float outMin,
            float outMax)
        {
            return Mathf.RoundToInt(Map(value, inMin, inMax, outMin, outMax));
        }
        
        public static int MapFloorFloat(float value, float inMin, float inMax, float outMin,
            float outMax)
        {
            return Mathf.FloorToInt(Map(value, inMin, inMax, outMin, outMax));
        }
        
        public static int MapCeilFloat(float value, float inMin, float inMax, float outMin,
            float outMax)
        {
            return Mathf.CeilToInt(Map(value, inMin, inMax, outMin, outMax));
        }
    }
}
