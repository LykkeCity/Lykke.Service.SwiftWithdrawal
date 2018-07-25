﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    /// <summary>
    /// User swift data
    /// </summary>
    public class SwiftDataModel
    {
        public string Bic { get; set; }
        public string AccNumber { get; set; }
        public string AccName { get; set; }
        public string AccHolderAddress { get; set; }
        public string BankName { get; set; }

        public string AccHolderCountry { get; set; }
        public string AccHolderCountryCode { get; set; }
        public string AccHolderZipCode { get; set; }
        public string AccHolderCity { get; set; }
    }
}
