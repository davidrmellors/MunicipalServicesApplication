using System;
using System.Collections.Generic;
using MunicipalServices.Models;
using MunicipalServices.Core.Services;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a max-heap data structure for storing and prioritizing service requests.
    /// Provides methods for insertion, extraction, and peeking at the highest priority request.
    /// </summary>
    public class ServiceRequestHeap
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// List storing the heap elements.
        /// </summary>
        private List<ServiceRequest> heap;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceRequestHeap class.
        /// </summary>
        public ServiceRequestHeap()
        {
            this.heap = new List<ServiceRequest>();
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new service request into the heap while maintaining the heap property.
        /// </summary>
        /// <param name="request">The service request to insert.</param>
        public void Insert(ServiceRequest request)
        {
            heap.Add(request);
            HeapifyUp(heap.Count - 1);
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Removes and returns the highest priority service request from the heap.
        /// </summary>
        /// <returns>The service request with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when heap is empty.</exception>
        public ServiceRequest ExtractMax()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");

            var max = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0);

            return max;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the highest priority service request without removing it from the heap.
        /// </summary>
        /// <returns>The service request with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when heap is empty.</exception>
        public ServiceRequest Peek()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Restores the heap property by moving an element up the heap.
        /// </summary>
        /// <param name="index">The index of the element to move up.</param>
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[parentIndex].Priority >= heap[index].Priority)
                    break;

                Swap(parentIndex, index);
                index = parentIndex;
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Restores the heap property by moving an element down the heap.
        /// </summary>
        /// <param name="index">The index of the element to move down.</param>
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int largest = index;
                int left = 2 * index + 1;
                int right = 2 * index + 2;

                if (left < heap.Count && heap[left].Priority > heap[largest].Priority)
                    largest = left;

                if (right < heap.Count && heap[right].Priority > heap[largest].Priority)
                    largest = right;

                if (largest == index)
                    break;

                Swap(index, largest);
                index = largest;
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Swaps two elements in the heap.
        /// </summary>
        /// <param name="i">Index of first element.</param>
        /// <param name="j">Index of second element.</param>
        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of elements in the heap.
        /// </summary>
        public int Count => heap.Count;
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//