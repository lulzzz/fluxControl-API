using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.Datas;
using FluxControlAPI.Models.Datas.BusinessRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluxControlAPI.Controllers
{
    [ApiController]
    [Authorize("Bearer", Roles = "Administrator")]
    [Route("API/[controller]")]
    public class InvoiceController : ControllerBase
    {
        [HttpPost]
        [Route("GenerateInvoice/{companyId}")]
        public ActionResult GenerateInvoice(int companyId, [FromBody] DateTime begin)
        {
            try
            {
                Company company;
                Rules rules;

                using (var companyDAO = new CompanyDAO())
                    company = companyDAO.Get(companyId);

                using (var providerDAO = new RulesDAO())
                    rules = providerDAO.Get();

                if (rules == null)
                    return StatusCode(404, new { Message = "Não há regras base para o cálculo da fatura" });

                List<FlowRecord> records;

                using (var flowRecordDAO = new FlowRecordDAO())
                {
                    decimal total = 0;
                    records = flowRecordDAO.ListCompanyRecords(company.Id, begin, begin.AddDays(company.InvoiceInterval));

                    Parallel.ForEach(records, (record) =>
                    {
                        if (flowRecordDAO.MarkCharge(record.Id, company.Id))
                        {
                            // Calcula o valor para o período de permanência desse Ônibus com base no intervalo de cobrança e o valor da tarifa
                            var permanence = record.Arrival - record.Departure;
                            total += rules.Tax * (Math.Ceiling(Convert.ToDecimal(permanence.Value.TotalMinutes) / rules.IntervalMinutes));

                            // Caso não tenha permanecido menos que o intervalo de cobrança, é cobrado a taxa mínima
                            if (total == 0)
                                total += rules.Tax;
                        }
                    });

                    using (var invoiceDAO = new InvoiceDAO())
                    {
                        var invoice = new Invoice()
                        {
                            GenerationDate = DateTime.Now,
                            CompanyDebtor = company.Id,
                            TaxConsidered = rules.Tax,
                            IntervalMinutesConsidered = rules.IntervalMinutes,
                            TotalCost = total
                        };

                        var invoiceId = invoiceDAO.Add(invoice);

                        if (invoiceId != 0)
                        {
                            invoice.Id = invoiceId;
                            return StatusCode(200, invoice);
                        }

                    }

                }

                return StatusCode(304);
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno ao gerar fatura" });
            }
        }
    }
}