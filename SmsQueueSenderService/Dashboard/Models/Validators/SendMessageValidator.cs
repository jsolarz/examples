using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using FluentValidation;

namespace Activetrail.SmsQueueSenderService.Dashboard.Models.Validators
{
    public class SendMessageValidator : AbstractValidator<SMSMessageQueueInfo>
    {
        public SendMessageValidator()
        {
            RuleFor(request => request.Token).NotEmpty().WithMessage("Atss the token is needed to send a message");
            RuleFor(request => request.RecipientNumber).NotEmpty().WithMessage("Atss the mobile number is needed to send a message");
            RuleFor(request => request.Content).NotEmpty().WithMessage("Atss the content is needed to send a message");
            RuleFor(request => request.ProxyType).NotEmpty().WithMessage("Atss the proxy type is needed to send a message");
        }
    }
}
