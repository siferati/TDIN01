namespace Server
{
    /// <summary>
    /// Represents a diginote.
    /// </summary>
    class Diginote
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// The initial quote of diginotes.
        /// </summary>
        private const double INITIAL_QUOTE = 1.0;

        /// <summary>
        /// The current quote of diginotes.
        /// </summary>
        public static double Quote { get; set; } = INITIAL_QUOTE;


        /* --- METHODS --- */

        public Diginote()
        {

        }
    }
}
