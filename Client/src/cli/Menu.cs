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
        protected String[] options;

        /// <summary>
        /// Title of this menu state.
        /// </summary>
        protected String title;


        /// <summary>
        /// Client object.
        /// </summary>
        protected Client client;


        /* --- METHODS --- */

        /// <summary>
        /// Super constructor called by subclasses.
        /// </summary>
        /// <param name="options">List of options of menu state</param>
        public Menu(Client client, String title, String[] options)
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
        public abstract Menu ProcessInput(String input);


        public override String ToString()
        {
            String str = "----------";
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
