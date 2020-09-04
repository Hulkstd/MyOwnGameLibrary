using System;
using Game.Tweener.TweenData;
using UnityEditor;
using UnityEngine;

namespace Game.Tweener.Module.Editor
{
    [CustomPropertyDrawer(typeof(LoopProperty))]
    public class LoopPropertyDrawer: PropertyDrawer
    {
        private SerializedProperty _loop;
        private string _loopName;
        private SerializedProperty _loopType;
        private string _loopTypeName;
        private bool _cache = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_cache)
            {
                property.Next(true);
                _loop = property.Copy();
                _loopName = property.name;
                if (_loopName[0] == '_' || char.IsLower(_loopName[0]))
                {
                    _loopName = _loopName.Replace("_", "");
                    _loopName = char.ToUpper(_loopName[0]) + _loopName.Substring(1);
                }
                property.Next(true);
                _loopType = property.Copy();
                _loopTypeName = property.name;
                if (_loopTypeName[0] == '_' || char.IsLower(_loopTypeName[0]))
                {
                    _loopTypeName = _loopTypeName.Replace("_", "");
                    _loopTypeName = char.ToUpper(_loopTypeName[0]) + _loopTypeName.Substring(1);
                }

                _cache = true;
            }

            EditorGUI.BeginProperty(position, label, property);

            var loopTypePosition = position;
            loopTypePosition.y += 20;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),
                new GUIContent(_loopName));
            if (_loop.boolValue)
                position.y -= 10;
            EditorGUI.PropertyField(position, _loop, GUIContent.none);

            if (_loop.boolValue)
            {
                loopTypePosition = EditorGUI.PrefixLabel(loopTypePosition, GUIUtility.GetControlID(FocusType.Passive),
                    new GUIContent(_loopTypeName));
                EditorGUI.PropertyField(loopTypePosition, _loopType, GUIContent.none);
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_loop == null)
                return 16f;
            
            return 16f + (_loop.boolValue ? 20f : 0);
        }
    }
}