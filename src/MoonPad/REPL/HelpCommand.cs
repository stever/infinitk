using System.Text;

namespace MoonPad.REPL
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class HelpCommand : ICommand
    {
        public string Name => "help";

        public string GetShortHelp()
        {
            return "help - gets the list of possible commands, or help about the specified command";
        }

        public string GetLongHelp()
        {
            return GetShortHelp();
        }

        public string Execute(ScriptContext context, string arg)
        {
            if (!string.IsNullOrWhiteSpace(arg))
            {
                var cmd = CommandManager.Find(arg);
                return cmd != null
                    ? cmd.GetLongHelp()
                    : $"Command '{arg}' not found.";
            }

            var sb = new StringBuilder();
            sb.Append("Type Lua code to execute Lua code (split lines are accepted)\n");
            sb.Append("or type one of the following commands to execute them.\n");
            sb.Append("\n");
            sb.Append("Commands:\n");
            sb.Append("\n");

            foreach (var cmd in CommandManager.GetCommands())
            {
                sb.Append("  !");
                sb.Append(cmd.GetShortHelp());
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
