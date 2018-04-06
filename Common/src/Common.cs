namespace Common
{
    /// <summary>
    /// Exposes the server methods to the client.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if username and passsword match, FALSE otherwise.</returns>
        bool Login(string username, string password);

        /// <summary>
        /// Registers a new user into the system.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if new user was created, FALSE otherwise.</returns>
        bool Register(string name, string username, string password);
    }
}
