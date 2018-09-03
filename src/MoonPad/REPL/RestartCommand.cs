namespace MoonPad.REPL
{
    // ReSharper disable once UnusedMember.Global
    internal class RestartCommand : ICommand
    {
        public string Name => "restart";

        public string GetShortHelp()
        {
            return "restart - creates a new Lua script context";
        }

        public string GetLongHelp()
        {
            return GetShortHelp();
        }

        public string Execute(ScriptContext context, string arg)
        {
            context.LuaRepl.Restart();
            return "Restarted";
        }
    }
}
