using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Payroll_Api.Exceptions
{
    [Serializable]
    public class PayrollException : Exception
    {
        public PayrollException()
      : base() { }

        public PayrollException(ErrorMessages message)
            : base(message.ToString()) { }

        public PayrollException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public PayrollException(string message, Exception innerException)
            : base(message, innerException) { }

        public PayrollException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected PayrollException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
