public class ErrorType
{
    public ErrorType()
    {

    }

    public ErrorType(string message)
    {
        this.code = "-1";
        this.message = message;
    }

    public ErrorType(string code, string message)
    {
        this.code = code;
        this.message = message;
    }

    public ErrorType(string code, string message, object values)
    {
        this.code = code;
        this.message = message;
        this.values = values;
    }

    public string code { get; set; }
    public string message { get; set; }
    public object values { get; set; }
}
