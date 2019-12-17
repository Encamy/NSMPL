using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMPL.Reporters
{
    public class HtmlReporter : IReporter, IDisposable
    {
        Model m_model;
        StreamWriter m_writer;

        public HtmlReporter(Model model)
        {
            m_model = model;
        }

        public void GetReport(string filename = null)
        {
            m_writer = new StreamWriter(filename);
            m_writer.AutoFlush = true;

            WriteHtmlHeader();
            WriteBody();
            WriteHtmlFooter();
        }

        private void GetQueuesReport()
        {
            m_writer.WriteLine(
                "   <table>\r\n" +
                "    <caption>Information about queues</caption>\r\n" +
                "    <tr>\r\n" +
                "     <th>Queue name</th>\r\n" +
                "     <th>Max observed length</th>\r\n" +
                "     <th>Average idle time</th>\r\n" +
                "     <th>Average length</th>\r\n" +
                "    </tr>");

            foreach (SMQueue queue in m_model.GetQueues())
            {
                double avgIdleTime = queue.WaitTimeSum * 1.0f / queue.MaxObeservedLength;
                string avgLength = (queue.TimeQueueSum * 1.0f / m_model.ModelTime).ToString("0.##");
                m_writer.WriteLine($"<tr><td>{queue.Name}</td><td>{queue.MaxObeservedLength}</td><td>{avgIdleTime.ToString("0.##")}</td><td>{avgLength}</td></tr>");
            }

            m_writer.WriteLine("   </table>");
        }

        private void GetDevicesReport()
        {
            m_writer.WriteLine(
                "   <table>\r\n" +
                "    <caption>Information about devices</caption>\r\n" +
                "    <tr>\r\n" +
                "     <th>Device name</th>\r\n" +
                "     <th>Avg processign time</th>\r\n" +
                "     <th>Load percentage</th>\r\n" +
                "     <th>Transact count</th>\r\n" +
                "    </tr>");

            foreach (Device device in m_model.GetDevices())
            {
                string avgProcessingTime = (device.TimeUsedSum * 1.0f / device.TransactCount).ToString("0.##");
                string loadPercentage = (device.TimeUsedSum * 1.0f / m_model.ModelTime * 100).ToString("0.##");
                m_writer.WriteLine($"<tr><td>{device.Name}</td><td>{avgProcessingTime}</td><td>{loadPercentage}%</td><td>{device.TransactCount}</td></tr>");
            }

            m_writer.WriteLine("   </table>");
        }

        private void WriteHtmlHeader()
        {
            m_writer.WriteLine(
            "<!DOCTYPE HTML>\r\n" +
            "<html>\r\n" +
            " <head>\r\n" +
            "  <style type=\"text/css\">\r\n" +
            "   table { \r\n" +
            "    display: block; \r\n" +
            "    width: 100%; \r\n" +
            "    overflow: auto; \r\n" +
            "    border-spacing: 0; \r\n" +
            "    border-collapse: collapse;\r\n" +
            "    margin-top: 25px;\r\n" +
            "   }\r\n" +
            "   body {\r\n" +
            "    font-family: Segoe UI,Helvetica,Arial,sans-serif,Apple Color Emoji,Segoe UI Emoji;\r\n" +
            "    font-size: 16px;\r\n" +
            "    line-height: 1.5;\r\n" +
            "   }\r\n" +
            "   th {\r\n" +
            "    padding: 6px 13px;\r\n" +
            "    border: 1px solid #dfe2e5;\r\n" +
            "    font-weight: 600;\r\n" +
            "   }\r\n" +
            "   td {\r\n" +
            "    padding: 6px 13px;\r\n" +
            "    border: 1px solid #dfe2e5;\r\n" +
            "   }\r\n" +
            "   h1 {\r\n" +
            "    padding-bottom: .3em;\r\n" +
            "    border-bottom: 1px solid #eaecef;\r\n" +
            "    font-size: 1.7em;\r\n" +
            "   }\r\n" +
            "  </style>\r\n" +
            "  <meta charset=\"utf-8\">\r\n" +
            "  <title>Model report</title>\r\n" +
            " </head>");
        }

        private void WriteBody()
        {
            m_writer.WriteLine(" <body>\r\n");
            m_writer.WriteLine($"   <h1>Report was generated {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}</h1>");
            GetDevicesReport();
            GetQueuesReport();
            m_writer.WriteLine(" </body>");
        }

        private void WriteHtmlFooter()
        {
            m_writer.WriteLine("</html>");
        }

        public void Dispose()
        {
            m_writer.Dispose();
        }
    }
}
