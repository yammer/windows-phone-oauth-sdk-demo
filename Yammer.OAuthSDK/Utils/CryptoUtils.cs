using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

namespace Yammer.OAuthSDK.Utils
{
    /// <summary>
    /// Utils class to handle cryptographic and encoding related operations.
    /// </summary>
    public static class CryptoUtils
    {
        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generates a (16 byte) base64 encoded string nonce
        /// </summary>
        /// <param name="length">(optional) number of bytes to use as the length</param>
        /// <returns>A random string nonce</returns>
        public static string GenerateUrlFriendlyNonce(int length = 16)
        {
            var data = new byte[length];
            random.GetBytes(data);
            return UrlTokenEncode(data);
        }

        /// <summary>
        /// Encrypt a string and store it in the phone's isolated storage
        /// </summary>
        /// <param name="value">The string to store</param>
        /// <param name="path">The path or "key" of the file to use for storage</param>
        public static void EncryptAndStore(string value, string path)
        {
            // Convert the string to a byte[].
            byte[] ValueByte = Encoding.UTF8.GetBytes(value);

            // Encrypt the string by using the Protect() method.
            byte[] ProtectedBytes = ProtectedData.Protect(ValueByte, null);

            // Store the encrypted string in isolated storage.
            StorageUtils.WriteToIsolatedStorage(ProtectedBytes, path);
        }

        /// <summary>
        /// Decrypt a string that is stored in the phone's isolated storage in the provided path
        /// </summary>
        /// <param name="path">The path or "key" of the file to use for storage</param>
        /// <returns>The decrypted stored string</returns>
        public static string DecryptStored(string path)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!file.FileExists(path)) return string.Empty;
            }
            // Retrieve the string from isolated storage.
            byte[] ProtectedValueByte = StorageUtils.ReadBytesFromIsolatedStorage(path);

            // Decrypt the string by using the Unprotect method.
            byte[] ValueByte = ProtectedData.Unprotect(ProtectedValueByte, null);

            // Convert the value from byte to string and return it.
            return Encoding.UTF8.GetString(ValueByte, 0, ValueByte.Length);
        }

        /// <summary>
        /// Encodes a byte array into its equivalent string representation using base 64 digits, which is usable for transmission on the URL.
        /// Taken from HttpServerUtility implementation in framework's system.web.dll
        /// </summary>
        /// <param name="input">The byte array to encode.</param>
        /// <returns>The string containing the encoded token if the byte array length is greater than one; otherwise, an empty string ("").</returns>
        public static string UrlTokenEncode(byte [] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (input.Length < 1)
                return String.Empty;
  
            string  base64Str   = null;
            int     endPos      = 0;
            char[]  base64Chars = null;
 
            ////////////////////////////////////////////////////////
            // Step 1: Do a Base64 encoding
            base64Str = Convert.ToBase64String(input);
            if (base64Str == null)
                return null;
 
            ////////////////////////////////////////////////////////
            // Step 2: Find how many padding chars are present in the end
            for (endPos = base64Str.Length; endPos > 0; endPos--)
            {
                if (base64Str[endPos - 1] != '=') // Found a non-padding char!
                {
                    break; // Stop here
                }
            }
  
            ////////////////////////////////////////////////////////
            // Step 3: Create char array to store all non-padding chars,
            //      plus a char to indicate how many padding chars are needed
            base64Chars = new char[endPos + 1];
            base64Chars[endPos] = (char)((int)'0' + base64Str.Length - endPos); // Store a char at the end, to indicate how many padding chars are needed
 
            ////////////////////////////////////////////////////////
            // Step 3: Copy in the other chars. Transform the "+" to "-", and "/" to "_"
            for (int iter = 0; iter < endPos; iter++)
            {
                char c = base64Str[iter];
  
                switch (c)
                {
                    case '+':
                        base64Chars[iter] = '-';
                        break;
  
                    case '/':
                        base64Chars[iter] = '_';
                        break;
 
                    case '=':
                        Debug.Assert (false);
                        base64Chars[iter] = c;
                        break;
  
                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }
            return new string(base64Chars);
        }
 
        /// <summary>
        /// Decodes a URL string token to its equivalent byte array using base 64 digits.
        /// Taken from HttpServerUtility implementation in framework's system.web.dll
        /// </summary>
        /// <param name="input">The URL string token to decode.</param>
        /// <returns>The byte array containing the decoded URL string token.</returns>
        static public byte [] UrlTokenDecode(string input) {
            if (input == null)
                throw new ArgumentNullException("input");
  
            int len = input.Length;
            if (len < 1)
                return new byte[0];
 
            ///////////////////////////////////////////////////////////////////
            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return null;
  
  
            ///////////////////////////////////////////////////////////////////
            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];
 
  
            ////////////////////////////////////////////////////////
            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];
 
                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;
  
                    case '_':
                        base64Chars[iter] = '/';
                        break;
 
                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }
 
            ////////////////////////////////////////////////////////
            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }
  
            // Do the actual conversion
            return Convert.FromBase64CharArray (base64Chars, 0, base64Chars.Length);
        }
    }
}
