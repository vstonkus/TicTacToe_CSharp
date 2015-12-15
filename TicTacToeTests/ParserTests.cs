using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FsCheck;

namespace TicTacToe.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void EncodeTest()
        {
            Assert.AreEqual("le", Parser.Encode(new Action[] { }));
            Assert.AreEqual("ld1:v1:x1:xi1e1:yi1eee", Parser.Encode(new [] {new Action() {letter = 'x', x = 1, y = 1} }));
        }

        [TestMethod()]
        public void DecodeTest()
        {
            Assert.AreEqual(1, Parser.Decode("ld1:v1:x1:xi1e1:yi1eee").Select(x =>
            {
                Assert.AreEqual('x', x.letter);
                Assert.AreEqual(1, x.y);
                Assert.AreEqual(1, x.x);
                return x;
            }).Count());
            Assert.AreEqual(0, Parser.Decode("le").Count());
        }

        [TestMethod()]
        public void DecodeTestFsCheck()
            {
            Prop.ForAll<char, int, int>((letter, x, y) =>
                {
                Assert.AreEqual(1, Parser.Decode(Parser.Encode(new[] {new Action() {letter = letter, x = x, y = y} } )).Count());
                }).QuickCheck();
            }
    }
}