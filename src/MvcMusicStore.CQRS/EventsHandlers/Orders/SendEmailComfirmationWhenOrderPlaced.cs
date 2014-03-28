using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Events;
using MvcMusicStore.CQRS.Hubs;
using MvcMusicStore.Model;
using SendGrid;
using SendGrid.Transport;

namespace MvcMusicStore.CQRS.EventsHandlers.Orders
{
    public class SendEmailComfirmationWhenOrderPlaced : IEventHandler<EntityCreatedEvent<Order>>
    {
        public Task HandleAsync(EntityCreatedEvent<Order> domainEvent)
        {
            var transport =
                SMTP.GetInstance(
                    new NetworkCredential("azure_1e11597f868c5011c2718c5cd0be4f40@azure.com", "sq2zl2op"));

            var mailMessage = Mail.GetInstance();
            mailMessage.From = new MailAddress("admin@mamazon.com");
            mailMessage.AddTo(domainEvent.CreatedByUser);
            mailMessage.Subject = "Your mamazon order";
            mailMessage.Text = "Ola k ase";

            transport.Deliver(mailMessage);

            return Task.FromResult(0);
        }
    }
}