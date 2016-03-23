using UnityEngine;
using System.Collections;
using System;               // for IComparable<T>

public class Heap<T> where T : IHeapItem<T>{

    T[] items;
    int currentItemCount;               // how many items are in the heap

    public Heap(int maxHeapSize) {      // have to know the maxsize since arry is had to resize
        items = new T[maxHeapSize];
    }

    public void Add(T item) {           // adding new items to the heap
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst() {            // gets the item with lowest f cost
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem (T item) {   // for changing the priority of item
        SortUp(item);       // no need to call SortDown too since we are only increase the prioiry
    }

    public int Count {      // number of currently in the heap
        get {
            return currentItemCount;
        }
    }

    public bool Contains(T item) {                  //  checks if item in the array with the same index
        return Equals(items[item.HeapIndex], item); // being passed in is equal to the actual item
    }

    void SortDown(T item) {
        while (true) {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if(childIndexLeft < currentItemCount) {         // check it this item have at least one child(child on the left)
                swapIndex = childIndexLeft;
                                                            // check to see if the rigth child also exist
                if(childIndexRight < currentItemCount) {    // if both items are less then currentItemCount
                    if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {   // checking which of the two has higher priority
                        swapIndex = childIndexRight;
                    }
                }

                if(item.CompareTo(items[swapIndex]) < 0){   // if parent has lower priority then the highest priority child
                    Swap(item, items[swapIndex]);
                }
                else {return;}  // if parent has highest priority then both of it's childrens, exit loop

            }
            else { return; }     // if parent doesn't have any children, exit the loop
        }
    }

    void SortUp(T item) {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true) {                          // infinitely sort until completion, then break out.
            T parentItem = items[parentIndex];
            if(item.CompareTo(parentItem) > 0) { // if priority is higher = 1 same = 0 lower =-1
                Swap(item, parentItem);
            }
            else { break; }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB) {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

// since it's hard to compare the genetic type T, we use interface
public interface IHeapItem<T> : IComparable<T>{
    int HeapIndex {
        get;
        set;
    }
}