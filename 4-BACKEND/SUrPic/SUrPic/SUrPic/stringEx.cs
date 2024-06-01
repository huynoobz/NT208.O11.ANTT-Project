using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class stringEx
{
    /// <summary>
    /// Convert ASCII string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] asciiStr2Bytes(this string input)
    {
        return Encoding.ASCII.GetBytes(input);
    }

    /// <summary>
    /// Convert base 64 string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] base64Str2Bytes(this string input)
    {
        return System.Convert.FromBase64String(input);
    }

    /// <summary>
    /// Convert UTF-8 string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] utf8Str2Bytes(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    /// <summary>
    /// Convert UTF-8 string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] utf32Str2Bytes(this string input)
    {
        return Encoding.UTF32.GetBytes(input);
    }

    /// <summary>
    /// Convert UTF-8 string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] utf16Str2Bytes(this string input)
    {
        return Encoding.Unicode.GetBytes(input);
    }

    /// <summary>
    /// Convert hex string to byte array
    /// </summary>
    /// <returns>byte[]</returns>
    public static byte[] hexStr2Bytes(this string input)
    {
        return Convert.FromHexString(input);
    }

}
