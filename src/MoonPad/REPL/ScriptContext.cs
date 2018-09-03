namespace MoonPad.REPL
{
    internal class ScriptContext
    {
        public LuaRepl LuaRepl { get; }

        public ScriptContext(LuaRepl luaRepl)
        {
            LuaRepl = luaRepl;
        }
    }
}
