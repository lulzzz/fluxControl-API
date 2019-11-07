using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models;
using FluxControlAPI.Models.APIs.OpenALPR;
using FluxControlAPI.Models.APIs.OpenALPR.Models;
using FluxControlAPI.Models.DataModels;
using FluxControlAPI.Models.DataModels.BusinessRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FluxControlAPI.Controllers
{
    
    [ApiController]
    // [Authorize("Bearer", Roles = "Operator")]
    [Route("API/[controller]")]
    public class FlowRecordController : ControllerBase
    {
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

                        Task<string> recognizeTask = Task.Run(() => OpenALPR.ProcessImage(buffer));
                        recognizeTask.Wait();

                        var response = JsonConvert.DeserializeObject<OpenALPRResponse>(recognizeTask.Result);

                        if (!response.error_code.Equals("400"))
                            response.error = "kk";

                        using (var recordFlowDAO = new FlowRecordDAO())
                            recordFlowDAO.Register("XUMBADO ATENÇÃO", null);

                            // Printa resultado
                            //SystemNotifier.SendNotificationAsync(response);
                            
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
        public ActionResult Record(int busNumber)
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
                        FlowRecordDAO.Register(busNumber.ToString(), user);

                    if (recordId != 0)
                        return StatusCode(202, new { Message = "Registrado com sucesso" });
                }

                return StatusCode(404, new { Message = "Usuário não encontrado" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Exception = ex, Message = "Erro ao gravar registro no servidor" });
            }
            
        }
    }
}