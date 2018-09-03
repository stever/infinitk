using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;

namespace MoonPad
{
    internal class LuaRepl
    {
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
            try
            {
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
    }
}
