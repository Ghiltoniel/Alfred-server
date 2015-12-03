using Microsoft.Kinect.Toolkit.Interaction;

namespace AlfredInterface
{
    class InteractionClient : IInteractionClient
    {
        public InteractionClient() { }

        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var ii = new InteractionInfo();
            return ii;
        }
    }
}
