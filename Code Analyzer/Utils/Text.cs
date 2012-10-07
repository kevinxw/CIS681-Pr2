/*
 * A uitl class used to deal with texts
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kevin.CIS681.Project.CodeAnalyzer.Utils {
    class Text {

        // generate random string part starts
        // derived from http://goo.gl/9mJ1m
        private static readonly Random _rng = new Random();
        // this is a string including 64 characters
        private const string _chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ+/";

        public static string getRandomString(int size) {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            return new string(buffer);
        }
        // generate ramdom string part ends
    }
}
