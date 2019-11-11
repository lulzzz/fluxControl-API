using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataAccessObjects;
using FluxControlAPI.Models.DataAccessObjects.BusinessRule;
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
        [Route("GenerateInvoice/{company}")]
        public ActionResult GenerateInvoice(Company company, [FromBody] DateTime begin)
        {
            Rules rules;

            using (var providerDAO = new ProviderDAO())
                rules = providerDAO.GetRules();

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
                        total += rules.Tax * (Math.Ceiling(Convert.ToDecimal(permanence.Value.TotalMinutes) / rules.Interval.Minutes));

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
                        IntervalConsidered = rules.Interval,
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
    }
}