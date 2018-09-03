using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace MoonPad
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
            var names = Database.GetLuaScriptNames();
            return new HashSet<string>(names).Contains(name);
        }

        public override object LoadFile(string name, Table globalContext)
        {
            return Database.GetLuaScript(name);
        }
    }
}
