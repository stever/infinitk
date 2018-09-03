using MoonSharp.Interpreter;

namespace MoonPad.REPL
{
    internal class ScriptContext
    {
        public Script Script { get; }

        public ScriptContext(Script script)
        {
            Script = script;
        }
    }
}
