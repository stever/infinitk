namespace MoonPad
{
    internal class JsonRpc
    {
        // https://en.wikipedia.org/wiki/JSON-RPC
        // {"jsonrpc":"2.0","method":"test","params":[],"id":2}

        public string jsonrpc { get; set; }
        public string method;
        public string[] @params;
        public int id;
    }
}
