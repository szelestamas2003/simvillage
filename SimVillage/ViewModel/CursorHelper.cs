using System.IO;
using System.Windows.Input;

public static class CursorHelper
{
    public static Cursor FromByteArray(byte[] array)
    {
        using (MemoryStream memoryStream = new MemoryStream(array))
        {
            return new Cursor(memoryStream);
        }
    }
}