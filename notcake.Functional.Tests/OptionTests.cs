using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Functional.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Option{T}"/> struct.
    /// </summary>
    [TestClass]
    public class OptionTests
    {
        private struct A { }

        /// <summary>
        ///     Tests <see cref="Option{T}.Match{U}(Func{T, U}, Func{U})"/>.
        /// </summary>
        [TestMethod]
        public void MatchValue()
        {
            Option<A> some = new A();
            Option<A> none = Option<A>.None;

            Assert.AreEqual(1, some.Match(_ => 1, () => 2));
            Assert.AreEqual(2, none.Match(_ => 1, () => 2));
        }

        /// <summary>
        ///     Tests <see cref="Option{T}.Match(Action{T}, Action)"/>.
        /// </summary>
        [TestMethod]
        public void MatchNoValue()
        {
            Option<A> some = new A();
            Option<A> none = Option<A>.None;

            some.Match(
                _ => { },
                () => throw new InvalidOperationException()
            );
            none.Match(
                _ => throw new InvalidOperationException(),
                () => { }
            );
        }
    }
}
