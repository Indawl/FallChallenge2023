using System;

namespace FallChallenge2023
{
    class Player
    {
#if DEBUG_MANAGER
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Bots.Bronze.Debug.GameManager.Generate();
#else
        static void Main(string[] args)
        {
#endif

            var bot = new Bots.Bronze.Bot();

            bot.ReadInitialize();

            while (true)
                Console.WriteLine(bot.GetAction(bot.ReadState()).ToString());
        }
    }
}
