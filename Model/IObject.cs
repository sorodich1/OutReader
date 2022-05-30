using System;
using OutReader.Helper;

namespace OutReader.Model
{
    interface IObject
    {
        DateTime LastUpdate { get; set; }
        bool NotConnection { get; }
        bool IsExceptionClient { get; set; }
        string ExceptionMessage { get; set; }
        void Run();
        void SaveToScada();
        void SaveToSql();
        void SaveLogToSql();
        void ConvertM();
        void Update();
    }
}
