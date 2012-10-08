/*
 * This is a simple version of parser that using Regular Expression
 * This parser does not use any grammar loader, thus, it only works with C#
 * 
 *
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class SimpleParser {
        // regex for computing cc
        private const string ccRegExStr = @"if|break|continue|for|while|foreach";

        private TextReader tr;
        private List<string> file = new List<string>();
        private XElement xElem = null;

        public SimpleParser(string filePath) {
            tr = new StreamReader(filePath);
            string text = tr.ReadToEnd();
            if (text == null)
                return;
            text = " " + text.Replace("\n", "\n "); // to make regex test easier
            text = eatComment(text);
            text = eatUsing(text);
            text = eatVariableDeclaration(text);
            text = eatEvaluatation(text);
            text = eatFunctionCall(text);
            string xml = toXML(text);
            try {
                xElem = XElement.Parse(xml);
            }
            catch (Exception e) {
                Console.Out.WriteLine("Oops! A error just occured when parsing XML data of {0}, {1}", filePath,e.Message);
                return;
            }
            computeCC(xElem);
            Console.Out.WriteLine(xElem.ToString());
        }

        // delete all the comment
        private static readonly Regex singleLineCommentRegEx = new Regex(@"//(.*)", RegexOptions.Compiled);
        private static readonly Regex multiLineCommentRegEx = new Regex(@"(?<!/)/\*([^*/]|\*(?!/)|/(?<!\*))*((?=\*/))(\*/)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex RegularStringRegEx = new Regex("((?<!\\\\)\"([^\"\\\\]|(\\\\.))*\")", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex AtStringRegEx = new Regex("@(\"([^\"]*)\")(\"([^\"]*)\")*", RegexOptions.Multiline | RegexOptions.Compiled);
        private string eatComment(string text) {
            Match mat = null;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\"') {
                    mat = RegularStringRegEx.Match(text, i);
                    if (mat.Success)
                        i = mat.Index + mat.Length;
                }
                else if (text[i] == '@') {
                    mat = AtStringRegEx.Match(text, i);
                    if (mat.Success)
                        i = mat.Index + mat.Length;
                }
                else if (text[i] == '/') {
                    mat = singleLineCommentRegEx.Match(text, i);
                    if (mat.Success) {
                        text = text.Remove(mat.Index, mat.Length);
                        i--;
                    }
                    else {
                        mat = multiLineCommentRegEx.Match(text, i);
                        if (mat.Success) {
                            text = text.Remove(mat.Index, mat.Length);
                            i--;
                        }
                    }
                }
            }
            return text;
        }
        // end comment part


        private const string variableRegExStr = @"\w+(?:\.\w+)*";
        private const string genericVariableRegExStr = variableRegExStr + @"(?:<\s*\w[\[\].\w<>,\s]*>)?";
        private const string genericFuncCallRegExStr = @"(?:[a-z]+\s+)?" + variableRegExStr + @"\s*\(\s*\w[\w,\s]*\)";

        // delete all "using namespace"
        private static readonly Regex usingRegEx = new Regex(@"\susing\s+.+?;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatUsing(string text) {
            return usingRegEx.Replace(text, "");
        }

        // delete all variable declaration
        private static readonly Regex variableDeclarationRegEx = new Regex(@"([a-z]+\s+)*" + genericVariableRegExStr + @"\s+\w+\s*((,|=).+?)?;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatVariableDeclaration(string text) {
            return variableDeclarationRegEx.Replace(text, "");
        }

        // delete all evaluate phrases
        private static readonly Regex evaluatationRegEx = new Regex("\\s" + genericVariableRegExStr + @"\s*(=\s*(" + genericVariableRegExStr + "|" + genericFuncCallRegExStr + @")\s*)+;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatEvaluatation(string text) {
            return evaluatationRegEx.Replace(text, "");
        }

        // eat everything except keywords
        private static readonly Regex funcCallRegEx = new Regex("\\s" + genericFuncCallRegExStr + @"\s*;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatFunctionCall(string text) {
            return funcCallRegEx.Replace(text, "");
        }
        private const string controlRegExWithoutBrace = @"\s(" + ccRegExStr + @")\s+(?:\(.+\))?\s*";
        // convert the code to xml
        private static readonly Regex namespaceRegEx = new Regex(@"\snamespace\s+(\w+(?:\.\w+)*)\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex classRegEx = new Regex(@"\sclass\s+(\w+)\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex methodEx = new Regex("(" + genericFuncCallRegExStr + @")\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex controlRegEx = new Regex(controlRegExWithoutBrace + "{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex controlRegEx2 = new Regex(controlRegExWithoutBrace , RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex everyOtherThingRegEx = new Regex(@">[^<]+<", RegexOptions.Compiled | RegexOptions.Multiline);
        private string toXML(string text) {
            string xml = text.Replace("<", "&lt;").Replace(">", "&gt;");
            xml = xml.Replace(" try ", "").Replace(" catch ","");   // delete try catch
            xml = namespaceRegEx.Replace(xml, @"<elem type=""namespace"" name=""$1"">");
            xml = classRegEx.Replace(xml, @"<elem type=""class"" name=""$1"">");
            xml = controlRegEx.Replace(xml, @"<elem type=""control"" name=""$1"">");
            xml = controlRegEx2.Replace(xml, @"<elem type=""control"" name=""$1""/>");
            xml = methodEx.Replace(xml, @"<elem type=""method"" name=""$1"">");
            xml = xml.Replace("{", @"<elem type=""anonymous"">");
            xml = xml.Replace("}", "</elem>");
            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><file>" + xml + "</file>";
            xml = everyOtherThingRegEx.Replace(xml, "><");
            return xml;
        }

        // compute cyclomatic complexity
        private void computeCC(XElement xml) {
            IEnumerable<XElement> elems = from nd in xml.Descendants("elem")
                                          where nd.Attribute("type").Value == "method"
                                          select nd;
            foreach (XElement el in elems) {
                int cc = (from nd in el.Descendants("elem")
                         where nd.Attribute("type").Value == "control"
                         select nd).Count();
                el.SetAttributeValue("cc", 1+cc);
            }
            elems = from nd in xml.Descendants("elem")
                    where nd.Attribute("type").Value == "anonymous" && !nd.HasElements
                    select nd;
            elems.Remove();
        }
    }
}
