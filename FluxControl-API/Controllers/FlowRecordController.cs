using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models;
using FluxControlAPI.Models.APIs.OpenALPR;
using FluxControlAPI.Models.APIs.OpenALPR.Models;
using FluxControlAPI.Models.DataAccessObjects;
using FluxControlAPI.Models.DataAccessObjects.BusinessRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FluxControlAPI.Controllers
{
    
    [ApiController]
    // [Authorize("Bearer", Roles = "System, Operator")]
    [Route("API/[controller]")]
    public class FlowRecordController : ControllerBase
    {
        private string _secretKey { get; set; }

        public FlowRecordController(IConfiguration configuration)
        {
            this._secretKey = configuration.GetSection("Secrets:OpenALPR:secretKey").Get<string>();
        }

        [HttpPost]
        [Route("ProcessImageBytes")]
        public ActionResult ProcessImageBytes()
        {
            try
            {
                var picture = Request.Body;
                var pictureSize = (int) Request.ContentLength;
                
                if (picture != null || picture.Length > 0)
                {
                    using (MemoryStream targetStream = new MemoryStream())
                    {
                        Stream sourceStream = picture;
                        
                        byte[] buffer = new byte[pictureSize + 1];
                        int read = 0;

                        while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            targetStream.Write(buffer, 0, read);

                        Task<string> recognizeTask = Task.Run(() => new OpenALPR(_secretKey).ProcessImage(buffer));
                        recognizeTask.Wait();

                        var response = JsonConvert.DeserializeObject<OpenALPRResponse>(recognizeTask.Result);

                        if (!response.error_code.Equals("400"))
                            response.error = "kk";

                        using (var recordFlowDAO = new FlowRecordDAO())
                            recordFlowDAO.Register("XUMBADO ATENÇÃO", null);

                        // SystemNotifier.SendNotificationAsync(response);
                            
                    }

                    return StatusCode(200);
                }

                return StatusCode(406);
            }

            catch (Exception ex)
            {
                return StatusCode(500);
            }
            
        }

        [HttpPost]
        [Route("Record")]
        public ActionResult Record([FromBody] int busNumber)
        {
            try
            {
                User user = null;

                using (var userDAO = new UserDAO())
                    user = userDAO.Get(Convert.ToInt32(User.FindFirst("id").Value));

                if (user != null)
                {
                    int recordId = 0;

                    using (var FlowRecordDAO = new FlowRecordDAO())
                        recordId = FlowRecordDAO.Register(busNumber.ToString(), user);

                    if (recordId != 0)
                        return StatusCode(202, new { Message = "Registrado com sucesso" });

                    else
                        return StatusCode(304, new { Message = "Não foi possível efetuar o registro" });
                }

                return StatusCode(404, new { Message = "Usuário não encontrado" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao gravar registro no servidor" });
            }
            
        }
    }
}