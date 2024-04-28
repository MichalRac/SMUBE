using SMUBE.Commands;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    internal class DecisionTreeActionSimple : DecisionTreeAction
    {
        private readonly BaseCommand _result;

        public DecisionTreeActionSimple(BaseCommand result)
        {
            _result = result;
        }
        public override BaseCommand GetCommand()
        {
            return _result;
        }
    }
    
    internal class DecisionTreeActionSimple<T> : DecisionTreeAction 
        where T : BaseCommand, new()    
    {
        public override BaseCommand GetCommand()
        {
            return new T();
        }
    }

}
