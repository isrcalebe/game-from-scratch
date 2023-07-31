/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Collections.Generic;
using System.Linq;
using miniblocks.API.Collections;
using NUnit.Framework;

namespace miniblocks.API.Tests.Collections;

[TestFixture]
public class TestWeakList
{
    [Test]
    public void Add_AddsItemToList()
    {
        var item = new object();
        var list = new WeakList<object> { item };

        Assert.That(list.Count(), Is.EqualTo(1));
        Assert.That(list, Does.Contain(item));

        GC.KeepAlive(item);
    }

    [Test]
    public void Add_AddsWeakReferenceToList()
    {
        var item = new object();
        var reference = new WeakReference<object>(item);
        var list = new WeakList<object> { reference };

        Assert.That(list.Contains(reference), Is.True);

        GC.KeepAlive(item);
    }

    [Test]
    public void Count_Enumerate()
    {
        var item = new object();
        var list = new WeakList<object> { item };

        Assert.That(list.Count(), Is.EqualTo(1));
        Assert.That(list, Does.Contain(item));

        GC.KeepAlive(item);
    }

    [Test]
    public void Remove_RemovesItemFromList()
    {
        var item = new object();
        var list = new WeakList<object> { item };

        list.Remove(item);

        Assert.That(list.Count(), Is.Zero);
        Assert.That(list, Does.Not.Contain(item));

        GC.KeepAlive(item);
    }

    [Test]
    public void Remove_RemovesWeakReferenceFromList()
    {
        var item = new object();
        var weakRef = new WeakReference<object>(item);
        var list = new WeakList<object> { weakRef };

        list.Remove(weakRef);

        Assert.That(list.Count(), Is.Zero);
        Assert.That(list.Contains(weakRef), Is.False);

        GC.KeepAlive(item);
    }

    [Test]
    public void Remove_RemovesObjectAtSide()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (object o in items)
            list.Add(o);

        list.Remove(items[0]);
        list.Remove(items[1]);
        list.Remove(items[4]);
        list.Remove(items[5]);

        Assert.That(list.Count(), Is.EqualTo(2));
        Assert.That(list, Does.Contain(items[2]));
        Assert.That(list, Does.Contain(items[3]));
    }

    [Test]
    public void Remove_RemovesObjectsAtCentre()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (object o in items)
            list.Add(o);

        list.Remove(items[2]);
        list.Remove(items[3]);

        Assert.That(list.Count(), Is.EqualTo(4));
        Assert.That(list, Does.Contain(items[0]));
        Assert.That(list, Does.Contain(items[1]));
        Assert.That(list, Does.Contain(items[4]));
        Assert.That(list, Does.Contain(items[5]));
    }

    [Test]
    public void Add_AddsAfterRemoveFromEnd()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
        };
        var newLastItem = new object();
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        list.Remove(items[2]);
        list.Add(newLastItem);

        Assert.That(list.Count(), Is.EqualTo(3));
        Assert.That(list, Does.Contain(items[0]));
        Assert.That(list, Does.Contain(items[0]));
        Assert.That(list, Does.Not.Contain(items[2]));
        Assert.That(list, Does.Contain(newLastItem));
    }

    [Test]
    public void Remove_RemovesAllUsingRemoveAtFromStart()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        for (int i = 0; i < items.Count(); i++)
            list.RemoveAt(0);

        Assert.That(list.Count(), Is.Zero);
    }

    [Test]
    public void Remove_RemovesAllUsingRemoveAtFromEnd()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        for (int i = 0; i < items.Count(); i++)
            list.RemoveAt(list.Count() - 1);

        Assert.That(list.Count(), Is.Zero);
    }
    [Test]
    public void Remove_RemovesAllUsingRemoveAtFromBothSides()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        for (int i = 0; i < items.Count(); i++)
        {
            if (i % 2 == 0)
                list.RemoveAt(0);
            else
                list.RemoveAt(list.Count() - 1);
        }

        Assert.That(list.Count(), Is.Zero);
    }
    [Test]
    public void Contains_CountIsZeroAfterClear()
    {
        var item = new object();
        var reference = new WeakReference<object>(item);
        var list = new WeakList<object> { item, reference };

        list.Clear();

        Assert.That(list.Count(), Is.Zero);
        Assert.That(list, Does.Not.Contain(item));
        Assert.That(list.Contains(reference), Is.False);

        GC.KeepAlive(item);
    }

    [Test]
    public void Clear_AddsAfterClear()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
        };
        var newItem = new object();
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        list.Clear();
        list.Add(newItem);

        Assert.That(list.Count(), Is.EqualTo(1));
        Assert.That(list, Does.Contain(newItem));
    }

    [Test]
    public void Remove_IterateWithRemove()
    {
        var item1 = new object();
        var item2 = new object();
        var item3 = new object();
        var list = new WeakList<object> { item1, item2, item3 };
        var count = 0;

        foreach (var item in list)
        {
            if (count == 1)
                list.Remove(item);
            count++;
        }

        Assert.That(count, Is.EqualTo(3));
        Assert.That(list, Does.Contain(item1));
        Assert.That(list, Does.Not.Contain(item2));
        Assert.That(list, Does.Contain(item3));

        GC.KeepAlive(item1);
        GC.KeepAlive(item2);
        GC.KeepAlive(item3);
    }

    [Test]
    public void Remove_IterateWithRemoveSkipsInvalidated()
    {
        var item1 = new object();
        var item2 = new object();
        var item3 = new object();
        var list = new WeakList<object> { item1, item2, item3 };
        var count = 0;

        foreach (var item in list)
        {
            if (count == 0)
                list.Remove(item2);
            Assert.That(item, Is.Not.EqualTo(item2));
            count++;
        }

        Assert.That(count, Is.EqualTo(2));

        GC.KeepAlive(item1);
        GC.KeepAlive(item2);
        GC.KeepAlive(item3);
    }

    [Test]
    public void Contains_DeadObjectsAreSkippedBeforeEnumeration()
    {
        GC.TryStartNoGCRegion(10 * 1000000);

        (var list, object[] aliveItems) = generateWeakList();

        GC.EndNoGCRegion();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        foreach (var item in list)
        {
            if (!aliveItems.Contains(item))
                Assert.Fail("Dead objects were iterated over.");
        }
    }

    [Test]
    public void Contains_DeadObjectsAreSkippedDuringEnumeration()
    {
        GC.TryStartNoGCRegion(10 * 1000000);

        (var list, object[] aliveItems) = generateWeakList();

        using (var enumerator = list.GetEnumerator())
        {
            GC.EndNoGCRegion();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            while (enumerator.MoveNext())
            {
                if (!aliveItems.Contains(enumerator.Current))
                    Assert.Fail("Dead objects were iterated over.");
            }
        }
    }

    private static (WeakList<object> list, object[] aliveObjects) generateWeakList()
    {
        var items = new List<object>
        {
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
            new object(),
        };
        var list = new WeakList<object>();

        foreach (var item in items)
            list.Add(item);

        return (list, new[] { items[1], items[2], items[4] });
    }
}
