using System;
using System.Runtime.CompilerServices;

namespace notcake.Functional
{
    /// <summary>
    ///     Represents a value with one of two possibilities.
    /// </summary>
    /// <typeparam name="L">The type of the <see cref="Either.Side.Left"/> possibility.</typeparam>
    /// <typeparam name="R">The type of the <see cref="Either.Side.Right"/> possibility.</typeparam>
    public readonly struct Either<L, R>
    {
        private readonly Either.Side side;
        private readonly L? left;
        private readonly R? right;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Either{L, R}"/> struct.
        /// </summary>
        /// <param name="left">The value of the <see cref="Either.Side.Left"/> possibility.</param>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either(L left)
        {
            this.side  = Either.Side.Left;
            this.left  = left;
            this.right = default;
        }

        /// <param name="right">
        ///     The value of the <see cref="Either.Side.Right"/> possibility.
        /// </param>
        /// <inheritdoc cref="Either{L, R}.Either(L)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either(R right)
        {
            this.side  = Either.Side.Right;
            this.left  = default;
            this.right = right;
        }

        /// <summary>
        ///     Gets an <see cref="Either.Side"/> representing the possibility which the
        ///     <see cref="Either{L, R}"/> contains.
        /// </summary>
        public Either.Side Side => this.side;

        /// <summary>
        ///     Gets the value of the <see cref="Either.Side.Left"/> possibility.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <see cref="Either{L, R}"/> is not a <see cref="Either.Side.Left"/>
        ///     possibility.
        /// </exception>
        public L Left
        {
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            get
            {
                if (this.side != Either.Side.Left) { throw new InvalidOperationException(); }

                return this.left!;
            }
        }

        /// <summary>
        ///     Gets the value of the <see cref="Either.Side.Left"/> possibility.
        /// </summary>
        public L? LeftOrDefault
        {
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            get
            {
                if (this.side != Either.Side.Left) { return default; }

                return this.left!;
            }
        }

        /// <summary>
        ///     Gets the value of the <see cref="Either.Side.Right"/> possibility.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <see cref="Either{L, R}"/> is not a <see cref="Either.Side.Right"/>
        ///     possibility.
        /// </exception>
        public R Right
        {
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            get
            {
                if (this.side != Either.Side.Right) { throw new InvalidOperationException(); }

                return this.right!;
            }
        }

        /// <summary>
        ///     Gets the value of the <see cref="Either.Side.Right"/> possibility.
        /// </summary>
        public R? RightOrDefault
        {
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            get
            {
                if (this.side != Either.Side.Right) { return default; }

                return this.right!;
            }
        }

        /// <summary>
        ///     Applies the given function to the <see cref="Left"/> possibility.
        /// </summary>
        /// <typeparam name="L2">
        ///     The new type of the <see cref="Left"/> possibility.
        /// </typeparam>
        /// <param name="f">The function to apply to the <see cref="Left"/> possibility.</param>
        /// <returns>
        ///     The <see cref="Either{L, R}"/> after <paramref name="f"/> has been applied to
        ///     <see cref="Left"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either<L2, R> FlatMap<L2>(Func<L, Either<L2, R>> f)
        {
            return this.side switch
            {
                Either.Side.Left  => f(this.left!),
                Either.Side.Right => new Either<L2, R>(this.right!),
                _                 => throw new InvalidOperationException(),
            };
        }

        /// <summary>
        ///     Applies the given function to the <see cref="Right"/> possibility.
        /// </summary>
        /// <typeparam name="R2">
        ///     The new type of the <see cref="Right"/> possibility.
        /// </typeparam>
        /// <param name="f">The function to apply to the <see cref="Right"/> possibility.</param>
        /// <returns>
        ///     The <see cref="Either{L, R}"/> after <paramref name="f"/> has been applied to
        ///     <see cref="Right"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either<L, R2> FlatMap<R2>(Func<R, Either<L, R2>> f)
        {
            return this.side switch
            {
                Either.Side.Left  => new Either<L, R2>(this.left!),
                Either.Side.Right => f(this.right!),
                _                 => throw new InvalidOperationException(),
            };
        }

        /// <inheritdoc cref="FlatMap{L2}(Func{L, Either{L2, R}})"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either<L2, R> Map<L2>(Func<L, L2> f)
        {
            return this.side switch
            {
                Either.Side.Left  => new Either<L2, R>(f(this.left!)),
                Either.Side.Right => new Either<L2, R>(this.right!),
                _                 => throw new InvalidOperationException(),
            };
        }

        /// <inheritdoc cref="FlatMap{R2}(Func{R, Either{L, R2}})"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Either<L, R2> Map<R2>(Func<R, R2> f)
        {
            return this.side switch
            {
                Either.Side.Left  => new Either<L, R2>(this.left!),
                Either.Side.Right => new Either<L, R2>(f(this.right!)),
                _                 => throw new InvalidOperationException(),
            };
        }

        /// <summary>
        ///     Applies the given functions to the <see cref="Left"/> and <see cref="Right"/>
        ///     possibilities.
        /// </summary>
        /// <param name="left">The function to apply to the <see cref="Left"/> possibility.</param>
        /// <param name="right">
        ///     The function to apply to the <see cref="Right"/> possibility.
        /// </param>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public void Match(Action<L> left, Action<R> right)
        {
            switch (this.side)
            {
                case Either.Side.Left:  left(this.left!);   return;
                case Either.Side.Right: right(this.right!); return;
            }
        }

        /// <typeparam name="T">
        ///     The type into which to transform the <see cref="Either{L, R}"/>.
        /// </typeparam>
        /// <returns>
        ///     The result of <paramref name="left"/> or <paramref name="right"/> applied to the
        ///     <see cref="Left"/> or <see cref="Right"/> possibility respectively.
        /// </returns>
        /// <inheritdoc cref="Match(Action{L}, Action{R})"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public T Match<T>(Func<L, T> left, Func<R, T> right)
        {
            return this.side switch
            {
                Either.Side.Left  => left(this.left!),
                Either.Side.Right => right(this.right!),
                _                 => throw new InvalidOperationException(),
            };
        }

        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static implicit operator Either<L, R>(L left)
        {
            return new Either<L, R>(left);
        }

        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static implicit operator Either<L, R>(R right)
        {
            return new Either<L, R>(right);
        }
    }
}
