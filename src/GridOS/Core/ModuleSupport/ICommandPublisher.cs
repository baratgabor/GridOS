using System.Collections.Generic;

namespace IngameScript
{
    /// <summary>
    /// Specidies a GridOS module class to be a publisher of executable command line commands (received as a string parameter from the game).
    /// </summary>
    interface ICommandPublisher
    {
        /// <summary>
        /// The list of commands to register in GridOS.
        /// After registration, commands can be executed by passing their command name to the Programmable Block.
        /// </summary>
        IEnumerable<CommandItem> Commands { get; }
    }
}
