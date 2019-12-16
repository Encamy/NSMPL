using System;

namespace SMPL
{
    public class Item
    {
        private UInt64 m_transactId;
        private UInt64 m_incomingTime;
        private UInt64 m_stage;

        public Item(UInt64 modelTime, UInt64 transactId, UInt64 stage)
        {
            m_incomingTime = modelTime;
            TransactId = transactId;
            Stage = stage;
        }

        public UInt64 TransactId { get => m_transactId; set => m_transactId = value; }
        public UInt64 IncomingTime { get => m_incomingTime; set => m_incomingTime = value; }
        public UInt64 Stage { get => m_stage; set => m_stage = value; }
    }
}