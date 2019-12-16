using System;

namespace SMPL
{
    public class Device
    {
        public string Name { get => m_name; set => m_name = value; }
        public UInt64? CurrentTransactId { get => m_currentTransactId; }
        public UInt64 LastTimeUsed { get => m_lastTimeUsed; }
        public UInt64 TransactCount { get => m_transactCount; }
        public UInt64 TimeUsedSum { get => m_timeUsedSum; }

        private string m_name;
        private UInt64? m_currentTransactId;
        private UInt64 m_lastTimeUsed;
        private UInt64 m_transactCount;
        private UInt64 m_timeUsedSum;
        private Model m_model;

        public Device(Model model, string name)
        {
            m_model = model;
            m_name = name;
            m_timeUsedSum = 0;
        }

        public void Reserve(UInt64 transactId)
        {
            if (m_currentTransactId.HasValue && m_currentTransactId != 0)
            {
                throw new InvalidOperationException("Device is busy");
            }

            m_currentTransactId = transactId;
            m_lastTimeUsed = m_model.ModelTime;
        }

        public void Release()
        {
            if (!m_currentTransactId.HasValue || m_currentTransactId == 0)
            {
                throw new InvalidOperationException("Can't release free device");
            }

            m_timeUsedSum += m_model.ModelTime - LastTimeUsed;
            m_transactCount++;
            m_currentTransactId = null;
        }

        public State GetState()
        {
            if (m_currentTransactId.HasValue && m_currentTransactId != 0)
            {
                return State.Active;
            }

            return State.Idle;
        }

        public enum State
        {
            Idle,
            Active
        }
    }
}
