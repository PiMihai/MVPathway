using MVPathway.Messages.Abstractions;
using Xamarin.Forms;

namespace MVPathway.Integration.Messages
{
    public class LogMessage : IMessage
    {
        public Span Span { get; set; }
    }
}
