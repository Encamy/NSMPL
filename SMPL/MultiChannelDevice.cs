using System;
using System.Collections.Generic;
using System.Text;

namespace SMPL
{
    public class MultiChannelDevice : Device
    {
        private Queue<UInt64> m_currentTransacts;

        public int StorageSize = 0;
        private int AvailableCapacity = 0;

        public MultiChannelDevice(Model model, string name) : base(model, name)
        {
            m_currentTransacts = new Queue<UInt64>();
            StorageSize = 1;
            AvailableCapacity = StorageSize;
        }

        public override State GetState()
        {
            if (m_currentTransacts.Count == StorageSize)
            {
                return State.Full;
            }

            if (m_currentTransacts.Count > 0)
            {
                return State.Active;
            }

            return State.Idle;
        }

        public override void Reserve(UInt64 transactId)
        {
            if (m_currentTransacts.Contains(transactId))
            {
                throw new InvalidOperationException("Transact is already in device");
            }

            if (m_currentTransacts.Count >= StorageSize)
            {
                throw new InvalidOperationException("Device is busy");
            }

            m_currentTransacts.Enqueue(transactId);

            m_lastTimeUsed = m_model.ModelTime;
            AvailableCapacity--;
        }

        public override UInt64 Release()
        {
            if (m_currentTransacts.Count == 0)
            {
                throw new InvalidOperationException("Can't release free device");
            }

            m_timeUsedSum += m_model.ModelTime - LastTimeUsed;
            m_transactCount++;

            AvailableCapacity++;
            return m_currentTransacts.Dequeue();
        }

        public void SetCapacity(int capacity)
        {
            StorageSize = capacity;
            AvailableCapacity = StorageSize;
        }
    }
}
