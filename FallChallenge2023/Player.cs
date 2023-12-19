using System;

namespace FallChallenge2023
{
    class Player
    {
        static void Main(string[] args)
        {
            var bot = new Bots.Wood.Bot();

            bot.ReadInitialize();

            while (true)
                Console.WriteLine(bot.GetAction(bot.ReadState()).ToString());
        }
    }
}
