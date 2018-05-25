using System;
using Common.Log;

namespace Lykke.Service.SwiftWithdrawal.Client
{
    public class SwiftWithdrawalClient : ISwiftWithdrawalClient, IDisposable
    {
        private readonly ILog _log;

        public SwiftWithdrawalClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
