namespace MoonPad.REPL
{
    internal interface ICommand
    {
        string Name { get; }
        string GetShortHelp();
        string GetLongHelp();
        string Execute(ScriptContext context, string arg);
    }
}
