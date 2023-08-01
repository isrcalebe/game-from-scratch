/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
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

    [TestCaseSource(nameof(getParsingConversionTests))]
    public void Parse_ParsingConversions(Type type, object input, object output)
    {
        var bindable = Activator.CreateInstance(
            typeof(Proxy<>).MakeGenericType(type),
            type == typeof(string)
            ? ""
            : Activator.CreateInstance(type)
        );

        Debug.Assert(bindable != null);

        ((ICanBeParsed)bindable).Parse(input);

        var value = bindable
            .GetType()
            .GetProperty(nameof(Proxy<object>.Value), BindingFlags.Public | BindingFlags.Instance)?
            .GetValue(bindable);

        Assert.That(value, Is.EqualTo(output));
    }

    [Test]
    public void Parse_NullIntoValueType()
    {
        var proxy = new Proxy<int>();

        Assert.That(()
            => proxy.Parse(null), Throws.ArgumentNullException);
    }

    [Test]
    public void Parse_EmptyStringIntoValueType()
    {
        var proxy = new Proxy<int>();

        Assert.Throws<FormatException>(()
            => proxy.Parse(string.Empty));
    }

    [Test]
    public void Parse_NullIntoNullableValueType()
    {
        var proxy = new Proxy<int?>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoNullableValueType()
    {
        var proxy = new Proxy<int?>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_NullIntoReferenceType()
    {
        var proxy = new Proxy<DummyClass>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoReferenceType()
    {
        var proxy = new Proxy<DummyClass>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_NullIntoReferenceWithNRT()
    {
        var proxy = new Proxy<DummyClass>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoReferenceWithNRT()
    {
        var proxy = new Proxy<DummyClass>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Null);
    }

#nullable enable
    [Test]
    public void Parse_NullIntoNullableReferenceTypeWithNRT()
    {
        var proxy = new Proxy<DummyClass?>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoNullableReferenceTypeWithNRT()
    {
        var proxy = new Proxy<DummyClass?>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Null);
    }
#nullable disable

    [Test]
    public void Parse_NullIntoEmptyStringType()
    {
        var proxy = new Proxy<string>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoStringType()
    {
        var proxy = new Proxy<string>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Empty);
    }

#nullable enable
    [Test]
    public void Parse_NullIntoNullableStringTypeWithNRT()
    {
        var proxy = new Proxy<string?>();
        proxy.Parse(null);

        Assert.That(proxy.Value, Is.Null);
    }

    [Test]
    public void Parse_EmptyStringIntoNullableStringTypeWithNRT()
    {
        var proxy = new Proxy<string?>();
        proxy.Parse(string.Empty);

        Assert.That(proxy.Value, Is.Empty);
    }
#nullable disable

    private static IEnumerable<object[]> getParsingConversionTests()
    {
        var testTypes = new[]
        {
            typeof(bool),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(short),
            typeof(ushort),
            typeof(byte),
            typeof(sbyte),
            typeof(decimal),
            typeof(string)
        };

        var inputs = new object[]
        {
            1, "1", 1.0, 1.0f, 1L, 1m,
            1.5, "1.5", 1.5f, 1.5m,
            -1, "-1", -1.0, -1.0f, -1L, -1m,
            -1.5, "-1.5", -1.5f, -1.5m,
        };

        foreach (var type in testTypes)
        {
            foreach (var input in inputs)
            {
                object expectedOutput = null;

                try
                {
                    expectedOutput = Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
                }
                catch
                {
                }

                if (expectedOutput != null)
                    yield return new object[] { type, input, expectedOutput };
            }
        }
    }

    private class DummyClass
    {
    }
}
