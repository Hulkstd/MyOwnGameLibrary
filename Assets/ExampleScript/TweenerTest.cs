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
        public Image image1;
        public Image image2;
        public Image image3;
        public Utility.Curves.Ease ease;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                image1.DoFade(endValue, 5.0f).SetEase(ease).Play();
                Debug.Log(endValue);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                image2.DoFade(endValue, 5.0f).SetEase(ease).SetLoop(true).Play();
                Debug.Log(endValue);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                image3.DoFade(endValue, 5.0f).SetEase(ease).SetLoop(true, LoopType.PingPong).Play();
                Debug.Log(endValue);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                image1.color = Color.white;
            }
        }
    }
}