using SMPL.Language;
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

        public TextReporter(Model model, int maxPossibleLines = 100)
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

            Console.WriteLine($"{GlobalizationEngine.GetString("Model report")}. {GlobalizationEngine.GetString("Report was generated")} {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}");
            GetCurrentState(m_maxPossibleLines);
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            GetModelResult();
        }

        public void GetCurrentState(int maxPossibleLines = 100)
        {
            Console.WriteLine($"{GlobalizationEngine.GetString("Current model time")}: {m_model.ModelTime}");
            GetEventsState(maxPossibleLines);
            GetDevicesState(maxPossibleLines);
            GetQueuesState(maxPossibleLines);
        }

        public void GetModelResult(int maxPossibleLines = 100)
        {
            Console.WriteLine(GlobalizationEngine.GetString("Total model results") + ":");
            GetDevicesReport(maxPossibleLines);
            GetQueuesReport(maxPossibleLines);
        }

        private void GetQueuesReport(int maxPossibleLines)
        {
            Console.WriteLine($"  {GlobalizationEngine.GetString("Information about queues")}:");
            int outputLength = 0;
            foreach (SMQueue queue in m_model.GetQueues())
            {
                double avgWaitTime = queue.WaitTimeSum * 1.0f / queue.MaxObservedLength;
                Console.WriteLine($"    {GlobalizationEngine.GetString("Name of queue")}: {queue.Name}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Max observed length")}: {queue.MaxObservedLength}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Average wait time")}: {avgWaitTime.ToString("0.##")}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Average length")}: {(queue.TimeQueueSum * 1.0f / m_model.ModelTime).ToString("0.##")}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine(GlobalizationEngine.GetString("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1"));
                    return;
                }
            }
        }

        private void GetDevicesReport(int maxPossibleLines)
        {
            Console.WriteLine($"  {GlobalizationEngine.GetString("Information about devices")}:");
            int outputLength = 0;
            foreach (Device device in m_model.GetDevices())
            {
                double avgProcessingTime = device.TimeUsedSum * 1.0f / device.TransactCount;
                Console.WriteLine($"    {GlobalizationEngine.GetString("Name of device")}: {device.Name}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Average processing time")}: {avgProcessingTime.ToString("0.##")}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Load percentage")}: {(device.TimeUsedSum * 1.0f / m_model.ModelTime * 100).ToString("0.##")}%");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Transact count")}: {device.TransactCount}");

                if (device is MultiChannelDevice)
                {
                    MultiChannelDevice multichannelDevice = device as MultiChannelDevice;
                    Console.WriteLine($"    {GlobalizationEngine.GetString("Channel count")}: {multichannelDevice.StorageSize}");
                }

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine(GlobalizationEngine.GetString("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1"));
                    return;
                }
            }
        }

        private void GetQueuesState(int maxPossibleLines)
        {
            Console.WriteLine($"  {GlobalizationEngine.GetString("State of queues")}:");
            int outputLength = 0;
            foreach (SMQueue queue in m_model.GetQueues())
            {
                Console.WriteLine($"    {GlobalizationEngine.GetString("Name of queue")}: {queue.Name}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Max observed length")}: {queue.MaxObservedLength}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Number of current items")}: {queue.Count}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine(GlobalizationEngine.GetString("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1"));
                    return;
                }
            }
        }

        private void GetDevicesState(int maxPossibleLines)
        {
            Console.WriteLine($"  {GlobalizationEngine.GetString("State of devices")}:");
            int outputLength = 0;
            foreach (Device device in m_model.GetDevices())
            {
                Console.WriteLine($"    {GlobalizationEngine.GetString("Name of device")}: {device.Name}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Number of transact")}: {device.CurrentTransactId}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Last time used")}: {device.LastTimeUsed}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine(GlobalizationEngine.GetString("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1"));
                    return;
                }
            }
        }

        private void GetEventsState(int maxPossibleLines)
        {
            Console.WriteLine($"  {GlobalizationEngine.GetString("State of unprocessed events")}:");
            int outputLength = 0;
            foreach (Event @event in m_model.GetEvents())
            {
                Console.WriteLine($"    {GlobalizationEngine.GetString("Time of event")}: {@event.Time}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Number of event")}: {@event.EventId.ToString()}");
                Console.WriteLine($"    {GlobalizationEngine.GetString("Number of transact")}: {@event.TransactId}");

                if (maxPossibleLines != -1 && outputLength++ > maxPossibleLines)
                {
                    Console.WriteLine(GlobalizationEngine.GetString("There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1"));
                    return;
                }
            }
        }
    }
}
