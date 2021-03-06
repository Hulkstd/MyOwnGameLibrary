﻿using System;
using System.Collections.Generic;
using Game.Serialization;
using UnityEngine;

namespace ExampleScript
{
    public class CSVTest : MonoBehaviour
    {
        [Serializable]
        public class TestData : CSVReader.CSVData
        {
            public int _id;
            public int _number;

            public override string SetData(string[] properties)
            {
                base.SetData(properties);

                _id = int.Parse(properties[0]);
                _number = int.Parse(properties[1]);

                return properties[0];
            }

            public override string ToString()
            {
                return $" ID:{_id}   Number:{_number}";
            }
        }

        private void Start()
        {
            var dict = new Dictionary<string, TestData>();
            
            CSVReader<TestData>.ReadCSV(dict, "0,1\n1,2\n3,2\n4,3\n5,4\n");

            foreach (var keyValuePair in dict)
            {
                Debug.Log($"Key[{keyValuePair.Key}] Value: [{keyValuePair.Value}]");
            }
        }
    }
}
