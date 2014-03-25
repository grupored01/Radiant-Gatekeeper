namespace OTE.eAdapter.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Helper class for native criteria
    /// </summary>
    public class NativeCriteriaGroup : IToXml
    {
        #region Variables
        private CriteriaGroupMatchType matchType;
        private List<NativeCriteria> conditions = new List<NativeCriteria>();
        #endregion

        #region Properties
        /// <summary>
        /// Match type for the criteria
        /// </summary>
        public CriteriaGroupMatchType MatchType
        {
            get
            {
                return this.matchType;
            }
            set
            {
                this.matchType = value;
            }
        }

        /// <summary>
        /// List of conditions used on the group
        /// </summary>
        public List<NativeCriteria> Conditions
        {
            get
            {
                return this.conditions;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of NativeCriteriaGroup
        /// </summary>
        /// <param name="pMatchType">Match Type</param>
        public NativeCriteriaGroup(CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            this.matchType = pMatchType;
        }

        /// <summary>
        /// Creates a new instance of NativeCriteriaGroup
        /// </summary>
        /// <param name="pConditions">List of conditions</param>
        /// <param name="pMatchType">Match Type</param>
        public NativeCriteriaGroup(List<NativeCriteria> pConditions, CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            this.matchType = pMatchType;

            if (pConditions == null)
            {
                throw new ArgumentNullException("pConditions");
            }

            if (pConditions.Count == 0)
            {
                throw new ArgumentOutOfRangeException("pConditions", "The list of conditions can not be empty");
            }

            this.conditions = pConditions;
        }

        /// <summary>
        /// Creates a new instance of NativeCriteriaGroup
        /// </summary>
        /// <param name="pEntity">Entity where the fields belongs to</param>
        /// <param name="pFieldName">Field from the entity</param>
        /// <param name="pValues">List of values to look for</param>
        /// <param name="pMatchType">Match Type</param>
        public NativeCriteriaGroup(string pEntity, string pFieldName, List<string> pValues, CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            this.matchType = pMatchType;

            foreach (string value in pValues)
            {
                NativeCriteria criteria = new NativeCriteria(pEntity, pFieldName, value);

                this.conditions.Add(criteria);
            }
        }

        /// <summary>
        /// Creates a new instance of NativeCriteriaGroup
        /// </summary>
        /// <param name="pEntity">Entity where the fields belongs to</param>
        /// <param name="pFieldName">Field from the entity</param>
        /// <param name="pValue">Value to look for</param>
        /// <param name="pMatchType">Match Type</param>
        public NativeCriteriaGroup(string pEntity, string pFieldName, string pValue, CriteriaGroupMatchType pMatchType = CriteriaGroupMatchType.Key)
        {
            this.matchType = pMatchType;

            NativeCriteria criteria = new NativeCriteria(pEntity, pFieldName, pValue);

            this.conditions.Add(criteria);
        }

        /// <summary>
        /// Converts the NativeCriteriaGroup to its XML representation
        /// </summary>
        /// <returns>Xml representation of the NativeCriteriaGroup</returns>
        public string ToXml()
        {
            if (this.conditions.Count == 0)
            {
                throw new InvalidOperationException("The list of conditions can not be empty");
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("<CriteriaGroup Type='").Append(this.matchType.ToString()).Append("'>");

            foreach (NativeCriteria condition in this.conditions)
            {
                builder.Append(condition.ToXml());
            }

            builder.Append("</CriteriaGroup>");

            return builder.ToString();
        }

        public override string ToString()
        {
            return this.matchType.ToString();
        }
    }
}
