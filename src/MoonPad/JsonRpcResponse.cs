namespace MoonPad
{
    internal class JsonRpcResponse
    {
        // https://en.wikipedia.org/wiki/JSON-RPC
        // {"jsonrpc":"2.0","result":19,"id":2}

        public string jsonrpc;
        public string result;
        public int id;
    }
}
