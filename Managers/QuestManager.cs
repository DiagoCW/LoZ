using System;

namespace LoZ.Managers
{
    internal static class QuestManager
    {
        public static bool LostCowFound { get; private set; } = false;
        public static bool GameWon { get; private set; } = false;

        public static void FoundLostCow() 
        {
            LostCowFound = true;
        }

        public static void WinGame() 
        {
            GameWon = true;
        }

        public static void Reset() 
        {
            LostCowFound = false;
            GameWon = false;
        }
    }
}
