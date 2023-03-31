using SMUBE.Core;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE_Utils.Simulator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        private static BattleCore _core;

        private static void Init()
        {

            var initUnits = new List<Unit>()
            {
                UnitHelper.CreateUnit<Squire>(0),
                UnitHelper.CreateUnit<Squire>(1),

                UnitHelper.CreateUnit<Hunter>(0),
                UnitHelper.CreateUnit<Hunter>(1),

                UnitHelper.CreateUnit<Scholar>(0),
                UnitHelper.CreateUnit<Scholar>(1),
            };

            _core = new BattleCore(initUnits);

            TurnChoice();

            return;
        }

        private static void TurnChoice()
        {
            var autoPlay = false;

            Console.WriteLine("Options:");
            Console.WriteLine("1. Continue");
            Console.WriteLine("2. Auto-continue");
            Console.WriteLine("3. Show full unit summary");
            Console.WriteLine("4. Show short unit summary");
            Console.WriteLine("0. Close");

            Console.WriteLine("\nChoice:");
            var key = Console.ReadKey(true);
            Console.Write("\n");

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Not implemented!");
                    Console.Write("\n");
                    ResolveTurn();
                    TurnChoice();
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Not implemented!");
                    Console.Write("\n");
                    TurnChoice();
                    break;
                case ConsoleKey.D3:
                    GetUnitSummary(0, false);
                    TurnChoice();
                    break;
                case ConsoleKey.D4:
                    GetUnitSummary(0, true);
                    TurnChoice();
                    break;
                case ConsoleKey.D0:
                    return;
            }
        }

        private static void ResolveTurn()
        {

        }

        private static void GetUnitSummary(int turn, bool showShortSummmary)
        {
            Console.WriteLine(
                "\n---\n" +
                $"Turn {turn}:" +
                "\n");


            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Team {i}:");
                foreach (var unit in _core.currentStateModel.GetTeamUnits(i))
                {
                    var summary = showShortSummmary ? unit.UnitData.ToShortString() : unit.UnitData.ToString();
                    Console.WriteLine(summary);
                }
            }

            Console.WriteLine("Press anything to continue.");
            Console.ReadKey();

        }
    }
}