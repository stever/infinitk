namespace MoonPad.REPL
{
    // ReSharper disable once UnusedMember.Global
    internal class DebugCommand : ICommand
    {
        public string Name => "debug";

        public string GetShortHelp()
        {
            return "debug - starts the interactive debugger server";
        }

        public string GetLongHelp()
        {
            return GetShortHelp() + " (note that this currently requires Visual Studio Code to connect)";
        }

        public string Execute(ScriptContext context, string arg)
        {
            context.LuaRepl.StartDebugger();
            return "[debugger started]";
        }
    }
}
