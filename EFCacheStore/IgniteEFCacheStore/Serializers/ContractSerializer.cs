using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core.Binary;
using Tim.DataAccess;

namespace IgniteEFCacheStore.Serializers
{
    public class ContractSerializer : IBinarySerializer
    {
        public void WriteBinary(object obj, IBinaryWriter writer)
        {
            var contract = (Contract)obj;
            writer.WriteTimestamp("AnnulDate", contract.AnnulDate.GetValueOrDefault().ToUniversalTime());
            writer.WriteTimestamp("CreationDateTime", contract.CreationDateTime.ToUniversalTime());
            writer.WriteTimestamp("LastChangeDateTime", contract.LastChangeDateTime.ToUniversalTime());
            writer.WriteTimestamp("ValidFrom", contract.ValidFrom.ToUniversalTime());
            writer.WriteTimestamp("ValidTo", contract.ValidTo.ToUniversalTime());
            writer.WriteInt("ID", contract.ID);
            writer.WriteInt("ContractStatusID", contract.ContractStatusID);
            writer.WriteInt("ContractorID", contract.ContractorID);
            writer.WriteInt("CreationUserId", contract.CreationUserId);
            writer.WriteInt("DistributorID", contract.DistributorID);
            writer.WriteInt("InvestDirectionID", contract.InvestDirectionID);
            writer.WriteInt("PaymentMethodID", contract.PaymentMethodID);
            writer.WriteInt("PaymentTermID", contract.PaymentTermID);
            writer.WriteInt("CreatedFromContractID", contract.CreatedFromContractID.GetValueOrDefault());
            writer.WriteInt("ImportContractID", contract.ImportContractID.GetValueOrDefault());
            writer.WriteInt("LastChangeUserID", contract.LastChangeUserID.GetValueOrDefault());
            writer.WriteInt("PurposeHO", contract.PurposeHO.GetValueOrDefault());
            writer.WriteString("Comment", contract.Comment);
            writer.WriteString("Num", contract.Num);
            writer.WriteDecimal("BonusPercent", contract.BonusPercent);
            writer.WriteDecimal("InvestPercentPlan", contract.InvestPercentPlan);
            writer.WriteDecimal("InvestPercentTradeNetPlan", contract.InvestPercentTradeNetPlan);
            writer.WriteBoolean("IsAnnul", contract.IsAnnul);
            writer.WriteBoolean("IsAuthorized", contract.IsAuthorized);
            writer.WriteBoolean("IsHeinBank", contract.IsHeinBank);
            writer.WriteBoolean("IsTradeNet", contract.IsTradeNet);
        }

        public void ReadBinary(object obj, IBinaryReader reader)
        {
            var month = (Contract)obj;
            month.ID = reader.ReadInt("ID");
            month.ValidTo = reader.ReadTimestamp("ValidTo").GetValueOrDefault();
            month.ValidFrom = reader.ReadTimestamp("ValidFrom").GetValueOrDefault();
        }
    }
}
