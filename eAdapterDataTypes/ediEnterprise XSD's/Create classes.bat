xsd /p:xsd_config.xml
ren Native_NativeAirline_NativeCommodityCode_NativeCompany_NativeContainer_NativeCountry_NativeCurrencyExchangeRate_NativeDangerousGood_NativeOrganization_NativeServiceLevel_NativeShipment_NativeStaff_NativeUNLOCO_NativeVessel_UniversalCommon.cs eAdapterDataTypesNative.cs
xsd /p:xsd_config-Universal.xml
ren UniversalCommon_UniversalEvent_UniversalInterchange_UniversalSchedule_UniversalShipment_UniversalTransaction_UniversalTransactionBatch.cs eAdapterDataTypesUniversal.cs
xsd /p:xsd_config-XMLInterchange.xml
ren Elements_XMLInterchange_Shipment_Invoice_Order_Event_FinancialInvoice_FinancialInvoiceLine_Billing_Declaration_ExportAWBHeader.cs eAdapterDataTypesInterchange.cs