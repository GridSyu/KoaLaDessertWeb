若要增加LogType在LogType資料夾增加一個類即可。

使用方式
``` csharp
// 定義
Logger _loggerForNormal = new Logger(new Normal(), "Log");
Logger _loggerForError = new Logger(new Error(), "Log");

// 範例
// 寫入log
_loggerForNormal.Write("test", "Run");

var startDate = DateTimeOffset.UtcNow.AddDays(-1);
var endDate = DateTimeOffset.UtcNow;
// 讀取log
List<LogOutput>? logOutputs = _loggerForNormal.ReadByUTCTime(startDate, endDate);
Console.WriteLine("讀取log...");
if (logOutputs != null)
{
    foreach (var item in logOutputs)
    {
        Console.WriteLine($"[{item.Timestamp}] {item.From} {item.Content}");
    }
}
// 讀取log (依來源)
List<LogOutput>? logOutputsByFrom = _loggerForNormal.ReadByUTCTimeAndFrom(startDate, endDate, "ggg");
Console.WriteLine("讀取log (依來源)...");
if (logOutputsByFrom != null)
{
    foreach (var item in logOutputsByFrom)
    {
        Console.WriteLine($"[{item.Timestamp}] {item.From} {item.Content}");
    }
}

```
