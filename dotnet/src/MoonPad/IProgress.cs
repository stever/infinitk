namespace MoonPad
{
    internal interface IProgress
    {
        int Maximum { set; }
        int Step { set; }
        int Value { set; }
        string Message { set; }
    }
}
