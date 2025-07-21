namespace StringFiltering.Application.Utilities;

public interface IStringArchiver
{
    byte[] Archive(string valueToArchive);
    string Unarchive(byte[] valueToUnarchive);
}