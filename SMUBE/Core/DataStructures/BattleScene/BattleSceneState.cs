using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneState
    {
        public BattleSceneType BattleSceneType { get; }
        public Dictionary<BattleScenePosition, BattleScenePositionContent> occupiedPositions = new Dictionary<BattleScenePosition, BattleScenePositionContent>();

        public BattleSceneState(BattleSceneType argBattleSceneType) 
        { 
            BattleSceneType = argBattleSceneType; 
        }

        public bool TryFindUnitPosition(UnitIdentifier unitIdentifier, out BattleScenePosition battleScenePosition)
        {
            battleScenePosition = null;
            foreach (var pos in occupiedPositions)
            {
                if(pos.Value.UnitIdentifier == unitIdentifier)
                {
                    battleScenePosition = pos.Key;
                    return true;
                }
            }
            return false;
        }
    }
}
