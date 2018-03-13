using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Founder.HisService
{
    public class SetSqlParameters
    {
       /// <summary>
       /// 动态设置参数值
       /// </summary>
       /// <param name="SqlParameters"></param>
       /// <param name="SqlParameterName"></param>
       /// <param name="Value"></param>
        public static void SetValue(ref SqlParameter[] SqlParameters, string SqlParameterName,object Value)
        {
            if (SqlParameters != null) {

                foreach (SqlParameter item in SqlParameters)
                {
                    
                    if (item.ParameterName == SqlParameterName)
                    {
                        item.Value = Value;
                        break;
                    }
                }
            }
          
        }
    }
}
