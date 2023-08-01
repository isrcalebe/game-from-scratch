/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using miniblocks.API.Proxies;
using NUnit.Framework;

namespace miniblocks.API.Tests.Proxies;

[TestFixture]
public class TestProxy
{
    [Test]
    public void Ctor_ValueUsedAsDefaultValue()
    {
        Assert.That(new Proxy<int>(10).Default, Is.EqualTo(10));
    }

    [Test]
    public void Ctor_ValueUsedAsInitialValue()
    {
        Assert.That(new Proxy<int>(10).Value, Is.EqualTo(10));
    }

    [Test]
    public void Connection_ConnectViaConnection()
    {
        var parentProxy = new Proxy<int>();

        var proxy1 = new Proxy<int>();
        IProxy<int> proxy2 = new Proxy<int>();
        IProxy proxy3 = new Proxy<int>();

        proxy1.Connection = parentProxy;
        proxy2.Connection = parentProxy;
        proxy3.Connection = parentProxy;

        parentProxy.Value = 5;
        parentProxy.Disabled = true;

        Assert.That(proxy1.Value, Is.EqualTo(5));
        Assert.That(proxy2.Value, Is.EqualTo(5));
        Assert.That(proxy3.Disabled, Is.True);
    }
}
