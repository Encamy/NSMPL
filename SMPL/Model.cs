using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMPL
{
    public class Model
    {
        List<SMQueue> m_queues;
        List<Device> m_devices;
        PriorityQueue<Event> m_events;

        UInt64 m_modelTime;

        public Model()
        {
            m_queues = new List<SMQueue>();
            m_devices = new List<Device>();
            m_events = new PriorityQueue<Event>();
            m_modelTime = 0;
        }

        public ulong ModelTime { get => m_modelTime; }

        public Device CreateDevice(string name)
        {
            Device device = new Device(this, name);
            m_devices.Add(device);
            return device;
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

        public void GetReport()
        {
            Console.WriteLine($"Время моделирования {ModelTime} тактов");
        }
    }
}
