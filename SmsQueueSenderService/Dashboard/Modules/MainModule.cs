using Activetrail.SmsQueueSenderService.Dashboard.Models;
using Activetrail.SmsQueueSenderService.Dashboard.Repositories;
using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using ActiveTrail.Framework;
using ActiveTrail.Framework.Core;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;
using System;

namespace Activetrail.SmsQueueSenderService.Modules
{
    public class MainModule : NancyModule
    {
        ISmsCounterRepository _repo;
        ISmsServiceConfiguration _configuration;
        ISmsMessageSender _smsMessageSender;

        public MainModule()
        {

        }
        public MainModule(ISmsCounterRepository repo, ISmsServiceConfiguration configuration, ISmsMessageSender smsMessageSender) : base()
        {
            _repo = repo;
            _configuration = configuration;
            _smsMessageSender = smsMessageSender;

            Get["/"] = _ =>
            {
                var model = new SmsCounters
                {
                    Proxies = repo.List()
                };
                return View["Home/Index", model];
            };

            Get["/Send", runAsync: true] = async (_, token) =>
           {
               try
               {
                   var message = this.Bind<SMSMessageQueueInfo>();
                   var validationResult = this.Validate(message);

                   if (!validationResult.IsValid)
                   {
                       return Negotiate.WithModel(validationResult).WithStatusCode(HttpStatusCode.BadRequest);
                   }

                   await smsMessageSender.ProcessAsync(message);
               }
               catch (Exception ex)
               {
                   LogWriter.LogException(ex, "ATSS Failed to connect to sms sending service");
               }

               return new
               {
                   Result = "Message sent"
               };
           };

            Get["/Messages/{proxy}/{status?All}/{page?0}"] = _ =>
            {
                var proxy = Enum.Parse(typeof(TypesSmsProxyTypes), _.proxy);
                var list = repo.GetBy(proxy, _.status, _.page);

                //var model = new MessagesDto
                //{
                //    Messages = list
                //};

                return View["Home/Messages", list];
            };
        }
    }
}
