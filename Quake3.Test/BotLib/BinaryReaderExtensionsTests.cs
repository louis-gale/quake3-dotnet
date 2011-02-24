using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quake3.BotLib.Extensions;

namespace Quake3.Test.BotLib
{
    [TestClass]
    public class BinaryReaderExtensionsTests
    {
        [TestMethod]
        public void TestTakeUntil()
        {
            string input1 = @"....ro.....r....robot....";
            string input2 = @"robo....ro.....r....robottt....";
            string input3 = @"robo....ro.....rr....robtt....robo";

            Debug.Assert(ReadInputUntil(input1) == 21);
            Debug.Assert(ReadInputUntil(input2) == 25);
            Debug.Assert(ReadInputUntil(input3) == 34);
        }

        [TestMethod]
        public void TestTakeWhile()
        {
            string input1 = @"qqqqqqqqqqqqqqqqqqqqq....";
            string input2 = @"qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq";
            string input3 = @".qqqqqqqqqqqqqqqqqqqqqqq";
            string input4 = @"qqqqqqqqqqqqq.qqqqqqqqq";

            Debug.Assert(ReadInputWhile(input1) == 21);
            Debug.Assert(ReadInputWhile(input2) == 31);
            Debug.Assert(ReadInputWhile(input3) == 0);
            Debug.Assert(ReadInputWhile(input4) == 13);
        }

        private int ReadInputUntil(string input)
        {
            using (var reader = new BinaryReader(new MemoryStream(Encoding.ASCII.GetBytes(input))))
            {
                int count = reader.TakeUntil("robot").Count();
                return count;
            }
        }

        private int ReadInputWhile(string input)
        {
            using (var reader = new BinaryReader(new MemoryStream(Encoding.ASCII.GetBytes(input))))
            {
                int count = reader.TakeWhile(c => c == 'q').Count();
                return count;
            }
        }
    }
}
