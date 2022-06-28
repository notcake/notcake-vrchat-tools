using System;

namespace notcake.Unity.Yaml.Nodes
{
    public partial struct YamlNodeValidity
    {
        /// <summary>
        ///     Represents the validity of a YAML node in various contexts.
        /// </summary>
        [Flags]
        private enum Validity : byte
        {
            /// <summary>Indicates that the YAML node is never valid.</summary>
            None = 0x00,
            /// <summary>Indicates that the YAML node is valid at the document root.</summary>
            AtRoot = 0x01,
            /// <summary>Indicates that the YAML node is valid in a `block-out` context.</summary>
            InBlockOut = 0x02,
            /// <summary>Indicates that the YAML node is valid in a `block-in` context.</summary>
            InBlockIn = 0x04,
            /// <summary>Indicates that the YAML node is valid in a `flow-out` context.</summary>
            InFlowOut = 0x08,
            /// <summary>Indicates that the YAML node is valid in a `flow-in` context.</summary>
            InFlowIn = 0x10,
            /// <summary>Indicates that the YAML node is valid in a `flow-key` context.</summary>
            InFlowKey = 0x20,
        }
    }
}
