using System;
using System.Collections;
using System.Collections.Generic;

namespace SMPL
{
    public class SMQueue : ICollection<Item>
    {
        private object m_syncRoot;
        private Queue<Item> m_queue;
        private UInt64 m_timeQueueSum;
        private UInt64 m_waitTimeSum;
        private UInt64 m_waitTimeSumSquared;
        private UInt64 m_lastTimeChanged;
        private string m_name;
        private UInt64 m_maxObeservedLength;
        private Model m_model;

        public SMQueue(Model model, string name)
        {
            m_queue = new Queue<Item>();
            m_syncRoot = new object();
            m_name = name;
            m_model = model;
        }

        public int Count { get => m_queue.Count; }

        public bool IsSynchronized => false;

        public object SyncRoot { get => m_syncRoot; }
        public UInt64 TimeQueueSum { get => m_timeQueueSum; }
        public UInt64 WaitTimeSum { get => m_waitTimeSum; }
        public UInt64 LastTimeChanged { get => m_lastTimeChanged; }
        public string Name { get => m_name; set => m_name = value; }

        public bool IsReadOnly => false;

        public UInt64 MaxObservedLength { get => m_maxObeservedLength; }

        public void Add(Item item)
        {
            m_queue.Enqueue(item);
            m_timeQueueSum += (UInt64)(m_queue.Count - 1) * (m_model.ModelTime - LastTimeChanged);
            m_maxObeservedLength = Math.Max((UInt64)m_queue.Count, m_maxObeservedLength);
            m_lastTimeChanged = m_model.ModelTime;
        }

        public void Add(UInt64 transactId, UInt64 stage)
        {
            Item item = new Item(m_model.ModelTime, transactId, stage);
            Add(item);
        }

        public Item Dequeue(out UInt64 outStage)
        {
            Item item = m_queue.Dequeue();

            m_timeQueueSum += (UInt64)(m_queue.Count + 1) * (m_model.ModelTime - item.IncomingTime);
            m_waitTimeSum += m_model.ModelTime - item.IncomingTime;
            m_waitTimeSumSquared += (UInt64)Math.Pow(m_model.ModelTime - item.IncomingTime, 2);
            m_lastTimeChanged = m_model.ModelTime;

            outStage = item.Stage;

            return item;
        }

        public void Clear()
        {
            m_queue.Clear();
        }

        public bool Contains(Item item)
        {
            return m_queue.Contains(item);
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            m_queue.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return m_queue.GetEnumerator();
        }

        public bool Remove(Item item)
        {
            throw new InvalidOperationException("Can't remove item from queue");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_queue.GetEnumerator();
        }
    }
}
