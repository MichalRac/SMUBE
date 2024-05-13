using System;
using System.Collections.Generic;

namespace SMUBE.AI.DecisionTree
{
    [Serializable]
    public class DecisionTreeDataSet
    {
        public Dictionary<string, float> Probabilities = new Dictionary<string, float>();
        public Dictionary<string, int> Weights = new Dictionary<string, int>();
    }
}