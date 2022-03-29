using Nirast.Pcms.Api.Sdk.Entities;
using Nirast.Pcms.Api.Sdk.Infrastructure;
using Nirast.Pcms.Api.Sdk.Logger;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using static Nirast.Pcms.Api.Sdk.Entities.Enums;
using Nirast.Pcms.Api.Data.Helpers;
using System.IO;

namespace Nirast.Pcms.Ap.Application.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private IPCMSLogger _logger;

        public NotificationService(IPCMSLogger logger)
        {
            _logger = logger;
        }

        public async  Task<bool> SendEMail(EmailInput data, List<string> ccAdresses= null,List<string>emailIdList=null)
            {
            bool isMessageSent = false;
            try
            {
                Security security = new Security();
               string fromEmail = data?.EmailIdConfig?.FromEmail?.ToString();
               string encryptedFromEmailPassword = data?.EmailIdConfig?.Password?.ToString();
                //GetEmailConfig(data.EmailType,  out fromEmail, out encryptedFromEmailPassword);
                _logger.Info(fromEmail + " " + encryptedFromEmailPassword);
                int emailPort = Convert.ToInt32(data.EmailConfig.MailPort);
               bool enableSSL = Convert.ToBoolean(data.EmailConfig.SSL);
             
                //  bool enableSSL =true;

                // bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                string host = data?.EmailConfig?.MailHost?.ToString();
                string encryptionPassword = ConfigurationManager.AppSettings["EncryptPassword"].ToString();

                //string fromEmailPassword = security.DecryptCipherTextToPlainText(encryptedFromEmailPassword);
                string fromEmailPassword = StringCipher.Decrypt(encryptedFromEmailPassword, encryptionPassword);
                string ccEmail = ConfigurationManager.AppSettings["CcEmail"].ToString();
                string PaymentEmail = ConfigurationManager.AppSettings["PaymentEmail"].ToString();
                if (null==emailIdList)
                {
                    using (MailMessage mail = new MailMessage(fromEmail, data.EmailId))
                    {
                        mail.Subject = data.Subject;
                        mail.Body = data.Body;
                        var pdfLocalFileName = "Invoice";
                        var client = new System.Net.WebClient();

                        if (data.Attachments != null && data.Attachments != "" && data.Attachment == null)
                        {
                            if (data.Attachments.StartsWith("http://") && data.Attachments.EndsWith(".pdf"))
                            {
                                if (data.EmailType == EmailType.Invoice)
                                {
                                    Uri uriAddress = new Uri(data.Attachments);
                                    var localPath = ConfigurationManager.AppSettings["InvoiceEmailPath"].ToString() + uriAddress.LocalPath;
                                    Attachment datas = new Attachment(localPath, MediaTypeNames.Application.Octet);
                                    datas.ContentDisposition.FileName = Path.GetFileName(localPath);
                                    mail.Attachments.Add(datas);
                                  
                                    _logger.Info("starts with http" + " " + "local path=" + localPath + " " + "UriAddress=" + uriAddress );
                                }
                                else
                                {
                                    client.DownloadFile(data.Attachments, pdfLocalFileName);
                                    Attachment datas = new Attachment(pdfLocalFileName, MediaTypeNames.Application.Pdf);
                                    mail.Attachments.Add(datas);
                                    
                                }
                            }
                            else
                            {
                                Uri uriAddress = new Uri(data.Attachments);
                                var localPath = uriAddress.LocalPath;
                                string fileName = Path.GetFileName(localPath);
                                _logger.Info("Not starts with http"+" "+"local path="+localPath +" "+"UriAddress="+ uriAddress + " " + "filename=" + fileName);
                                byte[] pdfData = client.DownloadData(data.Attachments);

                                MemoryStream memoryStreamOfFile = new MemoryStream(pdfData);

                                mail.Attachments.Add(new System.Net.Mail.Attachment(memoryStreamOfFile, fileName, "application/pdf"));

                            }
                        }
                        else if (data.Attachment != null)
                        {
                            mail.Attachments.Add(data.Attachment);
                        }

                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient()
                        {
                            Host = host,
                            EnableSsl = enableSSL,
                            UseDefaultCredentials = true,
                            Credentials = new System.Net.NetworkCredential(fromEmail, fromEmailPassword)
                        };
                        smtp.Port = emailPort;
                        if (ccAdresses == null)
                        {
                            ccAdresses = new List<string>();
                        }
                        if (data.EmailType == EmailType.UserPayment)
                        {
                            ccAdresses.Add(PaymentEmail);
                        }
                        ccAdresses.Add(ccEmail);
                        if (ccAdresses != null && ccAdresses.Count > 0)
                        {
                            foreach (var addr in ccAdresses)
                            {
                                mail.CC.Add(addr);
                            }
                        }

                        //mail.CC.Add(fromEmail);
                        smtp.Send(mail);
                        isMessageSent = true;
                    }
                }
                else
                {
                    foreach (var item in emailIdList)
                    {
                        using (MailMessage mail = new MailMessage(fromEmail, item))
                        {
                            mail.Subject = data.Subject;
                            mail.Body = data.Body;
                            var pdfLocalFileName = "Invoice";
                            var client = new System.Net.WebClient();

                            if (data.Attachments != null && data.Attachments != "" && data.Attachment == null)
                            {
                                if (data.Attachments.StartsWith("http://") && data.Attachments.EndsWith(".pdf"))
                                {
                                    if (data.EmailType == EmailType.Invoice)
                                    {
                                        Uri uriAddress = new Uri(data.Attachments);
                                        var localPath = ConfigurationManager.AppSettings["InvoiceEmailPath"].ToString() + uriAddress.LocalPath;
                                        Attachment datas = new Attachment(localPath, MediaTypeNames.Application.Octet);
                                        datas.ContentDisposition.FileName = Path.GetFileName(localPath);
                                        mail.Attachments.Add(datas);
                                    }
                                    else
                                    {
                                        client.DownloadFile(data.Attachments, pdfLocalFileName);
                                        Attachment datas = new Attachment(pdfLocalFileName, MediaTypeNames.Application.Pdf);
                                        mail.Attachments.Add(datas);
                                    }
                                }
                                else
                                {
                                    Uri uriAddress = new Uri(data.Attachments);
                                    var localPath = uriAddress.LocalPath;
                                    string fileName = Path.GetFileName(localPath);

                                    byte[] pdfData = client.DownloadData(data.Attachments);

                                    MemoryStream memoryStreamOfFile = new MemoryStream(pdfData);

                                    mail.Attachments.Add(new System.Net.Mail.Attachment(memoryStreamOfFile, fileName, "application/pdf"));

                                }
                            }
                            else if (data.Attachment != null)
                            {
                                mail.Attachments.Add(data.Attachment);
                            }

                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient()
                            {
                                Host = host,
                                EnableSsl = enableSSL,
                                UseDefaultCredentials = true,
                                Credentials = new System.Net.NetworkCredential(fromEmail, fromEmailPassword)
                            };
                            smtp.Port = emailPort;
                            if (ccAdresses == null)
                            {
                                ccAdresses = new List<string>();
                            }
                            if (data.EmailType == EmailType.UserPayment)
                            {
                                ccAdresses.Add(PaymentEmail);
                            }
                            ccAdresses.Add(ccEmail);
                            if (ccAdresses != null && ccAdresses.Count > 0)
                            {
                                foreach (var addr in ccAdresses)
                                {
                                    mail.CC.Add(addr);
                                }
                            }

                            //mail.CC.Add(fromEmail);
                            smtp.Send(mail);


                            isMessageSent = true;
                        }
                    }
                }
            }
            
            catch (Exception ex)
            {
                isMessageSent = false;
                _logger.Error(ex, "Failed to Send mail");
            }
            return isMessageSent;
        }

        private static void GetEmailConfig(EmailType emailType, out string fromEmail, out string encryptedFromEmailPassword)
        {
            switch (emailType)
            {
                case EmailType.Invoice:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["InvoiceFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["InvoiceFromEmail"].ToString();
                        break;
                    }
                case EmailType.Registration:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["RegistrationFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["RegistrationFromEmail"].ToString();
                        break;
                    }
                case EmailType.Booking:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["BookinFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["BookingFromEmail"].ToString();
                        break;
                    }
                case EmailType.Scheduling:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["SchedulingFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["SchedulingFromEmail"].ToString();
                        break;
                    }
                case EmailType.UserPayment:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["UserPaymentFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["UserPaymentFromEmail"].ToString();
                        break;
                    }
                default:
                    {
                        encryptedFromEmailPassword = ConfigurationManager.AppSettings["DefaultFromEmailPassword"].ToString();
                        fromEmail = ConfigurationManager.AppSettings["DefaultFromEmail"].ToString();
                        break;
                    }
            }

        }
    }

}

