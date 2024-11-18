using System;
using System.Collections.Generic;
using MunicipalServices.Models;
using MunicipalServices.Core.Services;
using MunicipalServices.Models;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestHeap
    {
        private List<ServiceRequest> heap;
        private readonly DataManager dataManager;

        public ServiceRequestHeap(DataManager dataManager)
        {
            this.heap = new List<ServiceRequest>();
            this.dataManager = dataManager;
        }

        public void Insert(ServiceRequest request)
        {
            heap.Add(request);
            HeapifyUp(heap.Count - 1);
        }

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

        public ServiceRequest Peek()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

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

        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        public int Count => heap.Count;
    }
}