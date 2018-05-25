using System.Threading.Tasks;

namespace Lykke.Service.SwiftWithdrawal.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
