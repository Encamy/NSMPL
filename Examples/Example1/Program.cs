using SMPL;
using SMPL.Reporters;
using System;
using static SMPL.Language.GlobalizationEngine;

namespace Example1
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
            InitGlobalizationEngine(Language.EN);

            Model model = new Model();
            Device device = model.CreateDevice("Master");
            SMQueue queue = model.CreateQueue("Accumulator");

            UInt64 transactCounter = 1;

            model.Schedule((int)Events.EventGenerate, model.Random(14, 26), transactCounter);
            model.Schedule((int)Events.EventEnd, 480, (ulong)1e9);

            // Объявим переменную, в которой будем хранить номер текущего транзакта
            UInt64 transact = 1;
            // И переменную, в которй будем хранить тип текущего события
            Events @event = Events.EventEnd;

            do
            {
                (UInt64, UInt64) top = model.Cause();
                // Разбираем пару на отдельные составляющие
                @event = (Events)top.Item1;
                transact = top.Item2;

                switch (@event)
                {
                    // Алгоритм обработки события порождения заявки
                    case Events.EventGenerate:
                        {
                            // Планируем сразу же событие резервирования устройства за этой заявкой
                            model.Schedule((int)Events.EventReserve, 0, transact);
                            // Увеличиваем счетчик транзактов
                            transactCounter++;
                            // Планируем поступление следующией заявки
                            model.Schedule((int)Events.EventGenerate, model.Random(14, 26), transactCounter);
                            break;
                        }
                    case Events.EventReserve:
                        {
                            if (device.GetState() == Device.State.Idle)
                            {
                                // В случае, если устройство свободно, займем его заявкой
                                device.Reserve(transact);
                                // Запланируем освобождение устройства
                                model.Schedule((int)Events.EventRelease, model.Random(12, 20), transact);
                            }
                            else
                            {
                                queue.Add(transact, 0);
                            }
                            
                            break;
                        }
                    case Events.EventRelease:
                        {
                            device.Release();
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
            while (@event != Events.EventEnd);

            TextReporter textReporter = new TextReporter(model);
            textReporter.GetReport("output.txt");

            using (MdReporter mdReporter = new MdReporter(model))
            {
                mdReporter.GetReport("output.md");
            }

            using (HtmlReporter htmlReporter = new HtmlReporter(model))
            {
                htmlReporter.GetReport("output.html");
            }
        }
    }
}
