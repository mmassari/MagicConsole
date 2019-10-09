namespace MagicConsole
{
    public enum CommandTypes { Exit, Help, Custom }
    public enum MenuType { Standard, Advanced }
    public enum OptionType
    {
        Flag, Date, Enum, Number, String
    }
    public enum InfoItem
    {
        fullpath, filename, title, description, company, product, copyright, trademark, assemblyVersion, fileVersion, guid, language
    }

}