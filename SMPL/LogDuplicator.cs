using System;
using System.IO;
using System.Text;

namespace SMPL
{
    internal class LogDuplicator
    {
        private static TextWriter m_current;
        private static string m_outputFile;

        private class OutputWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return m_current.Encoding;
                }
            }

            public override void WriteLine(string value)
            {
                m_current.WriteLine(value);
                File.AppendAllLines(m_outputFile, new string[] { value });
            }
        }

        public static void Init(string output)
        {
            m_outputFile = output;
            m_current = Console.Out;
            File.Delete(output);
            Console.SetOut(new OutputWriter());
        }
    }
}