using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoaLaDessertWeb.Tools.Logger.LogType;
using KoaLaDessertWeb.Tools.Logger.Model;

namespace KoaLaDessertWeb.Tools.Logger
{
    internal class Logger
    {
        private LogInterface _log;
        private static string _rootFilePath; // 設定log存放的根目錄

        public Logger(LogInterface log, string rootFilePath)
        {
            _log = log;
            _rootFilePath = rootFilePath;
            // 設定log存放的根目錄
            _log.RootFilePath = _rootFilePath;
            // 創建存放log的資料夾
            CreateDirectory();
        }

        /// <summary>
        /// 創建存放log的資料夾
        /// </summary>
        private void CreateDirectory()
        {
            try
            {
                // 組合資料夾路徑
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), _rootFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// 切換log類型
        /// </summary>
        public void ChangeLogType(LogInterface log)
        {
            _log = log;
        }

        /// <summary>
        /// 寫入log訊息
        /// </summary>
        public void Write(string message, string from)
        {
            Console.WriteLine(message);
            _log.Write(message, from);
        }

        /// <summary>
        /// 讀取log訊息
        /// </summary>
        public List<LogOutput>? ReadByUTCTime(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return _log.ReadByUTCTime(startDate, endDate);
        }

        /// <summary>
        /// 依來源與時間讀取log訊息
        /// </summary>
        public List<LogOutput>? ReadByUTCTimeAndFrom(DateTimeOffset startDate, DateTimeOffset endDate, string from)
        {
            try
            {
                // 依來源與時間讀取log訊息
                var logs = _log.ReadByUTCTime(startDate, endDate);
                if (logs != null)
                {
                    logs = logs.Where(c => c.From == from).ToList();
                }

                return logs;
            }
            catch (Exception)
            {
                return default;
            }
            
        }


    }
}
