using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL
{
    public class ResponseMessage<T>   
    {

        public int Code { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }

        public static ResponseMessage<T> GetResult (int code, string msg, T data = default(T))
        {
            return new ResponseMessage<T>
            {
                Code = code,
                Msg = msg,
                Data = data
            };
        }


    }
}
