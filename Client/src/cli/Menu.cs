using System;

namespace Client.Cli
{
    /// <summary>
    /// A generic menu state.
    /// </summary>
    abstract class Menu
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// The list of options of this menu state.
        /// </summary>
        protected string[] options;

        /// <summary>
        /// Title of this menu state.
        /// </summary>
        protected string title;


        /// <summary>
        /// Client object.
        /// </summary>
        protected Client client;


        /* --- METHODS --- */

        /// <summary>
        /// Super constructor called by subclasses.
        /// </summary>
        /// <param name="options">List of options of menu state</param>
        public Menu(Client client, string title, string[] options)
        {
            this.client = client;
            this.title = title;
            this.options = options;
        }


        /// <summary>
        /// Process user input and return new menu state.
        /// </summary>
        /// <param name="input">User input.</param>
        /// <returns>New menu state.</returns>
        public abstract Menu ProcessInput(string input);


        public override string ToString()
        {
            String str = "----------";
            
            if (client.User != null)
            {
                str += "\nName: " + client.User.Name + "\n";
                str += "Wallet: " + client.User.Wallet.Count + " Diginotes\n";
                str += "Quote: " + client.GetQuote() + "\n";
                str += "----------";
            }

            str += "\n" + title + "\n\n";
            for (int i = 0; i < options.Length; i++)
            {
                str += (i + 1) + ". " + options[i] + "\n";
            }
            str += "\nChoice: ";
            return str;
        }
    }
}
