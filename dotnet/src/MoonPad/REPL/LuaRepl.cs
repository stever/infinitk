using System;
using System.Reflection;
using System.Text;
using log4net;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.VsCodeDebugger;

namespace MoonPad.REPL
{
    internal class LuaRepl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string DebuggerScriptName = "MoonSharp REPL interpreter";

        private readonly FormWindow formWindow;
        private readonly ScriptContext context;

        private Script script;
        private ReplInterpreter interpreter;
        private MoonSharpVsCodeDebugServer debugger;

        private BrowserBoundAppHost BrowserBoundAppHost => formWindow.BrowserBoundAppHost;

        public LuaRepl(FormWindow formWindow)
        {
            this.formWindow = formWindow;
            context = new ScriptContext(this);
            CommandManager.Initialize();
            InitContext();
        }

        public void InitContext()
        {
            // NOTE: Inspect script requires Metatables, and requiring scripts requires LoadMethods.
            const CoreModules sandbox = CoreModules.Preset_HardSandbox | CoreModules.LoadMethods | CoreModules.Metatables;

            //const CoreModules sandbox = CoreModules.Preset_HardSandbox | CoreModules.LoadMethods;
            //const CoreModules sandbox = CoreModules.Preset_SoftSandbox | CoreModules.LoadMethods;
            //const CoreModules sandbox = CoreModules.Preset_Complete;

            /*
            const CoreModules sandbox = CoreModules.None
                | CoreModules.Basic             // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.GlobalConsts      // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.TableIterators    // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.Metatables        //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.String            // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.LoadMethods       //                                         Preset_Default, Preset_Complete
                | CoreModules.Table             // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
//              | CoreModules.ErrorHandling     //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.Math              // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
//              | CoreModules.Coroutine         //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.Bit32             // Preset_HardSandbox, Preset_SoftSandbox, Preset_Default, Preset_Complete
//              | CoreModules.OS_Time           //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
//              | CoreModules.OS_System         //                                         Preset_Default, Preset_Complete
//              | CoreModules.IO                //                                         Preset_Default, Preset_Complete
//              | CoreModules.Debug             //                                                         Preset_Complete
//              | CoreModules.Dynamic           //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
                | CoreModules.Json              //                     Preset_SoftSandbox, Preset_Default, Preset_Complete
                ;
            */

            debugger?.Detach(script);

            script = new Script(sandbox)
            {
                Options =
            {
                DebugPrint = s => BrowserBoundAppHost.SendPrintOutput(s),
                ScriptLoader = new LuaReplScriptLoader(formWindow)
            }
            };

            debugger?.AttachToScript(script, DebuggerScriptName);

            interpreter = new ReplInterpreter(script)
            {
                HandleDynamicExprs = false,
                HandleClassicExprsSyntax = true
            };
        }

        public void StartDebugger()
        {
            debugger = new MoonSharpVsCodeDebugServer();
            debugger.Start();

            // http://www.moonsharp.org/debugger.html
            // See also "Mapping VSCode debugger to file system files"
            debugger.AttachToScript(script, DebuggerScriptName);
        }

        public string HandleInput(string input)
        {
            try
            {
                if (input.StartsWith("!"))
                {
                    return ExecuteCommand(context, input.Substring(1));
                }

                var result = interpreter.Evaluate(input);
                if (result == null)
                {
                    return null;
                }

                var output = result.Type != DataType.Void
                    ? result.ToString().TrimEnd('\r', '\n')
                    : "";

                Log.DebugFormat("In: \"{0}\", Out: \"{1}\"", input, output);
                return output;
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
                Log.Error("EXCEPTION", ex);
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
