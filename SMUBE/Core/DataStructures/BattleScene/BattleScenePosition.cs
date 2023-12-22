using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.DataStructures.BattleScene
{
    public enum BattleScenePositionContentType
    {
        None,
        Obstacle,
        Defensive,
        Unstable,
    }

    public class BattleScenePosition
    {
        public SMUBEVector2<int> Coordinates { get; }

        public BattleScenePosition(int x, int y)
        {
            Coordinates = new SMUBEVector2<int>(x, y);
        }

        public BattleScenePosition(SMUBEVector2<int> pos)
        {
            Coordinates = pos;
        }

        public UnitIdentifier UnitIdentifier { get; private set; }
        public BattleScenePositionContentType ContentType { get; private set; }

        public bool IsEmpty() 
        {
            return UnitIdentifier == null && (ContentType != BattleScenePositionContentType.Obstacle);
        }

        public void Clear()
        {
            UnitIdentifier = null;
        }

        public void ApplyUnit(UnitIdentifier unitIdentifier)
        {
            Clear();
            UnitIdentifier = unitIdentifier;
        }

        public void ApplyObstacle()
        {
            Clear();
            ContentType = BattleScenePositionContentType.Obstacle;
        }

        public void ApplyDefensive()
        {
            Clear();
            ContentType = BattleScenePositionContentType.Defensive;
        }
        public void ApplyUnstable()
        {
            Clear();
            ContentType = BattleScenePositionContentType.Unstable;
        }

        public override string ToString()
        {
            return $"({Coordinates.x},{Coordinates.y})";
        }
    }
}
