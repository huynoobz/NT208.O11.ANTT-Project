using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class bytesEx
{
    /// <summary>
    /// Return a ASCII string
    /// </summary>
    /// <returns>ASCII string</returns>
    public static string toASCIIStr(this byte[] input)
    {
        return Encoding.ASCII.GetString(input);
    }

    /// <summary>
    /// Return a Base 64 string
    /// </summary>
    /// <returns>Base 64 string</returns>
    public static string toBase64Str(this byte[] input)
    {
        return System.Convert.ToBase64String(input);
    }

    /// <summary>
    /// Return a UTF-8 string
    /// </summary>
    /// <returns>UTF-8 string</returns>
    public static string toUTF8Str(this byte[] input)
    {
        return Encoding.UTF8.GetString(input);
    }

    /// <summary>
    /// Return a UTF-32 string
    /// </summary>
    /// <returns>UTF-32 string</returns>
    public static string toUTF32Str(this byte[] input)
    {
        return Encoding.UTF32.GetString(input);
    }

    /// <summary>
    /// Return a UTF-16(Unicode) string
    /// </summary>
    /// <returns>UTF-16 string</returns>
    public static string toUTF16Str(this byte[] input)
    {
        return Encoding.Unicode.GetString(input);
    }

    /// <summary>
    /// Return a Byte Array with fisrt element is 0x00
    /// </summary>
    /// <returns>unsigned byte[]</returns>
    public static byte[] toUnsignedBytes(this byte[] input)
    {
        byte[] output=new byte[input.Length+1];
        output[0] = 0x00;
        Array.Copy(input, 0, output, 1, input.Length);
        return output;
    }

    /// <summary>
    /// Return a hex string
    /// </summary>
    /// <returns>Hex string</returns>
    public static string toHexStr(this byte[] input)
    {
        return Convert.ToHexString(input);
    }

    /// <summary>
    /// Add a byte to a byte array at addIndex
    /// </summary>
    /// <returns>A new byte[] with addByte at addIndex</returns>
    public static byte[] addByte(this byte[] input,byte addByte,int addIndex)
    {
        byte[] output = new byte[input.Length + 1];
        output[addIndex] = addByte;
        Array.Copy(input, 0, output, 0, addIndex);
        Array.Copy(input, addIndex, output, addIndex+1, input.Length-addIndex);
        return output;
    }

    /// <summary>
    /// Add a byte to the end of a byte array
    /// </summary>
    /// <returns>A new byte[] with addByte at the end</returns>
    public static byte[] addByteAtLast(this byte[] input, byte addByte)
    {
        return input.addByte(addByte,input.Length);
    }

}
