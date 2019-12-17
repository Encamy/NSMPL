using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMPL.Reporters
{
    public class MdReporter : IReporter, IDisposable
    {
        Model m_model;
        StreamWriter m_writer;

        public MdReporter(Model model)
        {
            m_model = model;
        }

        public void GetReport(string filename = null)
        {
            m_writer = new StreamWriter(filename);
            m_writer.AutoFlush = true;

            m_writer.WriteLine($"# Model report. Report was generated {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}");
            m_writer.WriteLine("");
            GetDevicesReport();
            m_writer.WriteLine("");
            GetQueuesReport();
        }

        private void GetQueuesReport()
        {
            m_writer.WriteLine("## Information about queues");
            m_writer.WriteLine("");
            m_writer.WriteLine("| Queue name | Max observed length | Average idle time | Average length |");
            m_writer.WriteLine("|-------------|--------------------|-------------------|----------------|");
            foreach (SMQueue queue in m_model.GetQueues())
            {
                double avgIdleTime = queue.WaitTimeSum * 1.0f / queue.MaxObeservedLength;
                string avgLength = (queue.TimeQueueSum * 1.0f / m_model.ModelTime).ToString("0.##");
                m_writer.WriteLine($"| {queue.Name} | {queue.MaxObeservedLength} | {avgIdleTime.ToString("0.##")} | {avgLength} |");
            }
        }

        private void GetDevicesReport()
        {
            m_writer.WriteLine("## Information about devices");
            m_writer.WriteLine("");
            m_writer.WriteLine("| Device name | Avg processign time | Load percentage | Transact count |");
            m_writer.WriteLine("|-------------|---------------------|-----------------|----------------|");
            foreach (Device device in m_model.GetDevices())
            {
                string avgProcessingTime = (device.TimeUsedSum * 1.0f / device.TransactCount).ToString("0.##");
                string loadPercentage = (device.TimeUsedSum * 1.0f / m_model.ModelTime * 100).ToString("0.##");
                m_writer.WriteLine($"| {device.Name} | {avgProcessingTime} | {loadPercentage}% | {device.TransactCount} |");
            }
        }

        public void Dispose()
        {
            m_writer.Dispose();
        }
    }
}
