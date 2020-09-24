using System;
using Game.Tweener.Core;
using Game.Util.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleScript
{
    public class SequenceTest : MonoBehaviour
    {
        public Image _image;
        private Sequence _sequence; 
        
        private void Start()
        {
            _sequence = Sequence.GetSequence();

            _sequence
                .Append(_image.DoAlphaFade(0, 5.0f))
                .Join(_image.rectTransform.DoAnchorMove(Vector3.left * 250, 2))
                .Append(_image.rectTransform.DoRotate(Quaternion.Euler(0, 0, 180), 2))
                .PrependInterval(2)
                .InsertCallback(6, Callback);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                _sequence.Play();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _sequence.Stop();
            }
        }

        private void Callback()
        {
            Debug.Log("callback");
        }
    }
}