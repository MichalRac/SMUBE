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
        public static List<Goal> GetGoalsForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                    return GetHunterGoals();
                case Scholar _:
                    return GetScholarGoals();
                case Squire _:
                    return GetSquireGoals();
                default:
                    return null;
            }
        }

        public static List<Goal> GetHunterGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetScholarGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }
        public static List<Goal> GetSquireGoals()
        {
            return new List<Goal>()
            {
                new WinGoal(),
                new SurviveGoal(),
            };
        }

    }
}
