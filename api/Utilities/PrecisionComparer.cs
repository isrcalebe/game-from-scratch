/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Utilities;

public static class PrecisionComparer
{
    public const float FLOAT_EPSILON = 1e-3f;
    public const double DOUBLE_EPSILON = 1e-7;

    public static bool DefinitelyBigger(float lhs, float rhs, float acceptableDifference = FLOAT_EPSILON)
        => lhs - acceptableDifference > rhs;

    public static bool DefinitelyBigger(double lhs, double rhs, double acceptableDifference = DOUBLE_EPSILON)
        => lhs - acceptableDifference > rhs;

    public static bool AlmostBigger(float lhs, float rhs, float acceptableDifference = FLOAT_EPSILON)
        => lhs > rhs - acceptableDifference;

    public static bool AlmostBigger(double lhs, double rhs, double acceptableDifference = DOUBLE_EPSILON)
        => lhs > rhs - acceptableDifference;

    public static bool AlmostEquals(float lhs, float rhs, float acceptableDifference = FLOAT_EPSILON)
        => Math.Abs(lhs - rhs) <= acceptableDifference;

    public static bool AlmostEquals(double lhs, double rhs, double acceptableDifference = DOUBLE_EPSILON)
        => Math.Abs(lhs - rhs) <= acceptableDifference;
}
