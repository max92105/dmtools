namespace Data.Constants
{
    /// <summary>
    /// Default dice pools for damage resistance/vulnerability based on Challenge Rating.
    /// Used when importing legacy monsters that just have "resistance" or "vulnerability".
    /// </summary>
    public sealed class DamageModifierDefaults
    {
        /// <summary>
        /// Returns the default dice notation (count, size) for a given CR.
        /// </summary>
        public static void GetDefaultDice(decimal challengeRating, out short diceCount, out short diceSize)
        {
            diceSize = 6; // always d6

            if (challengeRating <= 1)
                diceCount = 1;
            else if (challengeRating <= 4)
                diceCount = 1;
            else if (challengeRating <= 8)
                diceCount = 2;
            else if (challengeRating <= 12)
                diceCount = 3;
            else if (challengeRating <= 16)
                diceCount = 4;
            else if (challengeRating <= 20)
                diceCount = 5;
            else
                diceCount = 6;
        }
    }
}
