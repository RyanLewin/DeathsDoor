using System.Collections;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int itemCount;
    public int Count { get => itemCount; }

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = itemCount;
        items[itemCount] = item;
        SortUp(item);
        itemCount++;
        if (itemCount == items.Length)
            return;
    }

    public T RemoveFirst ()
    {
        T firstItem = items[0];
        itemCount--;
        if (itemCount <= 0)
        {
            items = new T[items.Length];
        }
        else
        {
            items[0] = items[itemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
        }
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem (T item)
    {
        SortUp(item);
    }

    void SortDown (T item)
    {
        while (true)
        {
            int leftChild = item.HeapIndex * 2 + 1;
            int rightChild = item.HeapIndex * 2 + 2;
            int swap = 0;
            if (leftChild < itemCount)
            {
                swap = leftChild;
                if (rightChild < itemCount)
                {
                    if (items[leftChild].CompareTo(items[rightChild]) < 0)
                    {
                        swap = rightChild;
                    }
                }

                if (item.CompareTo(items[swap]) < 0)
                {
                    Swap(item, items[swap]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp (T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                return;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap (T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        var swap = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = swap;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
