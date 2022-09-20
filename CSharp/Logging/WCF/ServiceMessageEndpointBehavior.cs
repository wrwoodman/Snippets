using Common.Contracts.Logging;
using Common.Inspectors;
using PostSharp.Patterns.Diagnostics; // <-- Requires license
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Common.Behavior
{
    [Log(AttributeExclude = true)] // <-- This is from PostSharp
    public class ServiceMessageEndpointBehavior : IEndpointBehavior
    {
        private readonly IUniqueIdExtractor _uniqueIdExtractor;
        private readonly string _messageFormat;

        public ServiceMessageEndpointBehavior(IUniqueIdExtractor uniqueIdExtractor, string messageFormat)
        {
            _uniqueIdExtractor = uniqueIdExtractor;
            _messageFormat = messageFormat;
        }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new ServiceMessageInspector(_uniqueIdExtractor, _messageFormat));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
