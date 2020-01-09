/*
 * Данный пример демонстрирует использование многоканального устройства
 */

using System;
using SMPL;
using SMPL.Reporters;
using static SMPL.Language.GlobalizationEngine;

namespace Example3
{
    class Program
    {
        enum Events
        {
            EventGenerate = 1, // события порождения новой заявки
            EventRelease, // событие освобождения устройства
            EventReserve, // событие резервирования устройства
            EventEnd // событие окончания моделирования
        }

        static void Main(string[] args)
        {
            InitGlobalizationEngine(Language.RU);

            Model model = new Model();
            MultiChannelDevice multiChannelDevice = model.CreateMultiChannelDevice("MultiChannel", 7);
            SMQueue queue = model.CreateQueue("Accumulator");

            UInt64 transactCounter = 1;
            model.Schedule((int)Events.EventGenerate, model.Random(14, 26), transactCounter);
            model.Schedule((int)Events.EventEnd, 4800, (ulong)1e9);

            UInt64 currentTransact = 1;
            Events currentEvent = Events.EventEnd;

            do
            {
                (UInt64, UInt64) top = model.Cause();
                currentEvent = (Events)top.Item1;
                currentTransact = top.Item2;

                switch (currentEvent)
                {
                    case Events.EventGenerate:
                        {
                            model.Schedule((int)Events.EventReserve, 0, currentTransact);
                            transactCounter++;
                            model.Schedule((int)Events.EventGenerate, model.Random(14, 26), transactCounter);
                            break;
                        }
                    case Events.EventReserve:
                        {
                            if (multiChannelDevice.GetState() != Device.State.Full)
                            {
                                multiChannelDevice.Reserve(currentTransact);
                                model.Schedule((int)Events.EventRelease, model.Random(130, 150), currentTransact);
                            }
                            else
                            {
                                queue.Add(currentTransact, 0);
                            }

                            break;
                        }
                    case Events.EventRelease:
                        {
                            multiChannelDevice.Release();
                            if (queue.Count > 0)
                            {
                                UInt64 transactId = queue.Dequeue(out UInt64 stage).TransactId;
                                model.Schedule((int)Events.EventReserve, 0, transactId);
                            }

                            break;
                        }
                    case Events.EventEnd:
                        {
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
            }
            while (currentEvent != Events.EventEnd);

            using (HtmlReporter htmlReporter = new HtmlReporter(model))
            {
                htmlReporter.GetReport("output.html");
            }
        }
    }
}
