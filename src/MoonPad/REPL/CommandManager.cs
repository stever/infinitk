using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoonPad.REPL
{
    internal static class CommandManager
    {
        private static readonly Dictionary<string, ICommand> Registry = new Dictionary<string, ICommand>();

        public static void Initialize()
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes()
                .Where(tt => typeof(ICommand).IsAssignableFrom(tt))
                .Where(tt => tt.IsClass && (!tt.IsAbstract))
            )
            {
                var o = Activator.CreateInstance(t);
                var cmd = (ICommand)o;
                Registry.Add(cmd.Name, cmd);
            }
        }

        public static IEnumerable<ICommand> GetCommands()
        {
            yield return Registry["help"];

            foreach (var cmd in Registry.Values.Where(c => !(c is HelpCommand)).OrderBy(c => c.Name))
            {
                yield return cmd;
            }
        }

        public static ICommand Find(string cmd)
        {
            return Registry.ContainsKey(cmd) ? Registry[cmd] : null;
        }
    }
}
