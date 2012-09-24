using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kevin.CIS681.Project1.CodeAnalyzer.IO {
    class File {
        public const char NULL = '\0';
        // if pointer is at the end of stream
        private bool isEOS = false;
        private FileStream fs;
        private StreamReader sr;
        // file path should not be changed after the file is loaded
        private readonly string filePath;
        private bool _isTextFile = true;

        public File(string filePath) {
            this.filePath = filePath;
            fs = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read);

            determineTextFile();
        }
        public bool EOS {
            get {
                return isEOS;
            }
        }
        char read() {
            return (isEOS=(sr.Peek()<0)) ? NULL : (char) r.Read();
        }
        string readLine() {
            return (isEOS=(sr.Peek()<0)) ? String.Empty : r.ReadLine();
        }

        private void determineTextFile() {
            byte[] byteData = new byte[1];
            while (_isTextFile && fs.Read(byteData, 0, byteData.Length) > 0) {
                if (byteData[0] == 0)
                    _isTextFile = false;
            }
        }
        public bool isTextFile {
            get {
                return _isTextFile;
            }
        }
    }
}
