using System;
using System.Runtime.CompilerServices;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents the validity of a YAML node in various contexts.
    /// </summary>
    public partial struct YamlNodeValidity
    {
        /// <summary>
        ///     The <see cref="Validity"/> of the YAML node.
        /// </summary>
        private readonly Validity validity;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid somewhere.
        /// </summary>
        public bool Somewhere => this.validity != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid at the document root.
        /// </summary>
        public bool AtRoot => (this.validity & Validity.AtRoot) != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid in the `block-out` context.
        /// </summary>
        public bool InBlockOut => (this.validity & Validity.InBlockOut) != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid in the `block-in` context.
        /// </summary>
        public bool InBlockIn => (this.validity & Validity.InBlockIn) != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid in the `flow-out` context.
        /// </summary>
        public bool InFlowOut => (this.validity & Validity.InFlowOut) != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid in the `flow-in` context.
        /// </summary>
        public bool InFlowIn => (this.validity & Validity.InFlowIn) != Validity.None;

        /// <summary>
        ///     Gets a boolean indicating whether the YAML node is valid in the `flow-key` context.
        /// </summary>
        public bool InFlowKey => (this.validity & Validity.InFlowKey) != Validity.None;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlNodeValidity"/> struct.
        /// </summary>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public YamlNodeValidity()
        {
            this.validity = Validity.None;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlNodeValidity"/> struct.
        /// </summary>
        /// <param name="validity">The <see cref="Validity"/> of the YAML node.</param>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        private YamlNodeValidity(Validity validity)
        {
            this.validity = validity;
        }

        #region Object
        public override string ToString()
        {
            return this.validity.ToString();
        }
        #endregion

        #region YamlNodeValidity
        #endregion

        /// <summary>
        ///     Gets a <see cref="YamlNodeValidity"/> indicating that the YAML node is never valid.
        /// </summary>
        public static YamlNodeValidity None => new(Validity.None);

        /// <summary>
        ///     Creates a <see cref="YamlNodeValidity"/> indicating that the YAML node is always
        ///     valid in block contexts.
        /// </summary>
        /// <returns>
        ///     A <see cref="YamlNodeValidity"/> indicating that the YAML node is always valid in
        ///     block contexts.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static YamlNodeValidity Block()
        {
            return new YamlNodeValidity(
                Validity.AtRoot |
                Validity.InBlockOut |
                Validity.InBlockIn
            );
        }

        /// <summary>
        ///     Creates a <see cref="YamlNodeValidity"/> indicating that the YAML node is sometimes
        ///     valid in block contexts.
        /// </summary>
        /// <param name="atRoot">
        ///     A boolean indicating whether the YAML node is valid at the document root.
        /// </param>
        /// <param name="inBlockOut">
        ///     A boolean indicating whether the YAML node is valid in the `block-out` context.
        /// </param>
        /// <param name="inBlockIn">
        ///     A boolean indicating whether the YAML node is valid in the `block-in` context.
        /// </param>
        /// <returns></returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static YamlNodeValidity Block(bool atRoot, bool inBlockOut, bool inBlockIn)
        {
            Validity validity = Validity.None;
            if (atRoot    ) { validity |= Validity.AtRoot;     }
            if (inBlockOut) { validity |= Validity.InBlockOut; }
            if (inBlockIn ) { validity |= Validity.InBlockIn;  }
            return new YamlNodeValidity(validity);
        }

        /// <summary>
        ///     Creates a <see cref="YamlNodeValidity"/> indicating that the YAML node is always
        ///     valid in flow contexts.
        /// </summary>
        /// <returns>
        ///     A <see cref="YamlNodeValidity"/> indicating that the YAML node is always valid in
        ///     flow contexts.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static YamlNodeValidity Flow()
        {
            return new YamlNodeValidity(
               Validity.AtRoot |
               Validity.InFlowOut |
               Validity.InFlowIn |
               Validity.InFlowKey
            );
        }

        /// <summary>
        ///     Creates a <see cref="YamlNodeValidity"/> indicating that the YAML node is sometimes
        ///     valid in flow contexts.
        /// </summary>
        /// <param name="atRoot">
        ///     A boolean indicating whether the YAML node is valid at the document root.
        /// </param>
        /// <param name="inFlowOut">
        ///     A boolean indicating whether the YAML node is valid in the `flow-out` context.
        /// </param>
        /// <param name="inFlowIn">
        ///     A boolean indicating whether the YAML node is valid in the `flow-in` context.
        /// </param>
        /// <param name="inFlowKey">
        ///     A boolean indicating whether the YAML node is valid in the `flow-key` context.
        /// </param>
        /// <returns>
        ///     A <see cref="YamlNodeValidity"/> indicating that the YAML node is sometimes valid in
        ///     flow contexts.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        public static YamlNodeValidity Flow(
            bool atRoot,
            bool inFlowOut,
            bool inFlowIn,
            bool inFlowKey
        )
        {
            Validity validity = Validity.None;
            if (atRoot   ) { validity |= Validity.AtRoot;    }
            if (inFlowOut) { validity |= Validity.InFlowOut; }
            if (inFlowIn ) { validity |= Validity.InFlowIn;  }
            if (inFlowKey) { validity |= Validity.InFlowKey; }
            return new YamlNodeValidity(validity);
        }
    }
}
