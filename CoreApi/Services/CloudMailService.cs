using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = "admin@qq.com";
        private string _mailFrom = "noreply@alibaba.com";
        private readonly ILogger<CloudMailService> _logger;

        public CloudMailService(ILogger<CloudMailService> logger)
        {
            this._logger = logger;
        }

        public void Send(string subject, string msg)
        {
            this._logger.LogInformation($"从{_mailFrom}给{_mailTo}通过{nameof(CloudMailService)}发送了邮件");
        }
    }
}
