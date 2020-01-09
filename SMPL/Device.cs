using System;

namespace SMPL
{
    public partial class Device
    {
        public string Name { get => m_name; set => m_name = value; }
        public UInt64? CurrentTransactId { get => m_currentTransactId; }
        public UInt64 LastTimeUsed { get => m_lastTimeUsed; }
        public UInt64 TransactCount { get => m_transactCount; }
        public UInt64 TimeUsedSum { get => m_timeUsedSum; }

        private UInt64? m_currentTransactId;
        protected string m_name;
        protected UInt64 m_lastTimeUsed;
        protected UInt64 m_transactCount;
        protected UInt64 m_timeUsedSum;
        protected Model m_model;

        public Device(Model model, string name)
        {
            m_model = model;
            m_name = name;
            m_timeUsedSum = 0;
        }

        virtual public void Reserve(UInt64 transactId)
        {
            if (m_currentTransactId.HasValue && m_currentTransactId != 0)
            {
                throw new InvalidOperationException("Device is busy");
            }

            m_currentTransactId = transactId;
            m_lastTimeUsed = m_model.ModelTime;
        }

        virtual public UInt64 Release()
        {
            if (!m_currentTransactId.HasValue || m_currentTransactId == 0)
            {
                throw new InvalidOperationException("Can't release free device");
            }

            m_timeUsedSum += m_model.ModelTime - LastTimeUsed;
            m_transactCount++;

            UInt64 transactId = m_currentTransactId.Value;
            m_currentTransactId = null;

            return transactId;
        }

        virtual public State GetState()
        {
            if (m_currentTransactId.HasValue && m_currentTransactId != 0)
            {
                return State.Active;
            }

            return State.Idle;
        }
    }
}
