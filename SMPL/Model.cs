using DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SMPL
{
    public class Model
    {
        List<SMQueue> m_queues;
        List<Device> m_devices;
        PriorityQueue<Event> m_events;
        Random m_random;
        Random m_normalDistribution;

        UInt64 m_modelTime;

        public Model()
        {
            m_queues = new List<SMQueue>();
            m_devices = new List<Device>();
            m_events = new PriorityQueue<Event>();
            m_modelTime = 0;
            m_random = new Random((int)DateTime.UtcNow.Ticks);
            m_normalDistribution = new Random((int)DateTime.UtcNow.Ticks);
        }

        public ulong ModelTime { get => m_modelTime; }

        public Device CreateDevice(string name)
        {
            Device device = new Device(this, name);
            m_devices.Add(device);
            return device;
        }

        public ReadOnlyCollection<SMQueue> GetQueues()
        {
            return m_queues.AsReadOnly();
        }

        public ReadOnlyCollection<Device> GetDevices()
        {
            return m_devices.AsReadOnly();
        }

        public ReadOnlyCollection<Event> GetEvents()
        {
            return new ReadOnlyCollection<Event>(m_events.ToList());
        }

        public SMQueue CreateQueue(string name)
        {
            SMQueue queue = new SMQueue(this, name);
            m_queues.Add(queue);
            return queue;
        }

        public Event Schedule(UInt64 eventId, UInt64 time, UInt64 transactId)
        {
            Event @event = new Event(eventId, ModelTime + time, transactId);
            m_events.Add(@event);
            return @event;
        }

        public (UInt64, UInt64) Cause()
        {
            if (m_events.Count == 0)
            {
                throw new InvalidOperationException("Event queue is empty");
            }

            Event @event = m_events.Take();
            m_modelTime = @event.Time;
            return (@event.EventId, @event.TransactId);
        }

        public void Cancel(UInt64 eventId, UInt64 transactID)
        {
            Event @event = m_events.First(x => x.TransactId == transactID && x.EventId == eventId);
            m_events.Remove(@event);
        }

        public UInt64 Random(int left, int right)
        {
            return (UInt64)m_random.Next(left, right);
        }

        /// <summary>
        /// Generates random values from normal distribution.
        /// Note that here can be only positive values so if you will pass values close to 0, distrubution will not be normal
        /// </summary>
        public UInt64 NormalDistribution(int deviation, int expectation)
        {
            double u1 = 1.0 - m_normalDistribution.NextDouble();
            double u2 = 1.0 - m_normalDistribution.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = expectation + deviation * randStdNormal;
            UInt64 value = (UInt64)Math.Abs(Math.Round(randNormal));
            return value;
        }
    }
}
