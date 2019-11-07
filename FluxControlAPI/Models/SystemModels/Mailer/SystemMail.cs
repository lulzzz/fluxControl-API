using FluxControlAPI.Models.DataModels;
using FluxControlAPI.Models.DataModels.BusinessRule;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using FluxControlAPI.Models.SystemModels.UserToken;

namespace FluxControlAPI.Models.SystemModels.Mailer
{
    class SystemMail
    {
        private string Smtp { get; set; }
        private int Port { get; set; }
        private string Login { get; set; }
        private string Password { get; set; }

        public SystemMail(string smtp, int port, string login, string password)
        {
            this.Smtp = smtp;
            this.Port = port;
            this.Login = login;
            this.Password = password;
        }

        public bool SendNewPasswordMail(HttpRequest request, Token token)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Sistema Emurb", "sistema.emurb@gmail.com"));
                message.To.Add(new MailboxAddress(token.User.Email));

                message.Subject = "Nova Senha - " + token.User.Name;

                message.Body = new TextPart("plain")
                {
                    Text = string.Format(
                        @"Olá {0}, para fazer o login no sistema insira este email e a senha que você definir em {1}{2}/DefinirSenha/{3}.",
                        token.User.Name, (request.IsHttps ? "https://" : "http://"), request.Host, token.Hash)
                };

                using (var client = new SmtpClient())
                {
                    #if DEBUG
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    #endif

                    client.Connect(Smtp, Port, false);

                    client.Authenticate(Login, Password);

                    client.Send(message);
                    client.Disconnect(true);

                    return true;
                }

            }
            catch(Exception ex)
            {
                return false;
            }
                
        }
    }
}
