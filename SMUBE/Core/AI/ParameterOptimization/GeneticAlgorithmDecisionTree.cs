using System;
using System.Collections.Generic;
using SMUBE.AI.DecisionTree;

namespace SMUBE.AI.ParameterOptimization
{
    public class GeneticAlgorithmDecisionTree
    {
        private const int MAX_START_WEIGHT = 0;
        private const int MIN_START_WEIGHT = 100;

        private const int SIMULATIONS_PER_FITNESS_TEST = 10_000;
        
        private readonly int _generationSize;
        private readonly int _generationsToRun;
        
        private List<DecisionTreeDataSet> _solutions;
        private Random _rng;
        
        public GeneticAlgorithmDecisionTree(int generationSize, int generationsToRun)
        {
            _generationSize = generationSize;
            _generationsToRun = generationsToRun;
            _rng = new Random();
        }

        public void Run()
        {
        }
        


        /*
        public async Task<int> GetFitnessValue()
        {
        }
        */
        
        
    }
}