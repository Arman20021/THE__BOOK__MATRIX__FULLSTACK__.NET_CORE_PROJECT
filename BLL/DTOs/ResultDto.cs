using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = "";
    }
}
