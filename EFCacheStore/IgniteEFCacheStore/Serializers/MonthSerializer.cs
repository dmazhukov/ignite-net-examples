using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core.Binary;
using Tim.DataAccess;

namespace IgniteEFCacheStore.Serializers
{
    public class MonthSerializer:IBinarySerializer
    {
        public void WriteBinary(object obj, IBinaryWriter writer)
        {
            var month = (Month) obj;
            writer.WriteTimestamp("ActualDate", month.ActualDate.ToUniversalTime());
            writer.WriteTimestamp("CreationDate", month.CreationDate.ToUniversalTime());
            writer.WriteInt("ID", month.ID);
            writer.WriteString("Name", month.Name);
            writer.WriteInt("ActualMonth", month.ActualMonth.GetValueOrDefault());// needs fix, should not write if null
            writer.WriteInt("ActualYear", month.ActualYear.GetValueOrDefault());// same
        }

        public void ReadBinary(object obj, IBinaryReader reader)
        {
            var month = (Month)obj;
            month.ActualDate = reader.ReadTimestamp("ActualDate").GetValueOrDefault(); // todo: convert from UTC
            month.CreationDate = reader.ReadTimestamp("CreationDate").GetValueOrDefault();//same
            month.ID = reader.ReadInt("ID");
            month.Name = reader.ReadString("Name");
            month.ActualMonth = reader.ReadInt("ActualMonth");
            month.ActualYear = reader.ReadInt("ActualYear");
        }
    }
}
