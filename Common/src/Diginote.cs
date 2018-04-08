namespace Common
{
    /// <summary>
    /// Represents a diginote.
    /// </summary>
    public class Diginote
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// The serial number.
        /// </summary>
        public long Id { get; }


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instace of Diginote.
        /// </summary>
        /// <param name="id">Serial number.</param>
        public Diginote(long id)
        {
            this.Id = id;
        }
    }
}
