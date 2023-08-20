using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelegram
{
    public class ResultFormCreate
 {
    public Result Result { get; set; }
    public int Id { get; set; }
    public object Exception { get; set; }
    public int Status { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsCompleted { get; set; }
    public int CreationOptions { get; set; }
    public object AsyncState { get; set; }
    public bool IsFaulted { get; set; }
}

public class Result
{
    public int message_thread_id { get; set; }
    public string name { get; set; }
    public int icon_color { get; set; }
}

}
