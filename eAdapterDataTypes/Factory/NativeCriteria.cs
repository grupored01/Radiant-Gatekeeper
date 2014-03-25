namespace OTE.eAdapter.DataTypes
{
    using System;

    /// <summary>
    /// Helper class for native criteria
    /// </summary>
    public class NativeCriteria : IToXml
    {
        #region Variables
        private string entity = string.Empty;
        private string fieldName = string.Empty;
        private string value = string.Empty;

        private const string CRITERIA = "<Criteria Entity='{0}' FieldName ='{1}'>{2}</Criteria>";
        #endregion

        #region Properties
        /// <summary>
        /// Entity where the fields belongs to
        /// </summary>
        public string Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.Validate(value, "Entity");
                this.entity = value;
            }
        }

        /// <summary>
        /// Field from the entity
        /// </summary>
        public string FieldName
        {
            get
            {
                return this.fieldName;
            }
            set
            {
                this.Validate(value, "FieldName");
                this.fieldName = value;
            }
        }

        /// <summary>
        /// Value to look for
        /// </summary>
        public string Value
        {
            get
            {
                this.Validate(value, "Value");
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of NativeCriteria
        /// </summary>
        /// <param name="pEntity">Entity where the fields belongs to</param>
        /// <param name="pFieldName">Field from the entity</param>
        /// <param name="pValue">Value to look for</param>
        public NativeCriteria(string pEntity, string pFieldName, string pValue)
        {
            this.Entity = pEntity;
            this.FieldName = pFieldName;
            this.Value = pValue;
        }

        /// <summary>
        /// Validate a value
        /// </summary>
        /// <param name="pValueToValidate">Value to validate</param>
        /// <param name="pParameterName">Parameter name</param>
        private void Validate(string pValueToValidate, string pParameterName)
        {
            if (string.IsNullOrWhiteSpace(pValueToValidate))
            {
                throw new ArgumentException("The value can not be empty", pParameterName);
            }
        }

        /// <summary>
        /// Converts the NativeCriteria to its XML representation
        /// </summary>
        /// <returns>Xml representation of the NativeCriteria</returns>
        public string ToXml()
        {
            return string.Format(CRITERIA, this.entity, this.fieldName, this.value);
        }

        public override string ToString()
        {
            return this.ToXml();
        }
    }
}
