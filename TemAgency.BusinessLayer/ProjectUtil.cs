using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace TemAgency.BusinessLayer
{
    public class ProjectUtil
    {
        public ProjectUtil()
        {

        }
        static DataLayer layer = new DataLayer();
        //private string baseUrl = "https://localhost:44393/";
        private string baseUrl = "89.19.10.211:480";
        public bool SendEmailForProjectLawyer(int number)
        {
            string email = "burak.aktas@netline.net.tr";
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreementOnConfirm?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForProjectLawyerBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForProjectLawyerBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN 'AVUKAT ADI'");
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması için onayınız beklenmektedir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                if (string.IsNullOrEmpty(project.Explanation))
                {
                    body = body.Replace("{Explanation}", "");
                }
                else
                {
                    body = body.Replace("{Explanation}", "Açıklama: " + project.Explanation);
                }
                body = body.Replace("{LExplanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectRejectedLawyer(int number)
        {
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreement?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                string email = layer.GetUserById(project.UserId).Email;
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForRejectedProjectLawyerBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForRejectedProjectLawyerBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN " + layer.GetUserById(project.UserId).FullName);
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması 'AVUKAT ADI' tarafından reddedilmiştir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                if (string.IsNullOrEmpty(project.LExplanation))
                {
                    body = body.Replace("{LExplanation}", "");
                }
                else
                {
                    body = body.Replace("{LExplanation}", "Avukat Açıklaması: " + project.LExplanation);
                }
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectEditedLawyer(int number)
        {
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreement?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                string email = layer.GetUserById(project.UserId).Email;
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForEditedProjectLawyerBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForEditedProjectLawyerBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN " + layer.GetUserById(project.UserId).FullName);
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması için 'AVUKAT ADI' tarafından revize istenmiştir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                if (string.IsNullOrEmpty(project.LExplanation))
                {
                    body = body.Replace("{LExplanation}", "");
                }
                else
                {
                    body = body.Replace("{LExplanation}", "Avukat Açıklaması: " + project.LExplanation);
                }
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectAcceptedLawyer(int number)
        {
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreement?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                string email = layer.GetUserById(project.UserId).Email;
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForAcceptedProjectLawyerBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForAcceptedProjectLawyerBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN " + layer.GetUserById(project.UserId).FullName);
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması 'AVUKAT ADI' tarafından onaylanmıştır. 'FİNANS ADI' tarafından onay beklenmektedir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                if (string.IsNullOrEmpty(project.LExplanation))
                {
                    body = body.Replace("{LExplanation}", "");
                }
                else
                {
                    body = body.Replace("{LExplanation}", "Avukat Açıklaması: " + project.LExplanation);
                }
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectFinance(int number)
        {
            string email = "burak.aktas@netline.net.tr";
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreementOnConfirmFinance?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForProjectFinanceBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForProjectFinanceBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN 'FİNANS ADI'");
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması için onayınız beklenmektedir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                if (string.IsNullOrEmpty(project.Explanation))
                {
                    body = body.Replace("{Explanation}", "");
                }
                else
                {
                    body = body.Replace("{Explanation}", "Açıklama: " + project.Explanation);
                }
                if (string.IsNullOrEmpty(project.LExplanation))
                {
                    body = body.Replace("{LExplanation}", "");
                }
                else
                {
                    body = body.Replace("{LExplanation}", "Avukat Açıklaması: " + project.LExplanation);
                }
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectRejectedFinance(int number)
        {
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreement?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                string email = layer.GetUserById(project.UserId).Email;
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForRejectedProjectFinanceBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForRejectedProjectFinanceBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN " + layer.GetUserById(project.UserId).FullName);
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması 'FİNANS ADI' tarafından reddedilmiştir. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{LExplanation}", "");
                if (string.IsNullOrEmpty(project.FExplanation))
                {
                    body = body.Replace("{FExplanation}", "");
                }
                else
                {
                    body = body.Replace("{FExplanation}", "Finans Açıklaması: " + project.FExplanation);
                }
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendEmailForProjectAcceptedFinance(int number)
        {
            try
            {
                string link = baseUrl + "ProjectsAgreement/ProjectAgreement?number=" + number;
                var project = layer.GetProjectAgreementById(number);
                string email = layer.GetUserById(project.UserId).Email;
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = project.CustomerName + " Müşterisine ait projenin oluşturulması hk.";
                foreach (var item in project.Ntl_ProjectAgreementDocs)
                {
                    string pathDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string path = pathDirectory + "Sözleşmeler\\" + item.DocumentName;
                    if (File.Exists(path))
                    {
                        string extension = Path.GetExtension(path);
                        if (!string.IsNullOrEmpty(item.DocumentName))
                        {
                            byte[] fileBytes = File.ReadAllBytes(path);
                            Attachment att = new Attachment(new MemoryStream(fileBytes), item.DocumentName + extension);
                            Email.Attachments.Add(att);
                        }
                    }
                }
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForAcceptedProjectFinanceBody(project, link), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForAcceptedProjectFinanceBody(Ntl_ProjectsAgreement project, string link)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", project.CustomerName + " müşterisine proje açılması hakkında");
                body = body.Replace("{Person}", "SAYIN " + layer.GetUserById(project.UserId).FullName);
                body = body.Replace("{Content}", project.CustomerName + " müşterisine yeni bir proje oluşturulması 'FİNANS ADI' tarafından onaylanmıştır. Projeyi incelemek için <a href=" + link + ">buraya tıklayınız.</a>");
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{LExplanation}", "");
                if (string.IsNullOrEmpty(project.FExplanation))
                {
                    body = body.Replace("{FExplanation}", "");
                }
                else
                {
                    body = body.Replace("{FExplanation}", "Finans Açıklaması: " + project.FExplanation);
                }
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendConfirmationEmail(string userId, string token)
        {
            var user = layer.GetUserById(userId);
            string email = user.Email;
            try
            {
                string link = baseUrl + "User/ConfirmEmail?userId=" + userId + "&token=" + HttpUtility.UrlEncode(token);
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = "E-Posta onayı hk.";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailForConfirmationBody(link, user), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetEmailForConfirmationBody(string link, AspNetUsers user)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", "Hesabınız için e-posta onayı hakkında");
                body = body.Replace("{Person}", "SAYIN " + user.FullName);
                body = body.Replace("{Content}", "E-Posta adresinizi onaylamak için <a href=" + link + ">buraya tıklayınız.</a>");
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{LExplanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
        public bool SendResetPasswordEmail(string userId, string link)
        {
            var user = layer.GetUserById(userId);
            string email = user.Email;
            try
            {
                MailMessage Email = new MailMessage();
                SmtpClient mailClient = new SmtpClient();
                if (Regex.IsMatch(email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == true)
                {
                    Email.To.Add(email);
                }
                Email.IsBodyHtml = true;
                Email.Subject = "Şifre sıfırlama hk.";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetResetPasswordBody(link, user), null, "text/html");
                Email.AlternateViews.Add(htmlView);
                Email.From = new MailAddress("burak.aktas@netline.net.tr", "Burak Aktaş");
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("servis@netline.net.tr", "%Net11Line%");
                mailClient.Host = "mail.netline.net.tr";
                mailClient.Port = 587;
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = basicAuthenticationInfo;
                mailClient.Send(Email);
            }
            catch (Exception)
            {
            }
            return true;
        }
        public string GetResetPasswordBody(string link, AspNetUsers user)
        {
            string body = string.Empty;
            try
            {
                StreamReader reader;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                reader = new StreamReader(path + "/Templates/MainTemplate.html");
                body = reader.ReadToEnd();
                body = body.Replace("{Explain}", "Hesabınızın şifresini sıfırlama hakkında");
                body = body.Replace("{Person}", "SAYIN " + user.FullName);
                body = body.Replace("{Content}", "Hesabınızın şifresini sıfırlamak için <a href=" + link + ">buraya tıklayınız.</a>");
                body = body.Replace("{Explanation}", "");
                body = body.Replace("{LExplanation}", "");
                body = body.Replace("{FExplanation}", "");
                return body;
            }
            catch (Exception)
            {
                return body;
            }
        }
    }
}