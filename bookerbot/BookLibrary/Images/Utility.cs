namespace bookerbot.Images;

public static class Utility
{
    public static string GetDirectoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Images/";
}