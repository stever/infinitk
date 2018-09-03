using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace MoonPad.REPL
{
    internal class LuaReplScriptLoader : ScriptLoaderBase
    {
        private readonly FormWindow formWindow;

        private Database Database => formWindow.Database;

        public LuaReplScriptLoader(FormWindow formWindow)
        {
            this.formWindow = formWindow;

            // The following line is require to match resources to load.
            ModulePaths = new [] {"?"};
        }

        public override bool ScriptFileExists(string name)
        {
            if (Database == null) return false;
            var names = Database.GetLuaScriptNames();
            return new HashSet<string>(names).Contains(name);
        }

        public override object LoadFile(string name, Table globalContext)
        {
            var script = Database?.GetLuaScript(name);

            if (script == null)
            {
                throw new ScriptRuntimeException($"file '{name}' not found");
            }

            return script;
        }
    }
}
