using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EM.Collections
{
    public class StringUtil
    {
        public const string CRLF = "\r\n";        

        /// <summary>
        /// see EList.slice
        /// </summary>
        public static string slice(string txt, int idxFrom)
        {
            EList<char> lst = EList<char>.fromAray(txt.ToCharArray());
            EList<char> res = lst.slice(idxFrom);
            return res.join("");
        }

        /// <summary>
        /// see EList.slice
        /// </summary>
        public static string slice(string txt, int? idxFrom, int? idxTo)
        {
            EList<char> lst = EList<char>.fromAray(txt.ToCharArray());
            EList<char> res = lst.slice(idxFrom, idxTo);
            return res.join("");
        }

        public static EList<string> split(string txt, string splitter)
        {
            return EList<string>.fromAray(txt.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// creates a file with the given content. If a file already exist, it will be overritten. Uses ASCII encoding.
        /// </summary>
        public static void writeToFile(DirectoryInfo destFolder, string fileName, string content)
        {
            writeToFile(destFolder, fileName, content, false);
        }
        /// <summary>
        /// creates a file with the given content. If a file already exist, you can choose to append to it or overwrite it. Uses ASCII encoding.
        /// </summary>
        public static void writeToFile(DirectoryInfo destFolder, string fileName, string content, bool appendToExistingFileContent)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            //UnicodeEncoding enc = new UnicodeEncoding();
            writeToFile(destFolder, fileName, content, appendToExistingFileContent, enc);
        }
        public static void writeToFile(DirectoryInfo destFolder, string fileName, string content, bool appendToExistingFileContent, Encoding encoding )
        {
            FileStream fs;
            FileInfo destFile = new FileInfo(destFolder.FullName + "\\" + fileName);

            if (!appendToExistingFileContent && destFile.Exists)
            {
                destFile.Delete(); //we need to overwrite
                fs = new FileStream(destFile.FullName, FileMode.Create);
            }
            else if (destFile.Exists)
            {
                fs = new FileStream(destFile.FullName, FileMode.Open);
            }
            else
            {
                fs = new FileStream(destFile.FullName, FileMode.Create);
            }

            fs.Position = fs.Length;
            fs.Write(encoding.GetBytes(content + StringUtil.CRLF), 0, content.Length + 1);
            fs.Close();
        }
        
        public static Encoding getFileEncoding(string filePath)
        {
            Encoding enc = null;
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (file.CanSeek)
            {
                byte[] bom = new byte[4]; // Get the byte-order mark, if there is one
                file.Read(bom, 0, 4);
                file.Seek(0, System.IO.SeekOrigin.Begin);
                
                enc = getContentEncoding(bom);
            }
            else
            {
                // The file cannot be randomly accessed, so you need to decide what to set the default to
                // based on the data provided. If you're expecting data from a lot of older applications,
                // default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer
                // applications, default your encoding to Encoding.Unicode. Also, since binary files are
                // single byte-based, so you will want to use Encoding.ASCII, even though you'll probably
                // never need to use the encoding then since the Encoding classes are really meant to get
                // strings from the byte array that is the file.

                enc = Encoding.ASCII;
            }
            
            file.Close();

            return enc;

        }

        //it won't detect unicode if there is no byte order mark 
        //yet each char is on 2 bytes for example (utf-16) 
        public static Encoding getContentEncoding(byte[] content)
        {
            Encoding enc = null;
            
            // Check the byte-order mark, if there is one (first 4 bytes)
            if ((content[0] == 0xef && content[1] == 0xbb && content[2] == 0xbf) || // utf-8
                (content[0] == 0xff && content[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le
                (content[0] == 0xfe && content[1] == 0xff) || // utf-16 and ucs-2
                (content[0] == 0 && content[1] == 0 && content[2] == 0xfe && content[3] == 0xff)) // ucs-4
            {
                enc = Encoding.Unicode;   //utf-16  should cover them all  
            }
            else
            {
                enc = Encoding.ASCII;
            }
            return enc;
        }

        public static string getTextFileContent(string filePath) { return getTextFileContent(filePath, null); }
        public static string getTextFileContent(string filePath, Encoding enc)
        {
            StreamReader sr;
            if (enc == null)
            {
                //detect encoding, 
                sr = new StreamReader(filePath, true);                  
            }
            else
            {
                sr = new StreamReader(filePath, enc);
            }
            string fileContent = sr.ReadToEnd();
            sr.Close();

            return fileContent;
        }

        public static IEnumerable<string> getLinesInFile(string filePath) { return getLinesInFile(filePath, null); }
        public static IEnumerable<string> getLinesInFile(string filePath, Encoding enc)
        {
            StreamReader sr = null;
            try
            {
                if (enc == null)
                {
                    //detect encoding, 
                    sr = new StreamReader(filePath, true);
                }
                else
                {
                    sr = new StreamReader(filePath, enc);
                }
                string input;
                while ((input = sr.ReadLine()) != null)
                {
                    yield return input;
                }
            }
            finally
            {
                if (sr != null) { sr.Close(); }
            }

        }

        


    }
}
