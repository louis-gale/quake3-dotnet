using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quake3.BotLib;
using Quake3.BotLib.IO;

namespace Quake3.Test.BotLib
{
    [TestClass]
    public class ScriptTests : ILog
    {
        [TestMethod]
        public void TestScriptTokenizer()
        {
            var parser = new BspParser("entdata", ScriptTests.GetScriptStream(), this);

            foreach (var entity in parser.Entities)
            {
                foreach (var pair in entity.Pairs)
                {
                    Debug.WriteLine(pair);
                }

                Debug.WriteLine(Environment.NewLine);
            }
        }

        private static Stream GetScriptStream()
        {
            return File.OpenRead(@"..\..\..\Quake3.Test\Data\Script.txt");
        }

        public void Error(string message)
        {
            Debug.WriteLine("ERROR: " + message);
        }

        public void Warning(string message)
        {
            Debug.WriteLine("WARNING: " + message);
        }
    }
}