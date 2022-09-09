namespace BookLibrary.Images;

public static class Utility
{
    public static string GetDirectoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "/BookLibrary/Images/";
}