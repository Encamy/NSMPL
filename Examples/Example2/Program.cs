using SMPL;
using SMPL.Reporters;
using System;
using static SMPL.Language.GlobalizationEngine;

namespace Example2
{

    class Program
    {
        const int m1 = 6;
        const int s1 = 1;
        const int m2 = 5;
        const int s2 = 1;
        const int T1 = 3;
        const int T1_dop = 1;
        const int T2 = 7;
        const int T2_dop = 1;
        const int T3 = 8;
        const int T3_dop = 2;

        const int d1 = 20;
        const int d2 = 40;
        const int z1 = 2;
        const int z2 = 4;

        const int m_maxAvailableItems1 = 3;
        const int m_maxAvailableItems2 = 3;
        static int m_currentItems1 = 0;
        static int m_currentItems2 = 0;

        enum Events
        {
            Incoming1 = 1,
            Incoming2,
            ProcessorReserve1,
            ProcessorReserve2,
            ProcessorRelease1,
            ProcessorRelease2,
            OutputBufferReserve1,
            OutputBufferReserve2,
            OutputBufferRelease1,
            OutputBufferRelease2,
            End
        }

        static void Main(string[] args)
        {
            InitGlobalizationEngine(Language.RU);

            Model model = new Model();
            Device processor = model.CreateDevice("Processor");
            Device outputBuffer1 = model.CreateDevice("Output buffer 1");
            Device outputBuffer2 = model.CreateDevice("Output buffer 2");

            SMQueue inputQueue = model.CreateQueue("Input queue");
            SMQueue outputQueue1 = model.CreateQueue("Output queue 1");
            SMQueue outputQueue2 = model.CreateQueue("Output queue 2");

            SMQueue resultQueue1 = model.CreateQueue("Total processed messages from input 1");
            SMQueue resultQueue2 = model.CreateQueue("Total processed messages from input 2");

            UInt64 transactCounter = 1;

            model.Schedule((int)Events.Incoming1, model.Random(m1 - s1, m1 + s1), transactCounter);
            model.Schedule((int)Events.Incoming2, model.Random(m2 - s2, m2 + s2), ++transactCounter);
            model.Schedule((int)Events.End, (ulong)TimeSpan.FromMinutes(24).TotalMilliseconds, (ulong)1e9);

            UInt64 transact = 1;
            Events @event = Events.End;

            do
            {
                (UInt64, UInt64) top = model.Cause();
                @event = (Events)top.Item1;
                transact = top.Item2;

                switch (@event)
                {
                    case Events.Incoming1:
                        {
                            if (m_currentItems1 > m_maxAvailableItems1)
                            {
                                // Do nothing delete item and schedule next
                                model.Schedule((int)Events.Incoming1, model.Random(m1 - s1, m1 + s1), transactCounter);
                                break;
                            }
                            m_currentItems1++;
                            model.Schedule((int)Events.ProcessorReserve1, 0, transact);
                            transactCounter++;
                            model.Schedule((int)Events.Incoming1, model.Random(m1 - s1, m1 + s1), transactCounter);
                            break;
                        }
                    case Events.Incoming2:
                        {
                            if (m_currentItems2 > m_maxAvailableItems2)
                            {
                                // Do nothing delete item and schedule next
                                model.Schedule((int)Events.Incoming2, model.Random(m2 - s2, m2 + s2), transactCounter);
                                break;
                            }
                            m_currentItems2++;
                            model.Schedule((int)Events.ProcessorReserve2, 0, transact);
                            transactCounter++;
                            model.Schedule((int)Events.Incoming2, model.Random(m2 - s2, m2 + s2), transactCounter);
                            break;
                        }
                    case Events.ProcessorReserve1:
                        {
                            if (processor.GetState() == Device.State.Idle)
                            {
                                processor.Reserve(transact);
                                model.Schedule((int)Events.ProcessorRelease1, model.Random(T1 - T1_dop, T1 + T1_dop), transact);
                            }
                            else
                            {
                                // Stage gives info is it item from first input or second
                                inputQueue.Add(transact, 1);
                            }

                            break;
                        }
                    case Events.ProcessorReserve2:
                        {
                            if (processor.GetState() == Device.State.Idle)
                            {
                                processor.Reserve(transact);
                                model.Schedule((int)Events.ProcessorRelease2, model.Random(T1 - T1_dop, T1 + T1_dop), transact);
                            }
                            else
                            {
                                // Stage gives info is it item from first input or second
                                inputQueue.Add(transact, 2);
                            }

                            break;
                        }
                    case Events.ProcessorRelease1:
                        {
                            processor.Release();
                            if (inputQueue.Count > 0)
                            {
                                UInt64 transactId = inputQueue.Dequeue(out UInt64 stage).TransactId;
                                // In case if transact from first input is placed on top of queue generate ProcessorReserve1 otherwise ProcessorReserve2
                                if (stage == 1)
                                {
                                    model.Schedule((int)Events.ProcessorReserve1, 0, transactId);
                                }
                                else if (stage == 2)
                                {
                                    model.Schedule((int)Events.ProcessorReserve2, 0, transactId);
                                }
                            }

                            model.Schedule((int)Events.OutputBufferReserve1, 0, transact);

                            break;
                        }
                    case Events.ProcessorRelease2:
                        {
                            processor.Release();
                            if (inputQueue.Count > 0)
                            {
                                UInt64 transactId = inputQueue.Dequeue(out UInt64 stage).TransactId;
                                // In case if transact from first input is placed on top of queue generate ProcessorReserve1 otherwise ProcessorReserve2
                                if (stage == 1)
                                {
                                    model.Schedule((int)Events.ProcessorReserve1, 0, transactId);
                                }
                                else if (stage == 2)
                                {
                                    model.Schedule((int)Events.ProcessorReserve2, 0, transactId);
                                }
                            }

                            model.Schedule((int)Events.OutputBufferReserve2, 0, transact);

                            break;
                        }
                    case Events.OutputBufferReserve1:
                        {
                            if (outputBuffer1.GetState() == Device.State.Idle)
                            {
                                outputBuffer1.Reserve(transact);
                                model.Schedule((int)Events.OutputBufferRelease1, model.Random(T2 - T2_dop, T2 + T2_dop), transact);
                            }
                            else
                            {
                                outputQueue1.Add(transact, 0);
                            }
                            break;
                        }
                    case Events.OutputBufferReserve2:
                        {
                            if (outputBuffer2.GetState() == Device.State.Idle)
                            {
                                outputBuffer2.Reserve(transact);
                                model.Schedule((int)Events.OutputBufferRelease2, model.Random(T3 - T3_dop, T3 + T3_dop), transact);
                            }
                            else
                            {
                                outputQueue2.Add(transact, 0);
                            }
                            break;
                        }
                    case Events.OutputBufferRelease1:
                        {
                            outputBuffer1.Release();
                            if (outputQueue1.Count > 0)
                            {
                                UInt64 transactId = outputQueue1.Dequeue(out UInt64 stage).TransactId;
                                if (stage == 0)
                                {
                                    model.Schedule((int)Events.OutputBufferReserve1, 0, transactId);
                                }
                            }

                            m_currentItems1--;
                            resultQueue1.Add(transact, 1);
                            break;
                        }
                    case Events.OutputBufferRelease2:
                        {
                            outputBuffer2.Release();
                            if (outputQueue2.Count > 0)
                            {
                                UInt64 transactId = outputQueue2.Dequeue(out UInt64 stage).TransactId;
                                if (stage == 0)
                                {
                                    model.Schedule((int)Events.OutputBufferReserve2, 0, transactId);
                                }
                            }

                            m_currentItems2--;
                            resultQueue2.Add(transact, 1);
                            break;
                        }
                    case Events.End:
                        {
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
            }
            while (@event != Events.End);

            using (HtmlReporter htmlReporter = new HtmlReporter(model))
            {
                htmlReporter.GetReport("output.html");
            }
        }
    }
}
