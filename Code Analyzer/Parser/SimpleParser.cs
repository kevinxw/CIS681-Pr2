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
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using System.Xml;

namespace Kevin.CIS681.Project.CodeAnalyzer.Parser {
    class SimpleParser : IParser {
        // regex for computing cc
        private const string ccRegExStr = "if|break|continue|for|while|foreach";

        public const string ELEM_METHOD = "method";
        public const string ELEM_FILE = "file";
        public const string ELEM_CLASS = "class";
        public const string ELEM_NAMESPACE = "namespace";
        public const string ELEM_CONTROL = "control";
        public const string ELEM_UNKNOWN = "anonymous";
        public const string ELEM_ATTRIBUTE = "attribute";
        public const string ELEM_ATTRIBUTE_BODY = "attribute-body";
        public const string ELEM_LINE_TERMINATOR = "line-terminator";

        private string filePath = null, fileContent = null;
        private XElement xElem = null;

        public SimpleParser(string filePath) {
            using (TextReader tr = new StreamReader(this.filePath = filePath)) {
                fileContent = tr.ReadToEnd();
            };

        }

        public void save(string savePath) {
            if (xElem != null)
                xElem.Save(savePath);
            else
                Logger.error("Failed to save file {0}, as something wrong happened with parsing", filePath);
        }
        // default save the report to the same directory which source files located in
        public void save() {
            try {
                xElem.Save(filePath + ".ca.xml");
            }
            catch (Exception e) {
                Logger.error(e.Message);
            }
        }

        public XElement xElement {
            get {
                return xElem;
            }
        }

        public void read() {
            if (fileContent == null)
                return;
            string text = " " + fileContent.Replace("\n", "\n "); // to make regex test easier
            // attent, must eat comment 1st!
            // this operation list has an order! that's why I don't use delegate!
            text = eatComment(text);
            text = eatString(text);
            text = eatUsing(text);
            //text = eatLinq(text);
            //text = eatArrayMark(text);
            text = eatCollection(text);
            //text = eatVariableDeclaration(text);
            //text = eatEvaluatation(text);
            //text = eatFunctionCall(text);
            text = eatForLoop(text);
            //Logger.debug(text);
            //Console.Out.WriteLine(text);
            string xml = toXML(text);
            try {
                xElem = XElement.Parse(xml);
            }
            catch (Exception e) {
                Logger.error("Oops! A error just occured when parsing XML data of {0}, {1}", filePath, e.Message);
                return;
            }
            computeCC(xElem);
            computeSize(xElem);
            // remove useless anonymous blocks and line terminators after computing size
            removeAnonymousBlock(xElem);
            removeLineTerminators(xElem);
            // set attribute to file elem
            xElem.Element("elem").SetAttributeValue("path", filePath);
            Console.Out.WriteLine(xElem.ToString());
        }

