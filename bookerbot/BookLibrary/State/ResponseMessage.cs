namespace BookLibrary.State;

public class ResponseMessage
{
    public string Text { get; set; }
    public List<string> UpButtons { get; set; } = new();
    public List<string> DownButtons { get; set; } = new();
    public List<string> Buttons { get; set; } = new();
    public EResponseMessageType ResponseMessageType { get; set; }
    public string? PhotoUrl { get; set; }
}

public enum EResponseMessageType
{
    Text,
    Photo,
}