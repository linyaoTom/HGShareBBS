using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using HGShare.DataProvider.DapperHelper;
using HGShare.Model;
namespace HGShare.DataProvider
{    
	/// <summary>
    /// DianZanLog 
    /// </summary>
    public class DianZanLogs
    {

		/// <summary>
		/// 添加DianZanLogInfo
		/// </summary>
		/// <param name="dianzanlog"></param>
		/// <returns></returns>
		public static long AddDianZanLog(DianZanLogInfo dianzanlog)
		{
			string sql = @"INSERT INTO [DianZanLog]
			([MId],[CId],[UserId],[Ip],[Type])
			VALUES
			(@MId,@CId,@UserId,@Ip,@Type) 
			SELECT SCOPE_IDENTITY()
			";
			var par = new DynamicParameters();
			par.Add("@MId",dianzanlog.MId , DbType.Int64);
			par.Add("@CId",dianzanlog.CId , DbType.Int64);
			par.Add("@UserId",dianzanlog.UserId , DbType.Int32);
			par.Add("@Ip",dianzanlog.Ip , DbType.AnsiString);
			par.Add("@Type",dianzanlog.Type , DbType.Int16);
			return DapWrapper.InnerQueryScalarSql<long>(DbConfig.ArticleManagerConnString, sql, par);
		}
		/// <summary>
		/// 修改DianZanLogInfo
		/// </summary>
		/// <param name="dianzanlog"></param>
		/// <returns></returns>
		public static int UpdateDianZanLog(DianZanLogInfo dianzanlog)
		{
			string sql = @"UPDATE  [DianZanLog] SET 
						MId=@MId,
						CId=@CId,
						UserId=@UserId,
						Ip=@Ip,
						IsCancel=@IsCancel,
						CancelTime=@CancelTime,
						Type=@Type,
						CreateTime=@CreateTime
 WHERE Id=@Id";
			var par = new DynamicParameters();
			par.Add("@Id", dianzanlog.Id, DbType.Int64);
			par.Add("@MId",dianzanlog.MId , DbType.Int64);
			par.Add("@CId",dianzanlog.CId , DbType.Int64);
			par.Add("@UserId",dianzanlog.UserId , DbType.Int32);
			par.Add("@Ip",dianzanlog.Ip , DbType.AnsiString);
			par.Add("@IsCancel",dianzanlog.IsCancel , DbType.Boolean);
			par.Add("@CancelTime",dianzanlog.CancelTime , DbType.DateTime);
			par.Add("@Type",dianzanlog.Type , DbType.Int16);
			par.Add("@CreateTime",dianzanlog.CreateTime , DbType.DateTime);
			return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par);
		}
		/// <summary>
		/// 根据id获取DianZanLogInfo
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static DianZanLogInfo GetDianZanLogInfo(long id)
		{
			string sql = "select [Id],[MId],[CId],[UserId],[Ip],[IsCancel],[CancelTime],[Type],[CreateTime] FROM [DianZanLog] WHERE Id=@Id";
			var par = new DynamicParameters();
			par.Add("@Id", id, DbType.Int64);
			return DapWrapper.InnerQuerySql<DianZanLogInfo>(DbConfig.ArticleManagerConnString, sql, par).FirstOrDefault();
		}
		/// <summary>
		/// 根据id删除DianZanLog
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Int32 DeleteDianZanLog(long id)
		{
			string sql="DELETE [DianZanLog] WHERE Id=@Id";
			var par = new DynamicParameters();
			par.Add("@Id", id, DbType.Int64);
			return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par);
		}
		/// <summary>
		/// 根据ids删除DianZanLog多条记录
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public static Int32 DeleteDianZanLogs(long[] ids)
		{
			string sql="DELETE [DianZanLog] WHERE Id IN ("+string.Join(",",ids)+")";
			return DapWrapper.InnerExecuteText(DbConfig.ArticleManagerConnString, sql);
		}
		/// <summary>
       /// 获取DianZanLog分页列表(自定义存储过程)
       /// </summary>
       /// <param name="pageIndex">页码</param>
       /// <param name="pageSize">每页显示条数</param>
       /// <param name="beginTime">开始时间</param>
       /// <param name="endTime">结束时间</param>
       /// <param name="recordCount">总记录数</param>
       /// <returns>DianZanLog列表</returns>
       public static List<DianZanLogInfo> GetDianZanLogPageList(int pageIndex, int pageSize, DateTime? beginTime, DateTime? endTime, out int recordCount)
       {
           recordCount = 0;
           var par = new DynamicParameters();
           par.Add("@PageIndex", pageIndex, DbType.Int32);
           par.Add("@PageSize", pageSize, DbType.Int32);
           par.Add("@BeginTime", beginTime, DbType.DateTime);
           par.Add("@EndTime", !endTime.HasValue ? endTime : endTime.Value.AddDays(1).AddMilliseconds(-1), DbType.DateTime);
           par.Add("@TotalCount", recordCount, DbType.Int32, ParameterDirection.Output);
           var result = DapWrapper.InnerQueryProc<DianZanLogInfo>(DbConfig.ArticleManagerConnString, "proc_GetDianZanLogPageList", par);
           recordCount = par.Get<int>("@TotalCount");
           return result;
       }
	   /// <summary>
       /// 获取DianZanLog分页列表(分页存储过程)
       /// </summary>
       /// <param name="pageIndex">页码</param>
       /// <param name="pageSize">每页显示条数</param>
       /// <param name="beginTime">开始时间</param>
       /// <param name="endTime">结束时间</param>
       /// <param name="pageCount">页数</param>
       /// <param name="count">总记录数</param>
       /// <returns>DianZanLog列表</returns>
       public static List<DianZanLogInfo> GetDianZanLogPageList(int pageIndex, int pageSize, DateTime? beginTime,
           DateTime? endTime, out int pageCount, out int count)
       {
           const string fieldKey = "Id";
           const string fieldShow = "[Id],[MId],[CId],[UserId],[Ip],[IsCancel],[CancelTime],[Type],[CreateTime]";
           const string fieldOrder = "Id desc";
           const string @where = "";
          return Paging<DianZanLogInfo>.GetPageList("[DianZanLog]", fieldKey, fieldShow, fieldOrder, where, pageIndex, pageSize, out pageCount, out count).ToList();
       }


	    /// <summary>
	    /// 更新取消状态
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="isCancel"></param>
	    /// <returns></returns>
	    public static bool UpdateIsCancel(long id, bool isCancel)
       {
           string sql = @"UPDATE  [DianZanLog] SET 
						IsCancel=@IsCancel,
                        CancelTime=GETDATE()
                        WHERE Id=@Id";
           var par = new DynamicParameters();
           par.Add("@Id", id, DbType.Int64);
           par.Add("@IsCancel", isCancel, DbType.Boolean);
           return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par)>0;
       }
        /// <summary>
        /// 检查是否已经点赞
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mId"></param>
        /// <param name="cId"></param>
        /// <returns></returns>
        public static long GetDianZanLogId(int userId, long mId, long cId)
        {
            string sql = "select [Id] FROM [DianZanLog] WHERE UserId=@UserId AND MId=@MId AND CId=@CId";
            var par = new DynamicParameters();
            par.Add("@UserId", userId, DbType.Int32);
            par.Add("@MId", mId, DbType.Int64);
            par.Add("@CId", cId, DbType.Int64);
            return DapWrapper.InnerQuerySql<long>(DbConfig.ArticleManagerConnString, sql, par).FirstOrDefault();
        }
        /// <summary>
        /// 获取用户所有点过赞的评论id
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="aId">文章id</param>
        /// <returns></returns>
        public static List<long> GetUserAllDianZanCommentId(int userId, long aId)
        {
            string sql = "select [CId] FROM [DianZanLog] WHERE UserId=@UserId AND MId=@MId AND IsCancel=0";
            var par = new DynamicParameters();
            par.Add("@UserId", userId, DbType.Int32);
            par.Add("@MId", aId, DbType.Int64);
            return DapWrapper.InnerQuerySql<long>(DbConfig.ArticleManagerConnString, sql, par);
        }
    }
}