        // delete all the comment
        private static readonly Regex singleLineCommentRegEx = new Regex(@"//(.*)", RegexOptions.Compiled);
        private static readonly Regex multiLineCommentRegEx = new Regex(@"(?<!/)/\*([^*/]|\*(?!/)|/(?<!\*))*((?=\*/))(\*/)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex RegularStringRegEx = new Regex("((?<!\\\\)\"([^\"\\\\]|(\\\\.))*\")", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex AtStringRegEx = new Regex("@\"[^\"]*\"(\"[^\"]*\")*", RegexOptions.Multiline | RegexOptions.Compiled);
        private string eatComment(string text) {
            Match mat = null;
            bool hasQuoteSymbol = false;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '"') {
                    hasQuoteSymbol = !hasQuoteSymbol;
                }
                else
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
                        if (!hasQuoteSymbol && mat.Success) {
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

        private readonly Regex LinqRegEx = new Regex(@"from\s+.+?\s+select\s+.+?;", RegexOptions.Compiled);
        private string eatLinq(string text) {
            return LinqRegEx.Replace(text, "LINQ;");
        }

        // notice the symbol \ is also a escape symbol in Regular Expression
        private readonly Regex escapedQuoteRegEx = new Regex(@"(\\\\)*\\""", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Regex quoteRegEx = new Regex("\".*?\"", RegexOptions.Compiled | RegexOptions.Multiline);
        // delete all string including @string
        private string eatString(string text) {
            string str = AtStringRegEx.Replace(text, "\"\"");
            str = escapedQuoteRegEx.Replace(str, "");
            return quoteRegEx.Replace(str, "''");
        }

        private const string variableRegExStr = @"\w+(?:\.\w+)*";
        private const string genericVariableRegExStr = variableRegExStr + @"(?:<\s*\w[\[\].\w<>,\s]*>)?";
        private const string genericFuncCallRegExStr = @"(?:[a-z]+\s+)*" + variableRegExStr + @"\s*\(.*\)";   // this is not a strict match!, as RegEx cannot detect the nested structure, such as FuncA(FuncB())
        private const string funcDefinationRegExStr = @"(?:[a-z]+\s+)*(" + variableRegExStr + @")\s*\([^\)]*\)"; // only match method declaration

        private static readonly Regex arrayMarkRegEx = new Regex(@"\[.*?]\]", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatArrayMark(string text) {
            while (arrayMarkRegEx.IsMatch(text))
                text = arrayMarkRegEx.Replace(text, "");
            return text;
        }

        // delete all collections, which "looks like" a function
        private static readonly Regex collectionRegEx = new Regex(@"\s*new\s+" + genericVariableRegExStr + @"(?:\[\s*\]|\(\s*\))\s*{[^}]*}", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatCollection(string text) {
            return collectionRegEx.Replace(text, "");
        }

        // delete conditions of for loops (wipe out ; delimiter)
        private static readonly Regex forLoopRegEx = new Regex(@"\(.*?;.*?;.*?\)", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatForLoop(string text) {
            return forLoopRegEx.Replace(text, "()");
        }

        // delete all "using namespace"
        private static readonly Regex usingRegEx = new Regex(@"\susing\s+.+?;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatUsing(string text) {
            return usingRegEx.Replace(text, "");
        }

        // delete all variable declaration
        private static readonly Regex variableDeclarationRegEx = new Regex(@"([a-z]+\s+)*" + genericVariableRegExStr + @"\s+\w+\s*((,|=).+?)?;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatVariableDeclaration(string text) {
            return variableDeclarationRegEx.Replace(text, ";");
        }

        // delete all evaluate phrases
        private static readonly Regex evaluatationRegEx = new Regex("\\s" + genericVariableRegExStr + @"\s*(=\s*(" + genericVariableRegExStr + "|" + genericFuncCallRegExStr + @")\s*)+;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatEvaluatation(string text) {
            return evaluatationRegEx.Replace(text, ";");
        }

        // eat everything except keywords
        private static readonly Regex funcCallRegEx = new Regex("\\s" + genericFuncCallRegExStr + @"\s*;", RegexOptions.Compiled | RegexOptions.Multiline);
        private string eatFunctionCall(string text) {
            return funcCallRegEx.Replace(text, ";");
        }

        private const string controlRegExWithoutBrace = @"\s(" + ccRegExStr + @")\s+(?:\(.+\))?\s*";
        // convert the code to xml
        private static readonly Regex getSetRegEx = new Regex(@"\s(get|set)\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex namespaceRegEx = new Regex(@"\snamespace\s+(\w+(?:\.\w+)*)\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex classRegEx = new Regex(@"\sclass\s+(\w+)(?:\s*\:\s*\w+(?:\s*,\s*\w+)*)?\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex methodEx = new Regex("(?:" + funcDefinationRegExStr + @"(?:\s*:\s*[^{]+\s*)?|this\s+\[[^\]]+\])\s*{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex controlRegEx = new Regex(controlRegExWithoutBrace + "{", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex controlRegEx2 = new Regex(controlRegExWithoutBrace + ".*?;", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex everyOtherThingRegEx = new Regex(@">[^<;]+<", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex attributeRegEx = new Regex(@"\s+(\w+|this\s*\[.+?\])\s*{(\s*(?:get|set)\s*(?:;|{))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex usingResourceRegEx = new Regex(@"", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex eatEverythingRegEx = new Regex(@"([^;>]*;[^;<]*)", RegexOptions.Compiled | RegexOptions.Multiline);
        private string toXML(string text) {
            string xml = text.Replace("<", "").Replace(">", "");    // delete <> symbol
            xml = xml.Replace(" try ", "").Replace(" catch ", "");   // delete try catch, as it can be mistaken as method
            xml = namespaceRegEx.Replace(xml, "<elem type=\"" + ELEM_NAMESPACE + "\" name=\"$1\">");
            xml = classRegEx.Replace(xml, "<elem type=\"" + ELEM_CLASS + "\" name=\"$1\">");
            xml = controlRegEx.Replace(xml, "<elem type=\"" + ELEM_CONTROL + "\" name=\"$1\" cc=\"1\">");
            xml = controlRegEx2.Replace(xml, "<elem type=\"" + ELEM_CONTROL + "\" name=\"$1\" cc=\"1\"/>");
            xml = methodEx.Replace(xml, "<elem type=\"" + ELEM_METHOD + "\" name=\"$1\">");
            xml = attributeRegEx.Replace(xml, "<elem type=\"" + ELEM_ATTRIBUTE + "\" name=\"$1\">$2");
            xml = getSetRegEx.Replace(xml, "<elem type=\"" + ELEM_ATTRIBUTE_BODY + "\" name=\"$1\">");
            xml = xml.Replace("{", "<elem type=\"" + ELEM_UNKNOWN + "\">");
            xml = xml.Replace("}", "</elem>");
            xml = "<elem type=\"" + ELEM_FILE + "\">" + xml + "</elem>";
            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><root>" + xml + "</root>";
            xml = eatEverythingRegEx.Replace(xml, "<elem type=\"" + ELEM_LINE_TERMINATOR + "\" />"); // do not eat delimiter ';' here, so that we can count function size
            xml = everyOtherThingRegEx.Replace(xml, "><");
            return xml;
        }

        // compute cyclomatic complexity
        private void computeCC(XElement xml) {
            // calculate cc
            accumulateCC(xml, ELEM_METHOD, ELEM_CONTROL);
            accumulateCC(xml, ELEM_CLASS, ELEM_METHOD);
            accumulateCC(xml, ELEM_NAMESPACE, ELEM_CLASS);
            accumulateCC(xml, ELEM_FILE, ELEM_NAMESPACE);
        }

        private void accumulateCC(XElement xml, string parentType, string childrenType) {
            IEnumerable<XElement> elems = from nd in xml.Descendants("elem")
                                          where nd.Attribute("type").Value == parentType
                                          select nd;
            foreach (XElement el in elems) {
                int cc = (from nd in el.Descendants("elem")
                          where nd.Attribute("type").Value == childrenType
                          select Int32.Parse(nd.Attribute("cc").Value)).Sum();
                if (childrenType == ELEM_CONTROL)
                    cc++;
                el.SetAttributeValue("cc", cc);
            }
            elems.OrderByDescending(el => Int32.Parse(el.Attribute("cc").Value));
        }

        private void removeAnonymousBlock(XElement xml) {
            // remove anonymous block which has no child
            (from nd in xml.Descendants("elem")
             where nd.Attribute("type").Value == ELEM_UNKNOWN && !nd.HasElements
             select nd).Remove();
        }

        private void removeLineTerminators(XElement xml) {
            (from nd in xml.Descendants("elem")
             where nd.Attribute("type").Value == ELEM_LINE_TERMINATOR
             select nd).Remove();
        }

        // calculate function size
        private void computeSize(XElement xml) {
            IEnumerable<XElement> nodes = from nd in xml.Descendants("elem")
                                          where nd.HasElements
                                          select nd;
            // the simplest way to know function size: count its children, including grandchildren
            // the node value does not include its own text value, but the text value of its children, which actually saves us time
            // but remember, do count the number of "controls", namely if, for, while, as they are of one size
            foreach (var n in nodes) {
                n.SetAttributeValue("size",
                    (from nd in n.Descendants("elem") select nd).Count()
                );
            }
        }
    }
}
