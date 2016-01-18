using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Common.Tools
{
    /// <summary>
    /// 邮件发送类
    /// 使用方法：
    /// EmailModel eModel = new EmailModel()
    ///       {
    ///            From = "crayoncode@163.com",
    ///            Body = "<h2>这是一个测试实例!  " + DateTime.Now + "</h2>",
    ///            DisplayName = "Jackie",
    ///            Host = "smtp.163.com",
    ///            Subject = "测试",
    ///            To ="492118114@qq.com",
    ///            UserPwd = "",//自己发送邮箱的密码
    ///            UserID = "crayoncode@163.com"
    ///   };
            //发送邮件
     ///    new MailTool().Send(eModel);
    /// </summary>
    public class MailTool
    {
        #region 构造方法
        public MailTool()
        {

        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 电邮发送
        /// </summary>
        public void Send(EmailModel model)
        {
            //初始化邮件信息
            MailMessage mm = new MailMessage(model.From, model.To, model.Subject, model.Body);
            mm.IsBodyHtml = true;
            mm.SubjectEncoding = Encoding.UTF8;
            mm.BodyEncoding = Encoding.UTF8;
            //SMTP服务器
            SmtpClient stmp = new SmtpClient(model.Host);
            stmp.DeliveryMethod = SmtpDeliveryMethod.Network;
            stmp.Credentials = new NetworkCredential(model.UserID, model.UserPwd);
            //发送邮件
            try
            {
                stmp.Send(mm);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }

    public class EmailModel
    {
        #region 邮件相关信息
        /// <summary>
        /// 发件人电邮主机信息
        /// </summary>
        public string Host
        {
            get;
            set;
        }
        /// <summary>
        /// 发件人电邮地址
        /// </summary>
        public string From
        {
            get;
            set;
        }
        /// <summary>
        /// 收件人电邮地址 多个收件使用逗号(,)分割
        /// </summary>
        public string To
        {
            get;
            set;
        }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject
        {
            get;
            set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Body
        {
            get;
            set;
        }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserID
        {
            get;
            set;
        }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string UserPwd
        {
            get;
            set;
        }
        /// <summary>
        /// 发送人标题信息
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }
        #endregion
    }
}
