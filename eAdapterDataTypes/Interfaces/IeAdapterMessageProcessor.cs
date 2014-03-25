namespace OTE.eAdapter.DataTypes
{
    using System.Collections.Generic;
    using OTE.eAdapter.DataTypes.Interchange;
    using OTE.eAdapter.DataTypes.Universal;

    /// <summary>
    /// eAdapter Message Processor Interface
    /// </summary>
    public interface IeAdapterMessageProcessor
    {
        /// <summary>
        /// Process Universal Shipment
        /// </summary>
        /// <param name="pShipments">List of Universal Shipments</param>
        void ProcessMessages(List<UniversalShipmentData> pShipments);

        /// <summary>
        /// Process Consolidations
        /// </summary>
        /// <param name="pTransactions">List of Universal Shipments (Consolidations)</param>
        void ProcessMessagesConsolidations(List<UniversalShipmentData> pConsolidations);

        /// <summary>
        /// Process Universal Events
        /// </summary>
        /// <param name="pEvents">List of Universal Events</param>
        void ProcessMessages(List<UniversalEventData> pEvents);

        /// <summary>
        /// Process Universal Transactions
        /// </summary>
        /// <param name="pTransactions">List of Universal Transactions</param>
        void ProcessMessages(List<UniversalTransactionData> pTransactions);

        /// <summary>
        /// Process Interchange Shipments
        /// </summary>
        /// <param name="pShipments">List of Interchange Shipments</param>
        void ProcessMessages(List<Shipments> pShipments);
    }
}
