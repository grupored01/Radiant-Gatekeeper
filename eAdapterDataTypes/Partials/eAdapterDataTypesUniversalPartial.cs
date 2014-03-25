namespace OTE.eAdapter.DataTypes.Universal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    
using OTE.eAdapter.DataTypes.Native;

    /// <summary>
    /// Data Context
    /// </summary>
    public partial class DataContext
    {
        /// <summary>
        /// Retrieves a DataSource value
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="pDataSourceType">DataSource Type</param>
        /// <returns>DataSource value</returns>
        public T RetrieveDataSourceValue<T>(DataSourceType pDataSourceType)
        {
            if (this.DataSourceCollection == null || this.DataSourceCollection.Length == 0)
            {
                return default(T);
            }

            var dataSources = this.DataSourceCollection.Where(ds => ds.Type.Equals(pDataSourceType.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (dataSources.Count() > 0)
            {
                return dataSources.Take(1).Select(ds => (T)Convert.ChangeType(ds.Key, typeof(T))).First();
            }
            else
            {
                return default(T);
            }
        }
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public partial class Shipment
    {
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public NativeShipment NativeJobShipment = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private IEnumerable<Shipment> matchingSubShipments = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        /// <summary>
        /// Matching SubShipments
        /// </summary>
        private IEnumerable<Shipment> MatchingSubShipments
        {
            get
            {
                if (this.matchingSubShipments == null)
                {
                    // Get the shipments from the sub shipments
                    this.matchingSubShipments = this.RetrieveSubShipments(this.RetrieveDataSourceValue<string>(DataSourceType.ForwardingShipment));
                }

                return this.matchingSubShipments;
            }
        }

        /// <summary>
        /// Retrieves a DataSource value
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="pDataSourceType">DataSource Type</param>
        /// <returns>DataSource value</returns>
        public T RetrieveDataSourceValue<T>(DataSourceType pType)
        {
            if (this.DataContext == null)
            {
                return default(T);
            }

            return this.DataContext.RetrieveDataSourceValue<T>(pType);
        }

        /// <summary>
        /// Retrieves the WayBillNumber
        /// </summary>
        /// <returns>WayBillNumber</returns>
        public string RetrieveWayBillNumber()
        {
            if (this.MatchingSubShipments == null)
            {
                return this.WayBillNumber;
            }

            string wayBillNumber = string.Empty;

            // Look for the WayBillNumber in the sub shipments
            foreach (Shipment shipment in this.MatchingSubShipments)
            {
                if (!string.IsNullOrWhiteSpace(shipment.WayBillNumber))
                {
                    wayBillNumber = shipment.WayBillNumber;
                }
            }

            return wayBillNumber;
        }

        /// <summary>
        /// Retrieves the Booking Confirmation Reference
        /// </summary>
        /// <returns>Booking Confirmation Reference</returns>
        public string RetrieveBookingConfirmationReference()
        {
            if (this.MatchingSubShipments == null)
            {
                return this.BookingConfirmationReference;
            }

            string bookingConfirmationReference = string.Empty;

            // Look for the BookingConfirmationReference in the sub shipments
            foreach (Shipment shipment in this.MatchingSubShipments)
            {
                if (!string.IsNullOrWhiteSpace(shipment.BookingConfirmationReference))
                {
                    return shipment.BookingConfirmationReference;
                }
            }

            return bookingConfirmationReference;
        }

        /// <summary>
        /// Retrieves the organization from the shipment
        /// </summary>
        /// <param name="pAddressType">Address type used in the shipment for the organization</param>
        /// <returns>Organization object</returns>
        public OrganizationAddress RetrieveOrganization(params OrganizationAddressType[] pAddressType)
        {
            OrganizationAddress organization = default(OrganizationAddress);

            if (this.OrganizationAddressCollection != null && this.OrganizationAddressCollection.Length > 0)
            {
                string[] addressType = pAddressType.Select(at => at.ToString()).ToArray();

                // Look for the organization in the main shipment
                //Descomentar esto   
                //organization = this.OrganizationAddressCollection.Where(oa => oa.AddressType.In(addressType)).FirstOrDefault();

                if (organization != default(OrganizationAddress))
                {
                    return organization;
                }
            }

            // Look for the organization in the sub shipments
            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    organization = shipment.RetrieveOrganization(pAddressType);

                    if (organization != default(OrganizationAddress))
                    {
                        return organization;
                    }
                }
            } 

            return organization;
        }

        /// <summary>
        /// Retrieves the Service Level from the shipment
        /// </summary>
        /// <returns>Service Level</returns>
        public ServiceLevel RetrieveServiceLevel()
        {
            if (this.ServiceLevel != null)
            {
                return this.ServiceLevel;
            }

            ServiceLevel serviceLevel = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    serviceLevel = shipment.RetrieveServiceLevel();

                    if (serviceLevel != null)
                    {
                        return serviceLevel;
                    }
                } 
            }

            return serviceLevel;
        }

        /// <summary>
        /// Retrieves the Inco Term from the shipment
        /// </summary>
        /// <returns>Inco Term</returns>
        public IncoTerm RetrieveIncoTerm()
        {
            if (this.ShipmentIncoTerm != null)
            {
                return this.ShipmentIncoTerm;
            }

            IncoTerm incoTerm = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    incoTerm = shipment.RetrieveIncoTerm();

                    if (incoTerm != null)
                    {
                        return incoTerm;
                    }
                }
            }

            return incoTerm;
        }

        /// <summary>
        /// Retrieves the Container from the shipment
        /// </summary>
        /// <returns>Container</returns>
        public string RetrieveContainer()
        {
            if (this.ContainerMode != null)
            {
                return this.ContainerMode.Code;
            }

            string container = string.Empty;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    container = shipment.RetrieveContainer();

                    if (!string.IsNullOrWhiteSpace(container))
                    {
                        return container;
                    }
                }
            }

            return container;
        }

        /// <summary>
        /// Retrieves the Goods Description from the shipment
        /// </summary>
        /// <returns>Goods Description</returns>
        public string RetrieveGoodsDescription()
        {
            if (!string.IsNullOrWhiteSpace(this.GoodsDescription))
            {
                return this.GoodsDescription;
            }

            string goodsDescription = string.Empty;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    goodsDescription = shipment.RetrieveGoodsDescription();

                    if (!string.IsNullOrWhiteSpace(goodsDescription))
                    {
                        return goodsDescription;
                    }
                }
            }

            return goodsDescription;
        }

        /// <summary>
        /// Retrieves the Branch from the shipment
        /// </summary>
        /// <returns>Branch</returns>
        public Branch RetrieveBranch()
        {
            if (this.Branch != null)
            {
                return this.Branch;
            }

            Branch branch = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    branch = shipment.RetrieveBranch();

                    if (branch != null)
                    {
                        return branch;
                    }
                }
            }

            return branch;
        }

        /// <summary>
        /// Retrieves the Order Numbers from the shipment
        /// </summary>
        /// <returns>Order Numbers</returns>
        public ShipmentLocalProcessingOrderNumber[] RetrieveOrderNumbers()
        {
            if (this.LocalProcessing != null && this.LocalProcessing.OrderNumberCollection != null)
            {
                return this.LocalProcessing.OrderNumberCollection;
            }

            ShipmentLocalProcessingOrderNumber[] orderNumberCollection = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    orderNumberCollection = shipment.RetrieveOrderNumbers();

                    if (orderNumberCollection != null)
                    {
                        return orderNumberCollection;
                    }
                }
            }

            return orderNumberCollection;
        }

        /// <summary>
        /// Retrieves the Charge Lines from the Shipment
        /// </summary>
        /// <returns>Charge Lines</returns>
        public ShipmentJobCostingChargeLine[] RetrieveChargeLines()
        {
            if (this.JobCosting != null && this.JobCosting.ChargeLineCollection != null)
            {
                return this.JobCosting.ChargeLineCollection;
            }

            ShipmentJobCostingChargeLine[] chargeLineCollection = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    chargeLineCollection = shipment.RetrieveChargeLines();

                    if (chargeLineCollection != null)
                    {
                        return chargeLineCollection;
                    }
                }
            }

            return chargeLineCollection;
        }

        /// <summary>
        /// Retrieves the Packing Lines from the Shipment
        /// </summary>
        /// <returns>Packing Lines</returns>
        public PackingLine[] RetrievePackingLines()
        {
            if (this.PackingLineCollection != null)
            {
                return this.PackingLineCollection;
            }

            PackingLine[] packingLineCollection = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    packingLineCollection = shipment.RetrievePackingLines();

                    if (packingLineCollection != null)
                    {
                        return packingLineCollection;
                    }
                }
            }

            return packingLineCollection;
        }

        /// <summary>
        /// Retrieves the Outer Packs from the shipment
        /// </summary>
        /// <returns>Outer Packs</returns>
        public int RetrieveOuterPacks()
        {
            if (this.OuterPacks != 0)
            {
                return this.OuterPacks;
            }

            int outerPacks = 0;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    outerPacks = shipment.RetrieveOuterPacks();

                    if (outerPacks != 0)
                    {
                        return outerPacks;
                    }
                }
            }

            return outerPacks;
        }

        /// <summary>
        /// Retrieves the Outer Packs Package Type from the shipment
        /// </summary>
        /// <returns>Outer Packs Package Type</returns>
        public PackageType RetrieveOuterPacksPackageType()
        {
            if (this.OuterPacksPackageType != null)
            {
                return this.OuterPacksPackageType;
            }

            PackageType outerPacksPackageType = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    outerPacksPackageType = shipment.RetrieveOuterPacksPackageType();

                    if (outerPacksPackageType != null)
                    {
                        return outerPacksPackageType;
                    }
                }
            }

            return outerPacksPackageType;
        }

        /// <summary>
        /// Retrieves the Container from the shipment
        /// </summary>
        /// <returns>Container</returns>
        public IEnumerable<Note> RetrieveNotes(string pNoteDescription)
        {
            if (this.NoteCollection != null)
            {
                return this.NoteCollection.Where(nt => nt.Description.Equals(pNoteDescription, StringComparison.InvariantCultureIgnoreCase));
            }

            IEnumerable<Note> notes = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    notes = shipment.RetrieveNotes(pNoteDescription);

                    if (notes != null)
                    {
                        return notes;
                    }
                }
            }

            return notes;
        }

        /// <summary>
        /// Retrieves the Total Charge from the shipment
        /// </summary>
        /// <param name="pChargeField">Field containing the charge</param>
        /// <returns>Total Charge</returns>
        public decimal? RetrieveTotalCharge(string pChargeField)
        {
            if (this.JobCosting != null && this.JobCosting.ChargeLineCollection != null)
            {
                //Descomentar esto   
                //return this.JobCosting.ChargeLineCollection.Sum(cl => cl.RetrievePropertyValue<decimal>(pChargeField));
            }

            decimal? totalCharge = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    totalCharge = shipment.RetrieveTotalCharge(pChargeField);

                    if (totalCharge != default(decimal))
                    {
                        return totalCharge;
                    }
                } 
            }

            return null;
        }

        /// <summary>
        /// Retrieves the date from the shipment
        /// </summary>
        /// <param name="pDateType">Used to look for the date on the DateCollection</param>
        /// <param name="pDateProperty">Used to look for the date on the LocalProcessing</param>
        /// <returns>The date for the event</returns>
        public DateTime? RetrieveDate(DateType? pDateType, string pDateProperty)
        {            
            // Look for the date on the DateCollection
            if (this.DateCollection != null && this.DateCollection.Length > 0 && pDateType != null)
            {
                //Descomentar esto   
                //return this.DateCollection.Where(dt => dt.Type == pDateType && !string.IsNullOrWhiteSpace(dt.Value)).Select(dt => DateTime.Parse(dt.Value)).FirstOrNull();
            }

            // Look for the date on the LocalProcessing
            if (this.LocalProcessing != null && !string.IsNullOrWhiteSpace(pDateProperty))
            {
                //Descomentar esto   
                //DateTime date2;
                //if (DateTime.TryParse(this.LocalProcessing.RetrievePropertyValue<string>(pDateProperty), out date2))
                //{
                //    return date2;
                //}
            }

            DateTime? date = null;

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    date = shipment.RetrieveDate(pDateType, pDateProperty);

                    if (date != null)
                    {
                        return date;
                    }
                }
            }

            return date;
        }

        /// <summary>
        /// Retrieves the shipments matching the shipment number from the sub shipments
        /// </summary>
        /// <param name="pShipmentNumber">Shipment number to look for</param>
        /// <returns>Shipment</returns>
        public IEnumerable<Shipment> RetrieveSubShipments(string pShipmentNumber)
        {
            // Look for the organization in the sub shipments
            if (this.SubShipmentCollection != null && this.SubShipmentCollection.Length > 0)
            {
                // Get the sub shipments matching pShipmentNumber 
                return this.SubShipmentCollection.Where(sh => sh.DataContext != null && sh.DataContext.RetrieveDataSourceValue<string>(DataSourceType.ForwardingShipment).Equals(pShipmentNumber, StringComparison.InvariantCultureIgnoreCase));
            }

            return default(IEnumerable<Shipment>);
        }

        /// <summary>
        /// Retrieves the Order Number
        /// </summary>
        /// <param name="pSequence">Sequence</param>
        /// <returns>Order Number</returns>
        public string RetrieveOrderNumber(int pSequence)
        {
            string orderNumber = string.Empty;

            ShipmentLocalProcessingOrderNumber[] orderNumberCollection = this.RetrieveOrderNumbers();

            if (orderNumberCollection != null && orderNumberCollection.Length > 0)
            {
                orderNumber = orderNumberCollection.Where(on => on.Sequence == pSequence).Select(on => on.OrderReference).FirstOrDefault();
            }

            return orderNumber;
        }

        /// <summary>
        /// Retrieves the Actual Chargeable
        /// </summary>
        /// <returns>Actual Chargeable</returns>
        public decimal? RetrieveActualChargeable()
        {
            if (this.ActualChargeableSpecified)
            {
                return this.ActualChargeable;
            }

            if (this.MatchingSubShipments != null)
            {
                foreach (Shipment shipment in this.MatchingSubShipments)
                {
                    if (shipment.ActualChargeableSpecified)
                    {
                        return shipment.ActualChargeable;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Organization Address
    /// </summary>
    public partial class OrganizationAddress
    {
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public NativeOrganization NativeOrganization = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public NativeUNLOCO NativeUNLOCO = null;

        public override string ToString()
        {
            return string.Format("Name: {0}, Code: {1}, Address Type: {2}", this.CompanyName, this.OrganizationCode, this.AddressType);
        }
    }

    /// <summary>
    /// Event
    /// </summary>
    public partial class Event
    {
        /// <summary>
        /// Retrieves a DataSource value
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="pDataSourceType">DataSource Type</param>
        /// <returns>DataSource value</returns>
        public T RetrieveDataSourceValue<T>(DataSourceType pKey)
        {
            if (this.DataContext == null)
            {
                return default(T);
            }

            return this.DataContext.RetrieveDataSourceValue<T>(pKey);
        }

        /// <summary>
        /// Retrieves the user from the event
        /// </summary>
        /// <returns>User code</returns>
        public string RetrieveUser()
        {
            string user = string.Empty;

            if (this.DataContext != null && this.DataContext.EventUser != null)
            {
                user = this.DataContext.EventUser.Code;
            }

            return user;
        }

        /// <summary>
        /// Retrieves the description from the event
        /// </summary>
        /// <returns>Event description</returns>
        public string RetrieveEventDescription()
        {
            string description = string.Empty;

            if (this.DataContext != null)
            {
                if (this.DataContext.TriggerType == DataContextTriggerType.Milestone)
                {
                    description = this.DataContext.TriggerDescription;
                }
                else if (this.DataContext.EventType != null)
                {
                    description = this.DataContext.EventType.Description;
                }
            }

            return description;
        }

        /// <summary>
        /// Adds an Event Context to the Event
        /// </summary>
        /// <param name="pType">Type</param>
        /// <param name="pValue">Value</param>
        public void AddEventContext(string pType, string pValue)
        {
            this.AddEventContext(new EventContext() { Type = new EventContextType() { Value = pType }, Value = pValue });
        }

        /// <summary>
        /// Adds an Event Context to the Event
        /// </summary>
        /// <param name="pEventContext">Event Context</param>
        public void AddEventContext(EventContext pEventContext)
        {
            if (this.ContextCollection != null)
            {
                Array.Resize(ref this.ContextCollection, this.ContextCollection.Length + 1);
                this.ContextCollection[this.ContextCollection.Length - 1] = pEventContext;
            }
            else
            {
                this.ContextCollection = new EventContext[] { pEventContext };
            }
        }

        public override string ToString()
        {
            return string.Format("Event: {0}, Time: {1}", this.EventType, this.EventTime);
        }
    }

    public partial class ShipmentLocalProcessing
    {
        /// <summary>
        /// Retrieves the Order Number
        /// </summary>
        /// <param name="pSequence">Sequence</param>
        /// <returns>Order Number</returns>
        public string RetrieveOrderNumber(int pSequence)
        {
            string orderNumber = string.Empty;

            if (this.OrderNumberCollection != null && this.OrderNumberCollection.Length > 0)
            {
                orderNumber = this.OrderNumberCollection.Where(on => on.Sequence == pSequence).Select(on => on.OrderReference).FirstOrDefault();
            }

            return orderNumber;
        }
    }

    /// <summary>
    /// Staff
    /// </summary>
    public partial class Staff
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Company
    /// </summary>
    public partial class Company
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Event ContextType
    /// </summary>
    public partial class EventContextType
    {
        public override string ToString()
        {
            return this.Value;
        }
    }

    /// <summary>
    /// Event Context
    /// </summary>
    public partial class EventContext
    {
        public override string ToString()
        {
            return this.Type != null ? string.Format("Type: {0}, Value: {1}", this.Type, this.Value) : this.Value;
        }
    }

    /// <summary>
    /// DataSource
    /// </summary>
    public partial class DataSource
    {
        public override string ToString()
        {
            return string.Format("Type: {0}, Key: {1}", this.Type, this.Key);
        }
    }

    /// <summary>
    /// Country
    /// </summary>
    public partial class Country
    {
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(this.Name))
            {
                return string.Format("{0} ({1})", this.Name, this.Code);
            }
            if (!string.IsNullOrWhiteSpace(this.Code))
            {
                return this.Code;
            }
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                return this.Name;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// CodeDescription Pair
    /// </summary>
    public partial class CodeDescriptionPair
    {
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Code) && !string.IsNullOrWhiteSpace(this.Description))
            {
                return string.Format("{0} ({1})", this.Description, this.Code);
            }
            if (!string.IsNullOrWhiteSpace(this.Code))
            {
                return this.Code;
            }
            if (!string.IsNullOrWhiteSpace(this.Description))
            {
                return this.Description;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Container Mode
    /// </summary>
    public partial class ContainerMode
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Date
    /// </summary>
    public partial class Date
    {
        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}", this.Type, this.Value);
        }
    }

    /// <summary>
    /// Currency
    /// </summary>
    public partial class Currency
    {
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Code))
            {
                return string.Format("{0} ({1})", this.Description, this.Code);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Branch
    /// </summary>
    public partial class Branch
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Commodity
    /// </summary>
    public partial class Commodity
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Note
    /// </summary>
    public partial class Note
    {
        public override string ToString()
        {
            return string.Format("Type: {0}, NoteText: ({1})", this.Description, this.NoteText);
        }
    }

    /// <summary>
    /// Package Type
    /// </summary>
    public partial class PackageType
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Packing Line
    /// </summary>
    public partial class PackingLine
    {
        public override string ToString()
        {
            return string.Format("Goods Description: {0}, Quantity: ({1})", this.GoodsDescription, this.PackQty);
        }
    }

    /// <summary>
    /// Unit Of Length
    /// </summary>
    public partial class UnitOfLength
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Unit Of Volume
    /// </summary>
    public partial class UnitOfVolume
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Unit Of Weight
    /// </summary>
    public partial class UnitOfWeight
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// UNLOCO
    /// </summary>
    public partial class UNLOCO
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Service Level
    /// </summary>
    public partial class ServiceLevel
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Inco Term
    /// </summary>
    public partial class IncoTerm
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// WayBill Type
    /// </summary>
    public partial class WayBillType
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// Department
    /// </summary>
    public partial class Department
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Code);
        }
    }

    /// <summary>
    /// Entity Reference
    /// </summary>
    public partial class EntityReference
    {
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Type, this.Key);
        }
    }

    /// <summary>
    /// Charge Code
    /// </summary>
    public partial class ChargeCode
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.Code);
        }
    }

    /// <summary>
    /// GL Account
    /// </summary>
    public partial class GLAccount
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Description, this.AccountCode);
        }
    }

    /// <summary>
    /// Transaction
    /// </summary>
    public partial class TransactionInfo
    {
        /// <summary>
        /// Retrieves the Debtor Code
        /// </summary>
        /// <returns>Debtor Code</returns>
        public string RetrieveDebtorCode()
        {
            string debtor = string.Empty;

            if (this.PostingJournalCollection != null && this.PostingJournalCollection.Count() > 0)
            {
                debtor = this.PostingJournalCollection.First().RetrieveDebtorCode();
            }

            return debtor;
        }

        /// <summary>
        /// Retrieves the Debtor
        /// </summary>
        /// <returns>Organization object</returns>
        public OrganizationAddress RetrieveDebtor()
        {
            OrganizationAddress organization = default(OrganizationAddress);

            // Gets the shipment number
            string shipmentNumber = this.JobInvoiceNumber;
            
            // Get the shipment from the sub shipments
            IEnumerable<Shipment> subShipments = this.RetrieveSubShipments(shipmentNumber);

            // Look for the organization in the sub shipments
            if (subShipments != null)
            {
                if (subShipments != null)
                {
                    foreach (Shipment shipment in subShipments)
                    {
                        organization = shipment.RetrieveOrganization(new OrganizationAddressType[] { OrganizationAddressType.LocalClient, OrganizationAddressType.SendersLocalClient });

                        if (organization != default(OrganizationAddress))
                        {
                            return organization;
                        }
                    }
                } 
            }

            return organization;
        }

        /// <summary>
        /// Retrieves the shipments matching the shipment number from the sub shipments
        /// </summary>
        /// <param name="pShipmentNumber">Shipment number to look for</param>
        /// <returns>Shipment</returns>
        public IEnumerable<Shipment> RetrieveSubShipments(string pShipmentNumber)
        {
            // Look for the organization in the sub shipments
            if (this.ShipmentCollection != null && this.ShipmentCollection.Length > 0)
            {
                // Get the sub shipments matching pShipmentNumber 
                return this.ShipmentCollection.Where(sh => sh.DataContext != null && sh.DataContext.RetrieveDataSourceValue<string>(DataSourceType.ForwardingShipment).Equals(pShipmentNumber, StringComparison.InvariantCultureIgnoreCase));
            }

            return default(IEnumerable<Shipment>);
        }
    }

    /// <summary>
    /// Posting Journal
    /// </summary>
    public partial class PostingJournal
    {
        /// <summary>
        /// Retrieves the Debtor Code
        /// </summary>
        /// <returns>Debtor Code</returns>
        public string RetrieveDebtorCode()
        {
            if (this.Organization != null)
            {
                return this.Organization.Key;
            }
            else
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.ChargeCode, this.OSAmount);
        }
    }
}