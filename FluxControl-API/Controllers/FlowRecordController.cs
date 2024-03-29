﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Hubs;
using FluxControlAPI.Models;
using FluxControlAPI.Models.APIs.OpenALPR;
using FluxControlAPI.Models.APIs.OpenALPR.Models;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.Datas;
using FluxControlAPI.Models.Datas.BusinessRule;
using FluxControlAPI.Models.SystemModels.Broadcast;
using FluxControlAPI.Models.SystemModels.Broadcast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FluxControlAPI.Controllers
{
    
    [ApiController]
    [Authorize("Bearer", Roles = "System, Operator")]
    [Route("API/[controller]")]
    public class FlowRecordController : ControllerBase
    {
        private SystemNotifier SystemNotifier { get; set; }
        private string _secretKey { get; set; }

        public FlowRecordController(IHubContext<HistoricHub> hub, IConfiguration configuration)
        {
            this.SystemNotifier = new SystemNotifier(hub);
            this._secretKey = configuration.GetSection("Secrets:OpenALPR:secretKey").Get<string>();
        }

        [HttpPost]
        [Route("ProcessImageBytes")]
        public ActionResult ProcessImageBytes()
        {
            try
            {                
                if (Response.Body != null || Response.ContentLength > 0)
                {
                    using (StreamReader targetStream = new StreamReader(Request.Body))
                    {

                        byte[] bytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            Request.Body.CopyTo(memoryStream);
                            bytes = memoryStream.ToArray();
                        }

                        Task<string> recognizeTask = Task.Run(() => new OpenALPR(_secretKey).ProcessImage(bytes));
                        recognizeTask.Wait();

                        var response = JsonConvert.DeserializeObject<OpenALPRResponse>(recognizeTask.Result);

                        if (!response.Error && response.Results.Length > 0)
                        {
                            FlowRecord record = null;

                            using (var recordFlowDAO = new FlowRecordDAO())
                                record = recordFlowDAO.Register(response.Results[0].Plate, null);

                            if (record != null)
                            {
                                SystemNotifier.VehicleActionAsync(record);
                                return StatusCode(200);
                            }

                            else
                            {
                                SystemNotifier.WarningAsync(new SystemWarning()
                                {
                                    Moment = DateTime.Now,
                                    Type = SystemWarning.WarningType.NotRegistered,
                                    LicensePlate = response.Results[0].Plate,
                                    Message = "A placa não foi reconhecida para nenhuma empresa no banco de dados"
                                });

                                return StatusCode(404);
                            }

                        }
                        
                    }

                }

                SystemNotifier.WarningAsync(new SystemWarning()
                {
                    Moment = DateTime.Now,
                    Type = SystemWarning.WarningType.NotRecognized,
                    Message = "Falha ao identificar a placa"
                });

                return StatusCode(406);
            }

            catch (Exception ex)
            {
                return StatusCode(500);
            }
            
        }

        [HttpPost]
        [Route("Record")]
        public ActionResult Record([FromBody] string busNumber)
        {
            try
            {
                User user = null;

                using (var userDAO = new UserDAO())
                    user = userDAO.Get(Convert.ToInt32(User.FindFirst("id").Value));

                if (user != null)
                {
                    FlowRecord record;

                    using (var FlowRecordDAO = new FlowRecordDAO())
                        record = FlowRecordDAO.Register(busNumber, user);

                    if (record != null)
                    {
                        SystemNotifier.VehicleActionAsync(record);
                        return StatusCode(202, new { Message = "Registrado com sucesso" });
                    }
                    else
                    {
                        SystemNotifier.WarningAsync(new SystemWarning()
                        {
                            Moment = DateTime.Now,
                            Type = SystemWarning.WarningType.NotRegistered,
                            LicensePlate = busNumber,
                            Message = "A placa não foi reconhecida para nenhuma empresa no banco de dados"
                        });

                        return StatusCode(304, new { Message = "Não foi possível efetuar o registro" });
                    }
                        
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