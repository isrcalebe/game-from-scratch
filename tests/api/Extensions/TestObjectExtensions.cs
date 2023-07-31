/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Tests.Extensions;

#nullable enable

using NUnit.Framework;
using miniblocks.API.Extensions.ObjectExtensions;
using System;

[TestFixture]
public class ObjectExtensionsTests
{
    [Test]
    public void AsNonNull_WithNonNullTarget_ReturnsTarget()
    {
        var target = "hello";
        var result = target.AsNonNull();

        Assert.AreEqual(target, result);
    }

    [Test]
    public void AsNonNull_WithNullTarget_ThrowsNullReferenceException()
    {
        string? target = null;
        Assert.Throws<NullReferenceException>(() => target.AsNonNull());
    }

    [Test]
    public void IsNull_WithNullTarget_ReturnsTrue()
    {
        string? target = null;
        var result = target.IsNull();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsNull_WithNonNullTarget_ReturnsFalse()
    {
        var target = "hello";
        var result = target.IsNull();

        Assert.IsFalse(result);
    }

    [Test]
    public void IsNotNull_WithNonNullTarget_ReturnsTrue()
    {
        var target = "hello";
        var result = target.IsNotNull();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsNotNull_WithNullTarget_ReturnsFalse()
    {
        string? target = null;
        var result = target.IsNotNull();

        Assert.IsFalse(result);
    }
}
