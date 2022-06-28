using System;
using System.Runtime.CompilerServices;

namespace notcake.Functional
{
    /// <summary>
    ///     Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the value in the <see cref="Option{T}"/>.</typeparam>
    public readonly struct Option<T>
    {
        private readonly bool hasValue;
        private readonly T value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Option{T}"/> struct.
        /// </summary>
        /// <param name="value">The value in the <see cref="Option{T}"/>.</param>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Option(T value)
        {
            this.hasValue = true;
            this.value    = value;
        }

        /// <summary>
        ///     Gets a boolean indicating whether the <see cref="Option{T}"/> contains a value.
        /// </summary>
        public bool HasValue => this.hasValue;

        /// <summary>
        ///     Gets the value in the <see cref="Option{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the <see cref="Option{T}"/> does not contain a value.
        /// </exception>
        public T Value
        {
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            get
            {
                if (!this.hasValue) { throw new InvalidOperationException(); }

                return this.value;
            }
        }

        /// <summary>
        ///     Applies the given function to the value, if present.
        /// </summary>
        /// <typeparam name="U">
        ///     The new type of the value in the <see cref="Option{T}"/>.
        /// </typeparam>
        /// <param name="f">The function to apply to the value, if present.</param>
        /// <returns>
        ///     A new <see cref="Option{T}"/> containing the result of <paramref name="f"/> applied
        ///     to the value.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Option<U> FlatMap<U>(Func<T, Option<U>> f)
        {
            return this.hasValue ?
                f(this.value) :
                Option<U>.None;
        }

        /// <inheritdoc cref="FlatMap{U}(Func{T, Option{U}})"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public Option<U> Map<U>(Func<T, U> f)
        {
            return this.hasValue ?
                Option<U>.Some(f(this.value)) :
                Option<U>.None;
        }

        /// <summary>
        ///     Applies the given functions to the <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="some">The function to apply to the value, if present.</param>
        /// <param name="none">The function to apply if no value is present.</param>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public void Match(Action<T> some, Action none)
        {
            if (this.hasValue)
            {
                some(this.value);
            }
            else
            {
                none();
            }
        }

        /// <typeparam name="U">The new type of the value in the <see cref="Option{T}"/></typeparam>
        /// <returns>
        ///     The result of <paramref name="some"/> applied to the value, if present;<br/>
        ///     the result of <paramref name="none"/> applied otherwise.
        /// </returns>
        /// <inheritdoc cref="Match(Action{T}, Action)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public U Match<U>(Func<T, U> some, Func<U> none)
        {
            return this.hasValue ?
                some(this.value) :
                none();
        }

        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static implicit operator Option<T>(T value)
        {
            return Option<T>.Some(value);
        }

        public static implicit operator T?(Option<T> option)
        {
            return option.hasValue ?
                option.value :
                default;
        }

        /// <summary>
        ///     Creates a new <see cref="Option{T}"/> containing no value.
        /// </summary>
        public static Option<T> None => new();

        /// <summary>
        ///     Creates a new <see cref="Option{T}"/> containing the given value.
        /// </summary>
        /// <returns>An <see cref="Option{T}"/> containing the given value.</returns>
        /// <inheritdoc cref="Option(T)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }
    }
}
