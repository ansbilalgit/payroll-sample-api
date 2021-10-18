using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Api.APIModels
{
    public class BasicResponse
    {
        public BasicResponse()
        {
            this.Success = true;
            this.ErrorMessage = string.Empty;
            this.ErrorCode = 0;
        }
        public dynamic Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}
