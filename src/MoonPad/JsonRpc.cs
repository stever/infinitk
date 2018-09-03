using System.Text;

namespace MoonPad
{
    internal class JsonRpc
    {
        // https://en.wikipedia.org/wiki/JSON-RPC
        // {"jsonrpc":"2.0","method":"test","params":[],"id":2}

        public string jsonrpc;
        public string method;
        public string[] @params;
        public int id;

        public string GetLine()
        {
            var sb = new StringBuilder();
            sb.Append(method);

            foreach (var p in @params)
            {
                sb.Append($" {p}");
            }

            return sb.ToString();
        }
    }
}
