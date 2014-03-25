//-----------------------------------------------------------------------
// <copyright file="TextExtensionMethods.cs" company="Tiempo Development">
//     Copyright Tiempo Development. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WcfRadiant.Utils.ExtensionMethods.Text//TD.Utils.ExtensionMethods.Text
{
    using System;
    using System.Text;
    using System.Security;

    /// <summary>
    /// Class for Extension methods
    /// </summary>
    public static class TextExtensionMethods
    {
        /// <summary>
        /// Returns a value indicating whether the specified String object occurs within this StringBuilder
        /// </summary>
        /// <param name="pStringBuilder">StringBuilder to seek into</param>
        /// <param name="pValue">The string to seek</param>
        /// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false</returns>
        public static bool Contains(this StringBuilder pStringBuilder, string pValue)
        {
            try
            {
                return pStringBuilder.ToString().Contains(pValue);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a string that contains a specified number of characters from the left side of a string.
        /// </summary>
        /// <param name="pString">String expression from which the leftmost characters are returned.</param>
        /// <param name="pLength">Numeric expression indicating how many characters to return. If zero, a zero-length string ("") is returned. If greater than or equal to the number of characters in str, the complete string is returned.</param>
        /// <returns>Leftmost characters from the string</returns>
        public static string Left(this string pString, int pLength)
        {
            try
            {
                if (string.IsNullOrEmpty(pString))
                {
                    return pString;
                }

                if (pLength < 0)
                {
                    throw new ArgumentException("The length must not be negative", "pLength");
                }

                if (pLength > pString.Length)
                {
                    return pString;
                }
                else
                {
                    return pString.Substring(0, pLength);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a string that contains a specified number of characters from the right side of a string.
        /// </summary>
        /// <param name="pString">String expression from which the rightmost characters are returned.</param>
        /// <param name="pLength">Numeric expression indicating how many characters to return. If zero, a zero-length string ("") is returned. If greater than or equal to the number of characters in str, the complete string is returned.</param>
        /// <returns>Rightmost characters from the string</returns>
        public static string Right(this string pString, int pLength)
        {
            try
            {
                if (string.IsNullOrEmpty(pString))
                {
                    return pString;
                }

                if (pLength < 0)
                {
                    throw new ArgumentException("The length must not be negative", "pLength");
                }

                if (pLength > pString.Length)
                {
                    return pString;
                }
                else
                {
                    return pString.Substring(pString.Length - pLength);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a string that contains the string from the start to the index of "ToString"
        /// </summary>
        /// <param name="pString">String expression from which the substring characters are returned.</param>
        /// <param name="pToString">Substring to be found on the string</param>
        /// <returns>Substring</returns>
        public static string SubstringTo(this string pString, string pToString)
        {
            try
            {
                if (string.IsNullOrEmpty(pString))
                {
                    return pString;
                }

                int index = pString.IndexOf(pToString);

                return pString.Substring(0, index);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Convert unsecure string to SecureString.
        /// </summary>
        /// <param name="pPassword">Pass unsecure string for conversion.</param>
        /// <returns>SecureString</returns>
        public static SecureString ToSecureString(this string pPassword)
        {
            try
            {
                if (pPassword == null)
                {
                    throw new ArgumentNullException("pPassword");
                }

                var securePassword = new SecureString();
                foreach (char c in pPassword)
                {
                    securePassword.AppendChar(c);
                }

                securePassword.MakeReadOnly();

                return securePassword;
            }
            catch
            {
                throw;
            }
        }
        
        /// <summary>
        /// Returns a four-character (SOUNDEX) code to evaluate the similarity of two strings.
        /// </summary>
        /// <param name="pString">String to be evaluated.</param>
        /// <remarks>SOUNDEX converts an alphanumeric string to a four-character code to find similar-sounding words or names. The first character of the code is the first character of character_expression and the second through fourth characters of the code are pNumbers. Vowels in character_expression are ignored unless they are the first letter of the string. String functions can be nested.</remarks>
        /// <returns>A four-character code of the string.</returns>
        public static string Soundex(this string pString)
        {
            try
            {
                StringBuilder result = new StringBuilder();

                if (pString != null && pString.Length > 0)
                {
                    string previousCode, currentCode;

                    result.Append(Char.ToUpper(pString[0]));
                    previousCode = string.Empty;

                    for (int i = 1; i < pString.Length; i++)
                    {
                        currentCode = EncodeChar(pString[i]);

                        if (currentCode != previousCode)
                        {
                            result.Append(currentCode);
                        }

                        if (result.Length == 4)
                        {
                            break;
                        }

                        if (!currentCode.Equals(string.Empty))
                        {
                            previousCode = currentCode;
                        }
                    }
                }

                if (result.Length < 4)
                {
                    result.Append(new String('0', 4 - result.Length));
                }

                return result.ToString();
            }
            catch
            {                
                throw;
            }
        }

        /// <summary>
        /// Converts a character to their Soundex code
        /// </summary>
        /// <param name="pChar">Character to be converted</param>
        /// <returns>A Soundex code</returns>
        private static string EncodeChar(char pChar)
        {
            try
            {
                switch (Char.ToLower(pChar))
                {
                    case 'b':
                    case 'f':
                    case 'p':
                    case 'v':
                        return "1";
                    case 'c':
                    case 'g':
                    case 'j':
                    case 'k':
                    case 'q':
                    case 's':
                    case 'x':
                    case 'z':
                        return "2";
                    case 'd':
                    case 't':
                        return "3";
                    case 'l':
                        return "4";
                    case 'm':
                    case 'n':
                        return "5";
                    case 'r':
                        return "6";
                    default:
                        return string.Empty;
                }
            }
            catch
            {                
                throw;
            }
        }

        /// <summary>
        /// Returns an integer value that indicates the difference between the SOUNDEX values of two character expressions.
        /// </summary>
        /// <param name="pString">String to be evaluated.</param>
        /// <param name="pStringToCompare">String to be evaluated.</param>
        /// <remarks>The integer returned is the number of characters in the SOUNDEX values that are the same. The return value ranges from 0 through 4: 0 indicates weak or no similarity, and 4 indicates strong similarity or the same values.</remarks>
        /// <returns>An Int (0-4) indicating the similarity between the strings.</returns>
        public static int Difference(this string pString, string pStringToCompare)
        {
            try
            {
                int result = 0;

                if (pString.Equals(string.Empty) || pStringToCompare.Equals(string.Empty))
                {
                    return 0;
                }

                string soundex1 = Soundex(pString);

                string soundex2 = Soundex(pStringToCompare);

                if (soundex1.Equals(soundex2))
                {
                    result = 4;
                }
                else
                {
                    if (soundex1[0] == soundex2[0])
                    {
                        result = 1;
                    }

                    string sub1 = soundex1.Substring(1, 3); //characters 2, 3, 4

                    if (soundex2.IndexOf(sub1) > -1)
                    {
                        result += 3;

                        return result;
                    }

                    string sub2 = soundex1.Substring(2, 2); //characters 3, 4

                    if (soundex2.IndexOf(sub2) > -1)
                    {
                        result += 2;

                        return result;
                    }

                    string sub3 = soundex1.Substring(1, 2); //characters 2, 3

                    if (soundex2.IndexOf(sub3) > -1)
                    {
                        result += 2;

                        return result;
                    }

                    char sub4 = soundex1[1];

                    if (soundex2.IndexOf(sub4) > -1)
                    {
                        result++;
                    }

                    char sub5 = soundex1[2];

                    if (soundex2.IndexOf(sub5) > -1)
                    {
                        result++;
                    }

                    char sub6 = soundex1[3];

                    if (soundex2.IndexOf(sub6) > -1)
                    {
                        result++;
                    }
                }

                return result;
            }
            catch
            {                
                throw;
            }
        }
    }
}