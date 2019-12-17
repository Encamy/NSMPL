using System;
using System.Collections.Generic;
using System.Text;

namespace SMPL.Language
{
    // This can be implemented using resources, but as for me they are overcomplicated and does not seems like suitable here
    public static class GlobalizationEngine
    {
        private static Language m_language = Language.EN;
        private static Dictionary<string, string> m_englishDictionary = new Dictionary<string, string>
        {
            { "Total model results", "Total model results" },
            { "Current model time", "Current model time" },
            { "Information about queues", "Information about queues" },
            { "Name of queue", "Queue name" },
            { "Max observed length", "Max observed length" },
            { "Average idle time", "Average idle time" },
            { "Average length", "Average length" },
            { "Information about devices", "Information about devicess" },
            { "There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1", "There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1" },
            { "Name of device", "Device name" },
            { "Average processing time", "Average processing time" },
            { "Load percentage", "Load percentage" },
            { "Transact count", "Transact count" },
            { "Number of current items", "Number of current items" },
            { "Number of transact", "Number of transact" },
            { "Last time used", "Last time used" },
            { "Number of event", "Number of event" },
            { "Time of event", "Time of event" },
            { "State of queues", "State of queues" },
            { "State of devices", "State of devices" },
            { "State of unprocessed events", "State of unprocessed events" },
            { "Model report", "Model report" },
            { "Report was generated", "Report was generated" },
        };

        private static Dictionary<string, string> m_russianDictionary = new Dictionary<string, string>
        {
            { "Total model results", "Общие результаты модели" },
            { "Current model time", "Текущее модельное время" },
            { "Information about queues", "Информация об очередях" },
            { "Name of queue", "Название очереди" },
            { "Max observed length", "Максимальная наблюдаемая длина очереди" },
            { "Average idle time", "Среднее время простоя" },
            { "Average length", "Средняя длина" },
            { "Information about devices", "Информация об устройствах" },
            { "There are too many items to be displayed. Truncating output. If you want to avoid it, set maxPossibleLines to -1", "В отчете содержится слишком много данных, вывод будет ограничен. Чтобы избежать этого, задайте аргументы maxPossibleLines -1" },
            { "Name of device", "Название устройства" },
            { "Average processing time", "Среднее время обработки" },
            { "Load percentage", "Загруженность" },
            { "Transact count", "Количество транзактов" },
            { "Number of current items", "Количество текущих событий" },
            { "Number of transact", "Номер текущего транзакта" },
            { "Last time used", "Время последнего использования" },
            { "Number of event", "Номер текущего события" },
            { "Time of event", "Время события" },
            { "State of queues", "Состояние очередей" },
            { "State of devices", "Состояние устройств" },
            { "State of unprocessed events", "Состояние необработанных событий" },
            { "Model report", "Отчет о модели" },
            { "Report was generated", "Отчет был сформирован" },
        };

        public static void InitGlobalizationEngine(Language language)
        {
            m_language = language;
        }

        public static string GetString(string name)
        {
            switch (m_language)
            {
                case Language.EN:
                    return m_englishDictionary[name];
                case Language.RU:
                    return m_russianDictionary[name];
                default:
                    throw new NotImplementedException();
            }
        }

        public enum Language
        {
            EN,
            RU
        }
    }
}
