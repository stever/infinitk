using System;
using System.Reflection;
using System.Text;
using log4net;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;

namespace MoonPad.REPL
{
    internal class LuaRepl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FormWindow formWindow;
        private readonly ScriptContext context;

        private ReplInterpreter interpreter;

        public LuaRepl(FormWindow formWindow)
        {
            this.formWindow = formWindow;
            context = new ScriptContext(this);
            CommandManager.Initialize();
            Restart();
        }

        public void Restart()
        {
            // TODO: Identify the sandbox requirement for using the inspect module.
            //const CoreModules sandbox = CoreModules.Preset_HardSandbox | CoreModules.LoadMethods;
            const CoreModules sandbox = CoreModules.Preset_Complete;

            var script = new Script(sandbox) {Options =
            {
                // TODO: Capture and return print output.
                DebugPrint = s => Log.InfoFormat("Lua print: {0}", s),
                ScriptLoader = new LuaReplScriptLoader(formWindow)
            }};

            interpreter = new ReplInterpreter(script)
            {
                HandleDynamicExprs = false,
                HandleClassicExprsSyntax = true
            };
        }

        public string HandleInput(string s)
        {
            try
            {
                if (s.StartsWith("!"))
                {
                    return ExecuteCommand(context, s.Substring(1));
                }

                var result = interpreter.Evaluate(s);

                if (result == null)
                {
                    return null;
                }

                return result.Type != DataType.Void ? result.ToString() : "";
            }
            catch (SyntaxErrorException ex)
            {
                return ex.DecoratedMessage != null
                    ? $"Syntax Error. {ex.DecoratedMessage}"
                    : $"Syntax Error: {ex.Message}";
            }
            catch (ScriptRuntimeException ex)
            {
                return ex.DecoratedMessage != null
                    ? $"Script Error. {ex.DecoratedMessage}"
                    : $"Script Error: {ex.Message}";
            }
            catch (InterpreterException ex)
            {
                return $"{ex.DecoratedMessage ?? ex.Message}";
            }
            catch (Exception ex)
            {
                return $"{ex.GetType().Name}: {ex.Message}";
            }
        }

        private static string ExecuteCommand(ScriptContext context, string line)
        {
            var cmdBuilder = new StringBuilder();
            var argBuilder = new StringBuilder();

            var dest = cmdBuilder;
            foreach (var c in line)
            {
                if (dest == cmdBuilder && c == ' ')
                {
                    dest = argBuilder;
                    continue;
                }

                dest.Append(c);
            }

            var cmd = cmdBuilder.ToString().Trim();
            var arg = argBuilder.ToString().Trim();

            var found = CommandManager.Find(cmd);
            return found != null
                ? found.Execute(context, arg)
                : $"Invalid command '{cmd}'.";
        }
    }
}
