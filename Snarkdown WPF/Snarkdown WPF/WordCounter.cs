using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snarkdown_WPF
{
    static public class WordCounter
    {
        static private bool[] m_spaceChar;

        static public void initSpaceCharTable()
        {
            m_spaceChar = new bool[256];
            string spaceChars = " \t\r\n\v\f";
            for (int i = 0; i < spaceChars.Length; i++)
            {
                m_spaceChar[spaceChars[i]] = true;
            }
        }

        static public int CountWordsInString(string s)
        {
            int words = 0;
            // stub for the actual counting of words in string "s"

            // switch between algorithms?

            // borrowed and modified from http://nanowritool.sourceforge.net/

            bool inWord = false;
            int length = s.Length;

            for (int i = 0; i < length; i++)
            {
                char c = s[i];
                if (c > 0xFF || !m_spaceChar[c])
                {
                    // word char
                    if (!inWord)
                    {
                        // mark transition
                        inWord = true;
                        ++words;
                    }
                }
                else
                {
                    // space char
                    inWord = false;
                }
            }


            return words;
        }

        static public int CountWordsInProj()
        {
            int words = 0;
            // stub for counting words in each document in the project
            // possibly takes an array of file objects to be sent to the tree view (so as to display the word counts)

            return words;
        }


        /// <summary>
        /// Find out the number of bytes in the string for Kindle
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static private int GetKindleBytes(string text)
        {
            int b = 0;
            if (text != null && text.Length > 0)
            {
                // TODO: ADD IN CONVERSION TO .MOBI
                b = Encoding.UTF8.GetByteCount(text);
                //b = text.Length;
            }
            return b;
        }

        /// <summary>
        /// returns the location number of a position in bytes
        /// </summary>
        /// <param name="bytes">the number of bytes in .mobi</param>
        /// <returns>Kindle Location number</returns>
        static private int GetKindleLocationFromBytes(int bytes)
        {
            int loc = 0;

            loc = bytes / 128;

            return loc;
        }

        /// <summary>
        /// Get the Kindle Location from a string.
        /// </summary>
        /// <param name="s">string of text ending at the location</param>
        /// <returns>Int as a Kindle Location</returns>
        static public int GetKindleLocation(string s)
        {
            return GetKindleLocationFromBytes(GetKindleBytes(s));
        }

        /// <summary>
        /// Get the character number of the start of a given Kindle Location
        /// </summary>
        /// <param name="loc">a given Kindle Location</param>
        /// <returns>int char index in string</returns>
        static public int GetIndexFromKindleLocation(int loc)
        {
            return loc * 128;
        }
    }
}
