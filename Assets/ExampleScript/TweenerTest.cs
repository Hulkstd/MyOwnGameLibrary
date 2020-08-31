using System;
using Game.Tweener.Core;
using Game.Util;
using Game.Util.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ExampleScript
{
    public class TweenerTest : MonoBehaviour
    {
        public Image _image1;
        public Image _image2;
        public Image _image3;
        public Utility.Curves.Ease _ease;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                _image1.DoFade(endValue, 5.0f).SetEase(_ease).Play();
                Debug.Log(endValue);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                _image2.DoFade(endValue, 5.0f).SetEase(_ease).SetLoop(true).Play();
                Debug.Log(endValue);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                _image3.DoFade(endValue, 5.0f).SetEase(_ease).SetLoop(true, LoopType.PingPong).Play();
                Debug.Log(endValue);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _image1.color = Color.white;
            }
        }
    }
}