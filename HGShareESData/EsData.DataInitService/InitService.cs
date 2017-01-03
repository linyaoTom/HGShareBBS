﻿using System;
using System.ServiceProcess;
using EsData.Business.DataIndex;
using EsData.Utils.Log;

namespace EsData.DataInitService
{
    public partial class InitService : ServiceBase
    {
        private ILog _log;
        private InitTaskManager _initTaskManager;
        public InitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _log = new Log4Net("logger");

            _log.Info("服务开启启动...");
            try
            {
                _initTaskManager = new InitTaskManager(_log);
                _initTaskManager.Start();
            }
            catch (Exception ex)
            {
                _log.Error("服务异常", ex);
            }
            _log.Info("服务启动成功...");
        }

        protected override void OnStop()
        {
            try
            {
                _log.Info("服务开始停止...");
                _initTaskManager.Stop();
                _log.Info("服务停止成功...");
            }
            catch (Exception ex)
            {
                _log.Error("服务停止异常", ex);
            }


        }
    }
}
