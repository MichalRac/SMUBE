using System;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.Utils
{
    internal static class GenericChoiceUtils
    {
        private static void InvalidMessage()
        {
            Console.Clear();
            Console.WriteLine("Invalid input! Try again!\n\n");
        }

        public static bool GetBooleanChoice(string question)
        {
            Console.Clear();

            Console.WriteLine(question);
            Console.WriteLine("Y/N");
            var key = Console.ReadKey(true);

            Console.Clear();
            switch (key.Key)
            {
                case ConsoleKey.Y:
                    return true;
                case ConsoleKey.N: 
                    return false;
                default:
                    InvalidMessage();
                    return GetBooleanChoice(question);
            }
        }

        public static int GetInt(string question)
        {
            Console.WriteLine(question);
            var read = Console.ReadLine();
            if(!int.TryParse(read, out var number))
            {
                Console.WriteLine("\nNon int value provided! Try again!");
                return GetInt(question);
            }
            return number;

        }

        public static T GetListChoice<T>(string question, bool allowExit, List<(string description, T result)> choiceList, bool cleanConsole = true)
        {
            if (cleanConsole)
            {
                Console.Clear();
            }

            Console.WriteLine(question);
            for (int i = 0; i < choiceList.Count; i++)
            {
                Console.WriteLine($"{i+1}. {choiceList[i].description}");
            }
            if (allowExit)
            {
                Console.WriteLine($"0. Exit");
            }

            Console.WriteLine("\nChoice:");

            var instantChoice = choiceList.Count <= 9;
            var input = instantChoice 
                ? Console.ReadKey(true).KeyChar.ToString()
                : Console.ReadLine();

            if (int.TryParse(input.ToString(), out var choice))
            {
                if(choice > 0 && choice <= choiceList.Count)
                {
                    return choiceList[choice - 1].result;
                }
                if(choice == 0 && allowExit)
                {
                    return default;
                }
            }
            InvalidMessage();
            return GetListChoice(question, allowExit, choiceList);
        }
        
        public static T GetListChoiceKeyed<T>(string question, bool allowExit, Dictionary<ConsoleKey, (string description, T result)> choiceList, bool cleanConsole = true)
        {
            if (cleanConsole)
            {
                Console.Clear();
            }

            Console.WriteLine(question);
            foreach (var choiceItemToDescribe in choiceList)
            {
                Console.WriteLine($"{choiceItemToDescribe.Key.ToString()}. {choiceItemToDescribe.Value.description}");
            }
            if (allowExit)
            {
                Console.WriteLine($"0. Exit");
            }

            var input = Console.ReadKey(true).Key;

            if (choiceList.TryGetValue(input, out var choiceItemToInvoke))
            {
                return choiceItemToInvoke.result;
            }
            if(allowExit && input == ConsoleKey.D0)
            {
                return default;
            }

            InvalidMessage();
            return GetListChoiceKeyed(question, allowExit, choiceList);
        }

    }
}
