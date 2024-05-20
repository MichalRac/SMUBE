using System;
using System.Collections.Generic;
using SMUBE.Core;

namespace SMUBE.AI.DecisionTree
{
    [Serializable]
    public class DecisionTreeDataSet
    {
        public Dictionary<string, float> Probabilities = new Dictionary<string, float>();
        public Dictionary<string, DecisionTreeDataSetWeight> Weights = new Dictionary<string, DecisionTreeDataSetWeight>();

        public DecisionTreeDataSet DeepCopy()
        {
            var copy = new DecisionTreeDataSet();
            foreach (var probability in Probabilities)
            {
                copy.Probabilities[probability.Key] = probability.Value;
            }
            foreach (var weight in Weights)
            {
                copy.Weights[weight.Key] = new DecisionTreeDataSetWeight(weight.Value.CurrentWeightOption);
            }
            return copy;
        }
    }
    
    [Serializable]
    public enum WeightOption
    {
        Disabled = 0,
            
        Marginal5 = 1,
        ExtremelyLow10 = 2,
        VeryLow25 = 3,
        Low50 = 4,
        Medium100 = 5,
        High150 = 6,
        VeryHigh200 = 7,
        ExtremelyHigh400 = 8,
    }

    [Serializable]
    public class DecisionTreeDataSetWeight
    {

        public WeightOption CurrentWeightOption;
        public int GetValue() => WeightOptionToValue();

        public static DecisionTreeDataSetWeight Random()
        {
            var weight = Disabled();
            weight.RandomizeAssignedWeight();
            return weight;
        }
        public static DecisionTreeDataSetWeight Disabled() => new DecisionTreeDataSetWeight(WeightOption.Disabled);
        public static DecisionTreeDataSetWeight W5() => new DecisionTreeDataSetWeight(WeightOption.Marginal5);
        public static DecisionTreeDataSetWeight W10() => new DecisionTreeDataSetWeight(WeightOption.ExtremelyLow10);
        public static DecisionTreeDataSetWeight W25() => new DecisionTreeDataSetWeight(WeightOption.VeryLow25);
        public static DecisionTreeDataSetWeight W50() => new DecisionTreeDataSetWeight(WeightOption.Low50);
        public static DecisionTreeDataSetWeight W100() => new DecisionTreeDataSetWeight(WeightOption.Medium100);
        public static DecisionTreeDataSetWeight W150() => new DecisionTreeDataSetWeight(WeightOption.High150);
        public static DecisionTreeDataSetWeight W200() => new DecisionTreeDataSetWeight(WeightOption.VeryHigh200);
        public static DecisionTreeDataSetWeight W400() => new DecisionTreeDataSetWeight(WeightOption.ExtremelyHigh400);
        
        public DecisionTreeDataSetWeight(WeightOption currentWeightOption)
        {
            CurrentWeightOption = currentWeightOption;
        }

        public void RandomizeAssignedWeight()
        {
            CurrentWeightOption = (WeightOption)RngProvider.Next(0, 8);
        }
        
        private int WeightOptionToValue()
        {
            switch (CurrentWeightOption)
            {
                case WeightOption.Disabled: return 0;
                case WeightOption.Marginal5: return 5;
                case WeightOption.ExtremelyLow10: return 10;
                case WeightOption.VeryLow25: return 25;
                case WeightOption.Low50: return 50;
                case WeightOption.Medium100: return 100;
                case WeightOption.High150: return 150;
                case WeightOption.VeryHigh200: return 200;
                case WeightOption.ExtremelyHigh400: return 400;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}