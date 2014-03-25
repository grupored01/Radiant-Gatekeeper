namespace OTE.eAdapter.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Helper class for native request
    /// </summary>
    public class NativeRequest : IToXml
    {
        private NativeType requestedType;
        private List<NativeCriteriaGroup> conditionGroups = new List<NativeCriteriaGroup>();

        /// <summary>
        /// Native type
        /// </summary>
        public NativeType RequestedType
        {
            get
            {
                return this.requestedType;
            }
            set
            {
                this.requestedType = value;
            }
        }

        /// <summary>
        /// List of Condition Groups for the request
        /// </summary>
        public List<NativeCriteriaGroup> ConditionGroups
        {
            get
            {
                return this.conditionGroups;
            }
        }

        /// <summary>
        /// Creates a new instance of NativeRequest
        /// </summary>
        /// <param name="pRequestedType">Native message type</param>
        public NativeRequest(NativeType pRequestedType)
        {
            this.requestedType = pRequestedType;
        }

        /// <summary>
        /// Creates a new instance of NativeRequest
        /// </summary>
        /// <param name="pRequestedType">Native message type</param>
        /// <param name="pConditionGroups">List of conditions</param>
        public NativeRequest(NativeType pRequestedType, List<NativeCriteriaGroup> pConditionGroups)
        {
            this.requestedType = pRequestedType;

            if (pConditionGroups == null)
            {
                throw new ArgumentNullException("pConditions");
            }

            if (pConditionGroups.Count == 0)
            {
                throw new ArgumentOutOfRangeException("pConditions", "The list of condition groups can not be empty");
            }

            this.conditionGroups = pConditionGroups;
        }

        /// <summary>
        /// Adds a condition to the NativeRequest
        /// </summary>
        /// <param name="pEntity">Entity where the fields belongs to</param>
        /// <param name="pFieldName">Field from the entity</param>
        /// <param name="pValue">Value to look for</param>
        /// <param name="pMatchType">Match Type</param>
        public void AddCondition(string pEntity, string pFieldName, string pValue, CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            this.conditionGroups.Add(new NativeCriteriaGroup(pEntity, pFieldName, pValue, pMatchType));
        }

        /// <summary>
        /// Adds a condition to the NativeRequest
        /// </summary>
        /// <param name="pEntity">Entity where the fields belongs to</param>
        /// <param name="pFieldName">Field from the entity</param>
        /// <param name="pValues">List of values to look for</param>
        /// <param name="pMatchType">Match Type</param>
        public void AddCondition(string pEntity, string pFieldName, List<string> pValues, CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            foreach (string value in pValues)
            {
                NativeCriteriaGroup criteriaGroup = new NativeCriteriaGroup(pEntity, pFieldName, value, pMatchType);

                this.conditionGroups.Add(criteriaGroup);
            }
        }

        /// <summary>
        /// Converts the NativeRequest to its XML representation
        /// </summary>
        /// <returns>Xml representation of the NativeRequest</returns>
        public string ToXml()
        {
            if (this.conditionGroups.Count == 0)
            {
                throw new InvalidOperationException("The list of condition groups can not be empty");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("<").Append(this.requestedType.ToString()).Append(">");

            foreach (NativeCriteriaGroup condition in this.conditionGroups)
            {
                builder.Append(condition.ToXml());
            }

            builder.Append("</").Append(this.requestedType.ToString()).Append(">");

            return eAdapterDataTypesFactory.WrapMessage(builder.ToString(), WrapperType.Native);
        }

        public override string ToString()
        {
            return this.requestedType.ToString();
        }
    }
}
