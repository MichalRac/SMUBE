using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public static class GOPConfig
    {
        public static List<Goal> GetGoalsForArchetype(BaseCharacter character, bool useSimpleBehavior)
        {
            switch (character)
            {
                case Hunter _:
                    return useSimpleBehavior ? GetSimpleHunterGoals() : GetHunterGoals();
                case Scholar _:
                    return useSimpleBehavior ? GetSimpleScholarGoals() : GetScholarGoals();
                case Squire _:
                    return useSimpleBehavior ? GetSimpleSquireGoals() : GetSquireGoals();
                default:
                    return null;
            }
        }

        public static List<Goal> GetHunterGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new WinGoal(),
                new WinGoal(),
                new LowerEnemyHealthGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetScholarGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new WinGoal(),
                new WinGoal(),
                new LowerEnemyHealthGoal(),
                new KeepTeamHealthUpGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetSquireGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new WinGoal(),
                new WinGoal(),
                new LowerEnemyHealthGoal(),
                new KeepTeamGuardedGoal(),
                new SurviveGoal(),
            };
        }

        public static List<Goal> GetSimpleHunterGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetSimpleScholarGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetSimpleSquireGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }

    }
}
