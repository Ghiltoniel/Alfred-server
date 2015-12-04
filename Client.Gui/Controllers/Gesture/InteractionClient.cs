using Microsoft.Kinect.Toolkit.Interaction;

namespace Alfred.Client.Gui
{
    class InteractionClient : IInteractionClient
    {
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var ii = new InteractionInfo();
            return ii;
        }
    }
}
