using System;
using System.Reflection;
using log4net;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;

namespace MoonPad
{
    internal class LuaRepl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FormWindow formWindow;
        private readonly ReplInterpreter interpreter;

        public LuaRepl(FormWindow formWindow)
        {
            this.formWindow = formWindow;

            Script.DefaultOptions.ScriptLoader = new ReplInterpreterScriptLoader();
            var script = new Script(CoreModules.Preset_HardSandbox);

            interpreter = new ReplInterpreter(script)
            {
                HandleDynamicExprs = true,
                HandleClassicExprsSyntax = true
            };
        }

        public string HandleInput(string s)
        {
            Log.DebugFormat("Lua REPL line: {0}", s);
            Log.DebugFormat("HasPendingCommand: {0}", interpreter.HasPendingCommand);
            try
            {
                var result = interpreter.Evaluate(s);

                if (result == null)
                {
                    Log.Debug("Lua REPL command is incomplete (null returned)");
                    Log.DebugFormat("HasPendingCommand: {0}", interpreter.HasPendingCommand);
                }

                if (result != null && result.Type != DataType.Void)
                {
                    return result.ToString();
                }
            }
            catch (InterpreterException ex)
            {
                return $"{ex.DecoratedMessage ?? ex.Message}";
            }
            //catch (SyntaxErrorException ex)
            //{
            //    return $"Syntax Error: {ex.Message}";
            //}
            //catch (ScriptRuntimeException ex)
            //{
            //    return $"Script Error: {ex.Message}";
            //}
            catch (Exception ex)
            {
                return $"{ex.GetType().Name}: {ex.Message}";
            }

            return "";
        }
    }
}
