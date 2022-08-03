
namespace Litkey.Utility
{
    public static class ProbabilityCheck
    {
        /*
         * @param Chance: float number between 0 and 1
         * Return: returns the success based on the given chance number between 0 and 1
         */
        public static bool GetThisChanceResult(float Chance)
        {
            if (Chance < 0.0000001f)
            {
                Chance = 0.0000001f;
            }

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }

        /*
         * @param Percentage_Chance: number between 0 and 100.
         * Return: returns the success based on the given number percentage
         */
        public static bool GetThisChanceResult_Percentage(float Percentage_Chance)
        {
            if (Percentage_Chance < 0.0000001f)
            {
                Percentage_Chance = 0.0000001f;
            }

            Percentage_Chance = Percentage_Chance / 100;

            bool Success = false;
            int RandAccuracy = 10000000;
            float RandHitRange = Percentage_Chance * RandAccuracy;
            int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
            if (Rand <= RandHitRange)
            {
                Success = true;
            }
            return Success;
        }
    }
}
