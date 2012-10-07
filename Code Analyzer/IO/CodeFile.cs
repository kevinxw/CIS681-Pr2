using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kevin.CIS681.Project.CodeAnalyzer.IO {
    class CodeFile {
        public const char NULL = '\0';
        // if pointer is at the end of stream
        private bool _fileExist = true,   // whether the source file exist
            _isTextFile = true;    // whether the source file is a text file
        private FileStream fs;
        private StreamReader sr;
        // file path should not be changed after the file is loaded
        private readonly string filePath;


        public CodeFile(string filePath) {
            this.filePath = filePath;

            try {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);
            }
            catch (FileNotFoundException e) {
                _fileExist = false;
            }
            catch (Exception e) {
                return;
            }
            determineTextFile();
        }
        public bool fileExist { get { return _fileExist; } }
        public bool EndOfStream {
            get {
                return sr.EndOfStream;
            }
        }
        public char read() {
            return sr.EndOfStream ? NULL : (char) sr.Read();
        }
        public string readLine() {
            return sr.EndOfStream ? String.Empty : sr.ReadLine();
        }

        private void determineTextFile() {
            byte[] byteData = new byte[1];
            while (_isTextFile && fs.Read(byteData, 0, byteData.Length) > 0)
                if (byteData[0] == 0)
                    _isTextFile = false;
        }
        public bool isTextFile {
            get {
                return _isTextFile;
            }
        }
    }
}
