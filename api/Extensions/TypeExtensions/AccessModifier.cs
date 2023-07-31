/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Extensions.TypeExtensions;

[Flags]
public enum AccessModifier
{
    None = 0,
    Public = 1,
    Internal = 1 << 1,
    Protected = 1 << 2,
    Private = 1 << 3,
}
