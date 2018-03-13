using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Xml;

namespace Founder.HisService
{
    /// <summary>
    /// 心电图接口
    /// </summary>
    public class ECG
    {
        /// <summary>
        /// 获取申请单
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int PACS_REQ(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "XD_PACS_REQ";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@hosp_patient_id", SqlDbType.VarChar, 12), //患者ID号                
                    new SqlParameter("@begin_date", SqlDbType.DateTime), //日期起始
                    new SqlParameter("@end_date", SqlDbType.DateTime), //日期结束
                    new SqlParameter("@exam_class", SqlDbType.VarChar, 12), //申请类型代码
                    new SqlParameter("@exec_unit", SqlDbType.VarChar, 7) //执行科室代码
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@hosp_patient_id",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/hosp_patient_id")));


                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@begin_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/begin_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期起始格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@end_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/end_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期结束格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/begin_date"))) >
                    DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/end_date"))))
                {
                    responseData = "开始日期不允许大于结束日期。";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@exam_class",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_class")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exec_unit",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exec_unit")));

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// 申请单执行(确认)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int PACS_CONFIRM(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "XD_PACS_CONFIRM";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@appoint_id", SqlDbType.VarChar, 30), //门诊：检查申请单号；住院：医嘱号                
                    new SqlParameter("@hosp_patient_id", SqlDbType.VarChar, 12), //患者ID号           
                    new SqlParameter("@patient_type", SqlDbType.VarChar, 12), //门诊住院病人类型  
                    new SqlParameter("@charge_status", SqlDbType.VarChar, 1), //费用状态
                    new SqlParameter("@exam_class", SqlDbType.VarChar, 6), //项目大类代码
                    new SqlParameter("@exam_item", SqlDbType.VarChar, 6), //项目子类代码
                    new SqlParameter("@exam_name", SqlDbType.VarChar, 32), //项目名称
                    new SqlParameter("@exam_cost", SqlDbType.VarChar, 12), //费用
                    new SqlParameter("@confirm_opera", SqlDbType.VarChar, 5), //确认医生工号
                    new SqlParameter("@confirm_time", SqlDbType.DateTime), //确认时间                
                    new SqlParameter("@confirm_win", SqlDbType.Int) //确认窗口号
                };
                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #region 为空判断

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/appoint_id")) == "")
                {
                    responseData = "申请单号不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/hosp_patient_id")) == "")
                {
                    responseData = "病人唯一编号不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "")
                {
                    responseData = "病人类型不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_opera")) == "")
                {
                    responseData = "确认医生工号不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if
                    (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_time")) == "")
                {
                    responseData = "确认时间不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #endregion

                #region 设置参数值

                if ((XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "2") ||
                    (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "4")
                ) //门诊病人
                {
                }
                else if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "1") //住院病人
                {
                }
                else
                {
                    responseData = "病人类型异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@appoint_id",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/appoint_id")));
                SetSqlParameters.SetValue(ref sqlParameter, "@hosp_patient_id",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/hosp_patient_id")));
                SetSqlParameters.SetValue(ref sqlParameter, "@patient_type",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")));
                SetSqlParameters.SetValue(ref sqlParameter, "@charge_status",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/charge_status")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_class",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_class")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_item",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_item")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_name",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_name")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_cost",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_cost")));
                SetSqlParameters.SetValue(ref sqlParameter, "@confirm_opera",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_opera")));


                DateTime confirm_time;
                if (!DateTime.TryParse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_time")),
                    out confirm_time))
                {
                    confirm_time = DateTime.Now;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@confirm_time",
                    confirm_time.ToString("yyyy-MM-dd HH:mm:ss"));
                SetSqlParameters.SetValue(ref sqlParameter, "@confirm_win",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_win")));

                #endregion

                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// 申请单取消确认
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int PACS_CANCEL(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "XD_PACS_CANCEL";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@appoint_id", SqlDbType.VarChar, 30), //门诊：检查申请单号；住院：医嘱号                
                    new SqlParameter("@hosp_patient_id", SqlDbType.VarChar, 12), //患者ID号  
                    new SqlParameter("@patient_type", SqlDbType.VarChar, 1), //门诊住院病人类型  
                    new SqlParameter("@opera", SqlDbType.VarChar, 12), //取消操作员代码
                    new SqlParameter("@cancel_time", SqlDbType.DateTime), //取消时间
                    new SqlParameter("@cancel_win_no", SqlDbType.VarChar, 12), //取消窗口
                    new SqlParameter("@exam_class", SqlDbType.VarChar, 12), //项目大类代码
                    new SqlParameter("@exam_item", SqlDbType.VarChar, 20), //项目代码
                    new SqlParameter("@exam_name", SqlDbType.VarChar, 20) //项目名称
                };
                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #region 为空判断

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/appoint_id")) == "")
                {
                    responseData = "申请单号不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/hosp_patient_id")) == "")
                {
                    responseData = "病人唯一编号不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "")
                {
                    responseData = "病人类型不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());

                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/opera")) == "")
                {
                    responseData = "取消操作员不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/cancel_time")) == "")
                {
                    responseData = "取消时间不能为空!";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #endregion

                #region 设置参数值

                if ((XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "2") ||
                    (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "4")) //门诊病人
                {
                }
                else if (XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")) == "1") //住院病人
                {
                }
                else
                {
                    responseData = "病人类型异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@appoint_id",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/appoint_id")));
                SetSqlParameters.SetValue(ref sqlParameter, "@hosp_patient_id",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/hosp_patient_id")));
                SetSqlParameters.SetValue(ref sqlParameter, "@patient_type",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patient_type")));
                SetSqlParameters.SetValue(ref sqlParameter, "@opera",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/opera")));

                DateTime confirm_time;
                if (!DateTime.TryParse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/cancel_time")),
                    out confirm_time))
                {
                    confirm_time = DateTime.Now;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@cancel_time",
                    confirm_time.ToString("yyyy-MM-dd HH:mm:ss"));

                SetSqlParameters.SetValue(ref sqlParameter, "@cancel_win_no",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/cancel_win_no")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_class",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_class")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_item",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_item")));
                SetSqlParameters.SetValue(ref sqlParameter, "@exam_name",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/exam_name")));

                #endregion

                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// Lis字典同步
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int Lis_Dictionary(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "sp_lis_dictionary";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@code", SqlDbType.VarChar, 2) //字典类型                   
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@code",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/code")));

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// Lis病人信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int Lis_Patient(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "sp_lis_getpatinfo";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@pattype", SqlDbType.VarChar, 2), //病人来源(1 门诊，2急诊，3 住院，4 体检)                
                    new SqlParameter("@patno", SqlDbType.VarChar, 20) //病历号   
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@pattype",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/pattype")));
                SetSqlParameters.SetValue(ref sqlParameter, "@patno",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patno")));

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// Lis病区医嘱
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int Lis_Order(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "sp_lisgetorders";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ward_sn", SqlDbType.VarChar, 7), //病区代码                
                    new SqlParameter("@b_date", SqlDbType.DateTime), //日期起始
                    new SqlParameter("@e_date", SqlDbType.DateTime) //日期结束  
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@ward_sn",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/ward_sn")));


                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@b_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/b_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期起始格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@e_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/e_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期结束格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/b_date"))) >
                    DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/e_date"))))
                {
                    responseData = "开始日期不允许大于结束日期。";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }


        /// <summary>
        /// Lis获取申请单
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int Lis_Mzcharge(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "sp_lisgetoutpfee";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@p_bar", SqlDbType.VarChar, 20), //病历号                
                    new SqlParameter("@b_date", SqlDbType.DateTime), //日期起始
                    new SqlParameter("@e_date", SqlDbType.DateTime) //日期结束    
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@p_bar",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/p_bar")));


                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@b_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/b_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期起始格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                try
                {
                    SetSqlParameters.SetValue(ref sqlParameter, "@e_date",
                        DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/e_date"))));
                }
                catch (Exception)
                {
                    responseData = "日期结束格式有误";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                if (DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/b_date"))) >
                    DateTime.Parse(XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/e_date"))))
                {
                    responseData = "开始日期不允许大于结束日期。";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }

        /// <summary>
        /// Lis确认记费
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="businessType"></param>
        /// <param name="requestData"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int Lis_Confirm(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "sp_lischarge";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@pattype", SqlDbType.VarChar, 2), //病人来源(1 门诊，2急诊，3 住院，4 体检)                
                    new SqlParameter("@reqno", SqlDbType.VarChar, 50), //申请号
                    new SqlParameter("@patid", SqlDbType.VarChar, 50), //病历号
                    new SqlParameter("@patname", SqlDbType.VarChar, 50), //姓名
                    new SqlParameter("@charge_no",
                        SqlDbType.BigInt), //当初组件返回的项目信息时带的recordid，这唯一确定一条医嘱或者一条申请项目记录的字串，LIS自行开单为项目代码
                    new SqlParameter("@is_commit", SqlDbType.VarChar, 1), //是否提交事务: 0 不提交，1 提交
                    new SqlParameter("@confirmer", SqlDbType.VarChar, 5), //确认人/计价人工号
                    new SqlParameter("@amount", SqlDbType.Int), //计价数量: 默认1
                    new SqlParameter("@confirm_dept", SqlDbType.VarChar, 7), //计价科室 可以设定默认科室和仪器对应科室
                    new SqlParameter("@charge_type", SqlDbType.VarChar,
                        20) //计价类别(为空表示一般检验项目计价,'TUBE'则需要存储过程根据@5项目类别处理相应的附加项目信息.如试管费,采血费等,'BARCODE_ADD'表示增加条码,'BARCODE_DEL'表示取消或作废条码)
                };


                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }


                #region 设置参数值

                SetSqlParameters.SetValue(ref sqlParameter, "@pattype",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/pattype")));
                SetSqlParameters.SetValue(ref sqlParameter, "@reqno",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/reqno")));
                SetSqlParameters.SetValue(ref sqlParameter, "@patid",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patid")));
                SetSqlParameters.SetValue(ref sqlParameter, "@patname",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/patname")));
                SetSqlParameters.SetValue(ref sqlParameter, "@charge_no",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/charge_no")));
                SetSqlParameters.SetValue(ref sqlParameter, "@is_commit",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/is_commit")));
                SetSqlParameters.SetValue(ref sqlParameter, "@confirmer",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirmer")));
                SetSqlParameters.SetValue(ref sqlParameter, "@amount",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/amount")));
                SetSqlParameters.SetValue(ref sqlParameter, "@confirm_dept",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/confirm_dept")));
                SetSqlParameters.SetValue(ref sqlParameter, "@charge_type",
                    XmlGetValueHelper.GetValue(xmlDocument.SelectSingleNode("/root/charge_type")));

                #endregion


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }


        /// <summary>
        /// 通用接口
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static int GENERAL(LogMessage logMessage, out string responseData)
        {
            try
            {
                string sql = "XD_GENERALL";
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@XML", SqlDbType.VarChar, 20) //XML参数
                };
                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.LoadXml(logMessage.requestData);
                }
                catch (Exception ex)
                {
                    responseData = "参数requestData:" + ex.Message;
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                string xml = logMessage.businessType.ToString();

                foreach (XmlNode item in xmlDocument.SelectSingleNode("/root").ChildNodes)
                {
                    xml = xml + "|" + item.InnerText;
                }

                SetSqlParameters.SetValue(ref sqlParameter, "@XML", xml);


                DataSet dataSet = SqlHelper.ExecuteDataset(ConfigContext.GetConfigContext().config.DataBaseConfig,
                    CommandType.StoredProcedure, sql, sqlParameter);
                if (dataSet != null)
                {
                    if ((dataSet.Tables.Count != 0) && (dataSet.Tables[0].Rows.Count != 0))
                    {
                        if (dataSet.Tables[0].Rows[0][0].ToString() != "")
                        {
                            responseData = dataSet.Tables[0].Rows[0][0].ToString();
                            return 0;
                        }
                        else
                        {
                            responseData = "没有数据";
                            logMessage.responseData = responseData;
                            Log4NetHelper.Info<string>(logMessage.ToString());
                            return -1;
                        }
                    }

                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }
                else
                {
                    responseData = "服务返回数据异常";
                    logMessage.responseData = responseData;
                    Log4NetHelper.Info<string>(logMessage.ToString());
                    return -1;
                }

                ;
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
                //
            }
        }
    }
}