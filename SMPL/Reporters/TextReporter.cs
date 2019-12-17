using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMPL.Reporters
{
    public class TextReporter : IReporter
    {
        private Model m_model;
        private int m_maxPossibleLines;

        public TextReporter(Model model, string outputFilename = null, int maxPossibleLines = 100)
        {
            m_model = model;
            m_maxPossibleLines = maxPossibleLines;
        }

        public void GetReport(string filename = null)
        {
            if (filename != null && filename.Length > 0)
            {
                LogDuplicator.Init(filename);
            }

            GetCurrentState(m_maxPossibleLines);
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            GetModelResult();
        }

        public void GetCurrentState(int maxPossibleLines = 100)
        {
            Console.WriteLine($"Current model time: {m_model.ModelTime}");
            GetEventsState(maxPossibleLines);
            GetDevicesState(maxPossibleLines);
            GetQueuesState(maxPossibleLines);
        }

        public void GetModelResult(int maxPossibleLines = 100)
        {
            Console.WriteLine("Total model results:");
            GetDevicesReport(maxPossibleLines);
            GetQueuesReport(maxPossibleLines);
        }

        private void GetQueuesReport(int maxPossibleLines)
        {
            Console.WriteLine("  Information about queues:");
            int outputLength = 0;
            foreach (SMQueue queue in m_model.GetQueues())
            {
                double avgIdleTime = queue.WaitTimeSum * 1.0f / queue.MaxObeservedLength;
                Console.WriteLine($"    Name of queue: {queue.Name}");
                Console.WriteLine($"    Max observed length: {queue.MaxObeservedLength}");
                Console.WriteLine($"    Average idle time: {avgIdleTime.ToString("0.##")}");
                Console.WriteLine($"    Average length: {(queue.TimeQueueSum * 1.0f / m_model.ModelTime).ToString("0.##")}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1");
                    return;
                }
            }
        }

        private void GetDevicesReport(int maxPossibleLines)
        {
            Console.WriteLine("  Information about devices");
            int outputLength = 0;
            foreach (Device device in m_model.GetDevices())
            {
                double avgProcessingTime = device.TimeUsedSum * 1.0f / device.TransactCount;
                Console.WriteLine($"    Name of device: {device.Name}");
                Console.WriteLine($"    Average processing time: {avgProcessingTime.ToString("0.##")}");
                Console.WriteLine($"    Load percentage : {(device.TimeUsedSum * 1.0f / m_model.ModelTime * 100).ToString("0.##")}%");
                Console.WriteLine($"    Transact count : {device.TransactCount}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1");
                    return;
                }
            }
        }

        private void GetQueuesState(int maxPossibleLines)
        {
            Console.WriteLine("  State of queues:");
            int outputLength = 0;
            foreach (SMQueue queue in m_model.GetQueues())
            {
                Console.WriteLine($"    Name of queue: {queue.Name}");
                Console.WriteLine($"    Max observed length: {queue.MaxObeservedLength}");
                Console.WriteLine($"    Number of current items: {queue.Count}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1");
                    return;
                }
            }
        }

        private void GetDevicesState(int maxPossibleLines)
        {
            Console.WriteLine("  State of devices:");
            int outputLength = 0;
            foreach (Device device in m_model.GetDevices())
            {
                Console.WriteLine($"    Name of device: {device.Name}");
                Console.WriteLine($"    Number of transact: {device.CurrentTransactId}");
                Console.WriteLine($"    Last time used: {device.LastTimeUsed}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1");
                    return;
                }
            }
        }

        private void GetEventsState(int maxPossibleLines)
        {
            Console.WriteLine("  State of unprocessed events:");
            int outputLength = 0;
            foreach (Event @event in m_model.GetEvents())
            {
                Console.WriteLine($"    Time of event: {@event.Time}");
                Console.WriteLine($"    Number of event: {@event.EventId.ToString()}");
                Console.WriteLine($"    Transact number: {@event.TransactId}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1");
                    return;
                }
            }
        }
    }
}
