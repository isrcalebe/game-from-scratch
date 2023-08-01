/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Globalization;

namespace miniblocks.API.Timing;

public struct FrameTimeData
{
    public double Elapsed { get; set; }

    public double Current { get; set; }

    public override readonly string ToString()
        => Math
            .Truncate(Current)
            .ToString(CultureInfo.InvariantCulture);
}
