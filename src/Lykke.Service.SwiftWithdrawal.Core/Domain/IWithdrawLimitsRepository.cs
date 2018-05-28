using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain
{
    public interface IWithdrawLimit
    {
        string AssetId { get; }
        double LimitAmount { get; }
    }

    public interface IWithdrawLimitsRepository
    {
        Task<IEnumerable<IWithdrawLimit>> GetDataAsync();
        Task<bool> AddAsync(IWithdrawLimit item);
        Task<bool> DeleteAsync(string assetId);
        Task<double> GetLimitByAssetAsync(string assetId);
        double DefaultWithdrawalLimit { get; }
    }
}
