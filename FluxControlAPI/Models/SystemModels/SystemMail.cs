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

namespace FluxControlAPI.Models.SystemModels
{
    public class SystemMail
    {
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

                    client.Connect("smtp-relay.sendinblue.com", 587, false);

                    client.Authenticate("emurb.sistema@gmail.com", "R8kyPf6D4QWMApKT");

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
