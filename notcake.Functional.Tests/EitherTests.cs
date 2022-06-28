using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Functional.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Either{L, R}"/> struct.
    /// </summary>
    [TestClass]
    public class EitherTests
    {
        private struct A { }
        private struct B { }

        /// <summary>
        ///     Tests <see cref="Either{L, R}.Match{T}(Func{L, T}, Func{R, T})"/>.
        /// </summary>
        [TestMethod]
        public void MatchValue()
        {
            Either<A, B> eitherA = new A();
            Either<A, B> eitherB = new B();

            Assert.AreEqual(1, eitherA.Match(_ => 1, _ => 2));
            Assert.AreEqual(2, eitherB.Match(_ => 1, _ => 2));
        }

        /// <summary>
        ///     Tests <see cref="Either{L, R}.Match(Action{L}, Action{R})"/>.
        /// </summary>
        [TestMethod]
        public void MatchNoValue()
        {
            Either<A, B> eitherA = new A();
            Either<A, B> eitherB = new B();

            eitherA.Match(
                _ => { },
                _ => throw new InvalidOperationException()
            );
            eitherB.Match(
                _ => throw new InvalidOperationException(),
                _ => { }
            );
        }
    }
}
