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
    }
}
