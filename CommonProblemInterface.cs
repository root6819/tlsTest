using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tlsTestN
{
    /// <summary>
    /// 常见问题接口
    /// </summary>
    public class CommonProblemInterface
    {
        public string error { get; set; }
        public string msg { get; set; }
        public List data { get; set; }

        public class List
        {
            public List<CommonProblem> problem { get; set; }
        }
    }
}
