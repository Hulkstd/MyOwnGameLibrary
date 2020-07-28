using System;
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
        public Utility.Curves.Ease ease;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                image1.DoFade(endValue, 5.0f).SetEase(ease).Play();
                Debug.Log(endValue);
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                var endValue = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                image2.DoFade(endValue, 5.0f).SetEase(ease).SetLoop(true).Play();
                Debug.Log(endValue);
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                image1.color = Color.white;
            }
        }
    }
}