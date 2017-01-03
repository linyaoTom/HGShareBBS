﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HGShare.DataProvider.DapperHelper;
using HGShare.Model;
using HGShare.VWModel;

namespace HGShare.DataProvider
{
    /// <summary>
    /// Comment 
    /// </summary>
    public class Comments
    {

        /// <summary>
        /// 添加CommentInfo
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static long AddComment(CommentInfo comment)
        {
            string sql = @"INSERT INTO [Comment]
			([AId],[UserId],[Content],[IP],[UserAgent],[State])
			VALUES
			(@AId,@UserId,@Content,@IP,@UserAgent,@State) 
			SELECT SCOPE_IDENTITY()
			";
            var par = new DynamicParameters();
            par.Add("@AId", comment.AId, DbType.Int64);
            par.Add("@UserId", comment.UserId, DbType.Int32);
            par.Add("@Content", comment.Content, DbType.String);
            par.Add("@IP", comment.IP, DbType.AnsiString);
            par.Add("@UserAgent", comment.UserAgent, DbType.AnsiString);
            par.Add("@State", comment.State, DbType.Int16);
            return DapWrapper.InnerQueryScalarSql<long>(DbConfig.ArticleManagerConnString, sql, par);
        }
        /// <summary>
        /// 修改CommentInfo
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static int UpdateComment(CommentInfo comment)
        {
            string sql = @"UPDATE  [Comment] SET 
						AId=@AId,
						UserId=@UserId,
						CreateTime=@CreateTime,
						Content=@Content,
						IP=@IP,
						UserAgent=@UserAgent,
						State=@State,
						RefuseReason=@RefuseReason,
						IsDelete=@IsDelete,
						LastEditUserId=@LastEditUserId,
						LastEditTime=@LastEditTime,
						IsStick=@IsStick,
						DianZanNum=@DianZanNum
 WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", comment.Id, DbType.Int64);
            par.Add("@AId", comment.AId, DbType.Int64);
            par.Add("@UserId", comment.UserId, DbType.Int32);
            par.Add("@CreateTime", comment.CreateTime, DbType.Date);
            par.Add("@Content", comment.Content, DbType.String);
            par.Add("@IP", comment.IP, DbType.AnsiString);
            par.Add("@UserAgent", comment.UserAgent, DbType.AnsiString);
            par.Add("@State", comment.State, DbType.Int16);
            par.Add("@RefuseReason", comment.RefuseReason, DbType.String);
            par.Add("@IsDelete", comment.IsDelete, DbType.Boolean);
            par.Add("@LastEditUserId", comment.LastEditUserId, DbType.Int32);
            par.Add("@LastEditTime", comment.LastEditTime, DbType.DateTime);
            par.Add("@IsStick", comment.IsStick, DbType.Boolean);
            par.Add("@DianZanNum", comment.DianZanNum, DbType.Int32);
            return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par);
        }
        /// <summary>
        /// 根据id获取CommentInfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CommentInfo GetCommentInfo(long id)
        {
            string sql = "select [Id],[AId],[UserId],[CreateTime],[Content],[IP],[UserAgent],[State],[RefuseReason],[IsDelete],[LastEditUserId],[LastEditTime],[IsStick],[DianZanNum] FROM [Comment] WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", id, DbType.Int64);
            return DapWrapper.InnerQuerySql<CommentInfo>(DbConfig.ArticleManagerConnString, sql, par).FirstOrDefault();
        }
        /// <summary>
        /// 根据id删除Comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Int32 DeleteComment(long id)
        {
            string sql = "DELETE [Comment] WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", id, DbType.Int64);
            return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par);
        }
        /// <summary>
        /// 根据ids删除Comment多条记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static Int32 DeleteComments(long[] ids)
        {
            if (ids.Length == 0)
                return 0;
            string sql = "DELETE [Comment] WHERE Id IN (" + string.Join(",", ids) + ")";
            return DapWrapper.InnerExecuteText(DbConfig.ArticleManagerConnString, sql);
        }
        /// <summary>
        /// 获取Comment分页列表(自定义存储过程)
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns>Comment列表</returns>
        public static List<CommentInfo> GetCommentPageList(int pageIndex, int pageSize, DateTime? beginTime, DateTime? endTime, out int recordCount)
        {
            recordCount = 0;
            var par = new DynamicParameters();
            par.Add("@PageIndex", pageIndex, DbType.Int32);
            par.Add("@PageSize", pageSize, DbType.Int32);
            par.Add("@BeginTime", beginTime, DbType.DateTime);
            par.Add("@EndTime", !endTime.HasValue ? endTime : endTime.Value.AddDays(1).AddMilliseconds(-1), DbType.DateTime);
            par.Add("@TotalCount", recordCount, DbType.Int32, ParameterDirection.Output);
            var result = DapWrapper.InnerQueryProc<CommentInfo>(DbConfig.ArticleManagerConnString, "proc_GetCommentPageList", par);
            recordCount = par.Get<int>("@TotalCount");
            return result;
        }
        /// <summary>
        /// 获取Comment分页列表(分页存储过程)
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageCount">页数</param>
        /// <param name="count">总记录数</param>
        /// <returns>Comment列表</returns>
        public static List<CommentVModel> GetCommentPageList(int pageIndex, int pageSize, DateTime? beginTime,
            DateTime? endTime, out int pageCount, out int count)
        {
            const string fieldKey = "Id";
            const string fieldShow = "[Id],[AId],[UserId],[CreateTime],[Content],[IP],[UserAgent],[State],[RefuseReason],[IsDelete],[LastEditUserId],[LastEditTime],[IsStick],[DianZanNum],dbo.GetNickNameByUserId(UserId) AS NickName,dbo.GetArticleTitleById(AId) AS ArticleTitle";
            const string fieldOrder = "Id desc";
            const string @where = "";
            return Paging<CommentVModel>.GetPageList("[Comment]", fieldKey, fieldShow, fieldOrder, where, pageIndex, pageSize, out pageCount, out count).ToList();
        }

        #region 前端

        /// <summary>
        /// 获取Comment分页列表(自定义存储过程)
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="order"></param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="aId"></param>
        /// <param name="authorId"></param>
        /// <returns>Comment列表</returns>
        public static List<CommentVModel> GetCommentPageList(int pageIndex, int pageSize, long aId, int authorId, string order, out int recordCount)
        {
            if (string.IsNullOrEmpty(order) || (order.ToLower() != "asc" || order.ToLower() != "desc"))
                order = "ASC";
            recordCount = 0;
            var par = new DynamicParameters();
            par.Add("@PageIndex", pageIndex, DbType.Int32);
            par.Add("@PageSize", pageSize, DbType.Int32);
            par.Add("@AId", aId, DbType.Int64);
            par.Add("@Order", order, DbType.String);
            par.Add("@AuthorId", authorId, DbType.Int32);
            par.Add("@TotalCount", recordCount, DbType.Int32, ParameterDirection.Output);
            var result = DapWrapper.InnerQueryProc<CommentVModel>(DbConfig.ArticleManagerConnString, "proc_GetCommentPageList", par);
            recordCount = par.Get<int>("@TotalCount");
            return result;
        }

        /// <summary>
        /// 获取Comment分页列表(自定义存储过程)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">-1所有 0待审核 1 已通过 2未通过..</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <returns>Comment列表</returns>
        public async static Task<IEnumerable<CommentVModel>> SearchCommentsByUserId(int userId, int state, int pageIndex, int pageSize)
        {
            var par = new DynamicParameters();
            par.Add("@PageIndex", pageIndex, DbType.Int32);
            par.Add("@PageSize", pageSize, DbType.Int32);
            par.Add("@UserId", userId, DbType.Int32);
            par.Add("@State", state, DbType.Int32);
            par.Add("@TotalCount", 0, DbType.Int32, ParameterDirection.Output);
            var result =await DapWrapper.InnerQueryProcAsync<CommentVModel>(DbConfig.ArticleManagerConnString, "proc_GetCommentPageListByUserId", par);
            return result;
        }
        /// <summary>
        /// 获取Comment分页列表(自定义存储过程)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">-1所有 0待审核 1 已通过 2未通过..</param>
        /// <returns>Comment列表</returns>
        public async static Task<int> SearchCommentsCountByUserId(int userId, int state)
        {
            var par = new DynamicParameters();
            par.Add("@UserId", userId, DbType.Int32);
            par.Add("@State", state, DbType.Int32);
            var result = await DapWrapper.InnerQueryScalarProcAsync<int>(DbConfig.ArticleManagerConnString, "proc_GetCommentCountByUserId", par);
            return result;
        }
        /// <summary>
        /// 获取用户最后评论时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DateTime GetLastPostTime(int userId)
        {
            string sql = @"SELECT MAX(CreateTime)
                          FROM [dbo].[Comment]
                          WHERE UserId=@UserId";
            var par = new DynamicParameters();
            par.Add("@UserId", userId, DbType.Int32);
            return DapWrapper.InnerQueryScalarSql<DateTime>(DbConfig.ArticleManagerConnString, sql, par);
        }

        /// <summary>
        /// 修改评论点赞数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num">+-1</param>
        /// <returns></returns>
        public static bool UpdateDianZanNum(long id,int num)
        {
            string sql = @"UPDATE  [Comment] SET 
						DianZanNum=DianZanNum+@DianZanNum
                            WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", id, DbType.Int64);
            par.Add("@DianZanNum", num, DbType.Int32);
            return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par)>0;
        }
        #endregion
        /// <summary>
        /// 更新评论状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool UpdateState(long id, int userId, bool state, string reason = null)
        {
            string sql = @"UPDATE  [Comment] SET 
						State=@State,
						LastEditUserId=@LastEditUserId,
                        RefuseReason=@RefuseReason,
                        LastEditTime=GETDATE()
                        WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", id, DbType.Int64);
            par.Add("@State", state ? 1 : 2, DbType.Int16);
            par.Add("@RefuseReason", reason, DbType.String);
            par.Add("@LastEditUserId", userId, DbType.Int32);
            return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par) > 0;
        }
        /// <summary>
        /// 更新评论状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool UpdateState(long[] ids, int userId, bool state, string reason = null)
        {
            if (ids.Length == 0)
                return false;
            string sql = string.Format(@"UPDATE  [Comment] SET 
						State=@State,
						LastEditUserId=@LastEditUserId,
                        RefuseReason=@RefuseReason,
                        LastEditTime=GETDATE()
                        WHERE Id IN ({0})", string.Join(",", ids));
            var par = new DynamicParameters();
            par.Add("@State", state ? 1 : 2, DbType.Int16);
            par.Add("@RefuseReason", reason, DbType.String);
            par.Add("@LastEditUserId", userId, DbType.Int32);
            return DapWrapper.InnerExecuteSql(DbConfig.ArticleManagerConnString, sql, par) > 0;
        }
        /// <summary>
        /// 根据id获取CommentVModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CommentVModel GetComment(long id)
        {
            string sql = "select [Id],[AId],[UserId],[CreateTime],[Content],[IP],[UserAgent],[State],[RefuseReason],[IsDelete],[LastEditUserId],[LastEditTime],[IsStick],[DianZanNum],dbo.GetNickNameByUserId(UserId) AS NickName,dbo.GetArticleTitleById(AId) AS ArticleTitle FROM [Comment] WHERE Id=@Id";
            var par = new DynamicParameters();
            par.Add("@Id", id, DbType.Int64);
            return DapWrapper.InnerQuerySql<CommentVModel>(DbConfig.ArticleManagerConnString, sql, par).FirstOrDefault();
        }
        #region es索引数据使用
        /// <summary>
        /// 获取Comments
        /// </summary>
        /// <returns>Comment列表</returns>
        public static List<CommentVModel> GetAllData()
        {
            const string sql = "SELECT [Id],[AId],[UserId],[CreateTime],[Content],[IP],[UserAgent],[State],[RefuseReason],[IsDelete],[LastEditUserId],[LastEditTime],[IsStick],[DianZanNum],dbo.GetNickNameByUserId(UserId) AS NickName,dbo.GetArticleTitleById(AId) AS ArticleTitle FROM [Comment]";
            return DapWrapper.InnerQuerySql<CommentVModel>(DbConfig.ArticleManagerConnString, sql);
        }
        #endregion
    }
}
