namespace OTE.eAdapter.DataTypes
{
    /// <summary>
    /// Wrapper Type used for the eAdapter Message
    /// </summary>
    public enum WrapperType
    {
        Native,
        Universal,
        NativeResponse,
        Interchange
    }

    /// <summary>
    /// Native message type
    /// </summary>
    public enum NativeType
    {
        Airline,
        CommodityCode,
        Company,
        Container,
        Country,
        CurrencyExchangeRate,
        DangerousGood,
        Organization,
        ServiceLevel,
        Shipment,
        Staff,
        UNLOCO,
        Vessel
    }

    /// <summary>
    /// Message type
    /// </summary>
    public enum MessageType
    {
        UniversalEvent,
        UniversalShipment,
        UniversalConsolidation,
        UniversalAccountingBatch,
        UniversalTransactionInfo,
        InterchangeShipment
    }

    /// <summary>
    /// Message type code
    /// </summary>
    public enum MessageTypeCode
    {
        XUE,
        XUS,
        XUC,
        UAB,
        XUT,
        XML
    }

    /// <summary>
    /// CriteriaGroup Match Type
    /// </summary>
    public enum CriteriaGroupMatchType
    {
        Partial,
        Key
    }

    /// <summary>
    /// Organization Address Type used on the shipment
    /// </summary>
    public enum OrganizationAddressType
    {
        Creditor,
        ShippingLineAddress,
        ConsigneeDocumentaryAddress,
        ConsigneePickupDeliveryAddress,
        ConsignorDocumentaryAddress,
        ConsignorPickupDeliveryAddress,
        NotifyParty,
        LocalClient,
        SendersLocalClient,
        PickupLocalCartage,
        DeliveryLocalCartage,
        DeliveryAgent,
        SendingForwarderAddress,
        ReceivingForwarderAddress
    }

    /// <summary>
    /// DataSource Type
    /// </summary>
    public enum DataSourceType
    {
        ForwardingShipment,
        ForwardingConsol
    }
}
