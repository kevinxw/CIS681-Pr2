/*
 * Calculate "File distance"
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kevin.CIS681.Project.CodeAnalyzer.Task;
using System.Xml.Linq;
using Kevin.CIS681.Project.CodeAnalyzer.IO;
using System.Diagnostics;
using System.Threading;
using Kevin.CIS681.Project.CodeAnalyzer.Parser;
using System.Collections;

namespace Kevin.CIS681.Project.CodeAnalyzer {
    class FileDistanceTask : ITask {
        private static Dictionary<string, XElement> data = new Dictionary<string, XElement>();

        public void start(object state) {
            TaskData td = state as TaskData;
            string[] filePath = td.state as string[];
            string[] lowFilePath = new string[2];
            XElement[] xElem = new XElement[2] { null, null };
            // calculate executing time
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            // load XML report
            lock (data) {
                for (int i = 0; i < 2; i++) {
                    lowFilePath[i] = filePath[i].ToLower();
                    if (data.ContainsKey(lowFilePath[i]))
                        xElem[i] = data[lowFilePath[i]];
                }
            }
            for (int i = 0; i < 2; i++)
                if (xElem[i] == null) {
                    try {
                        xElem[i] = XElement.Load(filePath[i] + ".ca.xml");
                        lock (data) {
                            data[lowFilePath[i]] = xElem[i];
                        }
                    }
                    catch (Exception e) {
                        Logger.error(e.Message);
                        return;
                    }
                }
            Logger.debug("\nNow computing the following file distance:\n{0}\n{1}", filePath[0], filePath[1]);
            // use LINQ!!, do I really have to sort the two collection 1st? I don't think so.
            /*
            var matchElems = from e1 in xElem[0].Descendants("elem")
                             where e1.Attribute("type").Value == SimpleParser.ELEM_METHOD
                             && e1.Attribute("cc") != null
                             join e2 in xElem[1].Descendants("elem")
                             on new {
                                 type = e1.Attribute("type").Value,
                                 cc = e1.Attribute("cc").Value
                             }
                             equals new {
                                 type = e2.Attribute("type").Value,
                                 cc = (e2.Attribute("cc") == null ? null : e2.Attribute("cc").Value)
                             }
                             orderby e1.Attribute("cc").Value descending, e1.Attribute("name").Value
                             select new {
                                 left = e1,
                                 right = e2
                             };
             */
            // OK.. give up.. this query will get all possible combination where cc value is the same
            // get each list then sort
            var left = (from e in xElem[0].Descendants("elem")
                        where e.Attribute("type").Value == SimpleParser.ELEM_METHOD
                              && e.Attribute("cc") != null
                              && e.Parent.Attribute("name") != null
                              && e.Attribute("name") != null
                        let cc = Int32.Parse(e.Attribute("cc").Value)
                        let name = e.Parent.Attribute("name").Value + "." + e.Attribute("name").Value
                        orderby cc descending
                        select new {
                            name,
                            cc
                        }).ToList();
            // how to write a function of which the return type is "var"?
            var right = (from e in xElem[1].Descendants("elem")
                         where e.Attribute("type").Value == SimpleParser.ELEM_METHOD
                               && e.Attribute("cc") != null
                               && e.Parent.Attribute("name") != null
                               && e.Attribute("name") != null
                         let cc = Int32.Parse(e.Attribute("cc").Value)
                         let name = e.Parent.Attribute("name").Value + "." + e.Attribute("name").Value
                         orderby cc descending
                         select new {
                             name,
                             cc
                         }).ToList();
            int index = 0;
            while (Math.Min(left.Count(), right.Count()) > index) {
                if (left[index].cc > right[index].cc)
                    left.RemoveAt(index);   // strike out the bigger value
                else if (left[index].cc < right[index].cc)
                    right.RemoveAt(index);
                else
                    // cc are equal
                    index++;
            }
            // notice the tail of the longer array is not truncated
            // get match list
            StringBuilder sb = new StringBuilder();
            int len = Math.Min(left.Count(), right.Count());
            for (int i = 0; i < len; i++)
                sb.AppendFormat("\nCC[{2}] {0} <-> {1}", left[i].name, right[i].name, left[i].cc);
            // select all function
            var allFuncs = from e1 in xElem[0].Descendants("elem")
                           where e1.Attribute("type").Value == SimpleParser.ELEM_METHOD
                           join e2 in xElem[1].Descendants("elem")
                           on e1.Attribute("type").Value equals e2.Attribute("type").Value
                           into eSet
                           from e3 in eSet.DefaultIfEmpty()
                           select e3;
            double allFuncCount = allFuncs.Count();

            // must output information in one statement, or multi-thread will break the contents
            Logger.info("The distance between file {0} and {1} is {2:F2}. {3} match functions are found (total {4}). {5}",
                filePath[0],
                filePath[1],
                allFuncCount == 0 ? 1 : 1 - (2 * len) / allFuncCount,
                len, allFuncCount,
                sb.ToString());
            // finished
            td.resetEvent.Set();
        }
    }
}
