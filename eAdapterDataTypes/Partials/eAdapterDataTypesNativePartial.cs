namespace OTE.eAdapter.DataTypes.Native
{
    using System.Linq;

    /// <summary>
    /// Native Shipment
    /// </summary>
    public partial class NativeShipment
    {
        /// <summary>
        /// Retrieves the event from a Native Shipment
        /// </summary>
        /// <param name="pEvent">Event to retrieve</param>
        /// <returns>Event object</returns>
        public NativeShipmentProcessTasks RetrieveEvent(string pEvent)
        {
            return this.ProcessTasksCollection != null ? this.ProcessTasksCollection.Where(pt => pt.MilestoneEvent != null && pt.MilestoneEvent.Code == pEvent).FirstOrDefault() : null;
        }
    }
}
