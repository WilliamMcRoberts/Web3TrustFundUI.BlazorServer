using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class GlobalConfig
    {
        private readonly IConfiguration _config;

        public GlobalConfig(IConfiguration config)
        {
            _config = config;
        }
    }
}
