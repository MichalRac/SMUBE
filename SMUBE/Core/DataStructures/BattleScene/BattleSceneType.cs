using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public abstract class BattleSceneType
    {
        public int SceneSize { get; }
        public BattleSceneType(int argSceneSize) 
        { 
            SceneSize = argSceneSize; 
        }
        public abstract bool IsValidPosition(BattleScenePosition targetPos);
        public abstract List<BattleScenePosition> GetAdjacentPositions(BattleScenePosition targetPos);
        public abstract bool IsOccupied(BattleScenePosition targetPos, BattleSceneState battleSceneState);
        public abstract bool IsReachable(BattleScenePosition basePos, BattleScenePosition targetPos, BattleSceneState battleSceneState);

    }
}
