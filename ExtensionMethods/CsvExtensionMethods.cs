//-----------------------------------------------------------------------
// <copyright file="CsvExtensionMethods.cs" company="Tiempo Development">
//     Copyright Tiempo Development. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WcfRadiant.Utils.ExtensionMethods.Csv//TD.Utils.ExtensionMethods.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Utils.ExtensionMethods.Text;

    /// <summary>
    /// Class for Extension methods
    /// </summary>
    public static class CsvExtensionMethods
    {
        /// <summary>
        /// Appends the DataRow to the specified FileInfo
        /// </summary>
        /// <param name="pDataRow">DataRow to be added</param>
        /// <param name="pDestination">FileInfo where the DataRow will be added</param>
        public static void AppendToCsv(this DataRow pDataRow, FileInfo pDestination)
        {
            try
            {
                using (FileStream destination = new FileStream(pDestination.FullName, FileMode.Append))
                {
                    // Save the rows to a new csv
                    using (StreamWriter writer = new StreamWriter(destination, Encoding.Default))
                    {
                        writer.WriteLine(pDataRow.ToCsvFormat());
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a DataRow to the Csv format
        /// </summary>
        /// <param name="pDataRow">DataRow to be formatted</param>
        /// <param name="pColumnLength">Max column length. Set it to 0 for no restrictions</param>
        /// <param name="pRemoveQuotationMarks">Removes quotation marks</param>
        /// <returns>A string with the DataRow in Csv format</returns>
        public static string ToCsvFormat(this DataRow pDataRow, int pColumnLength, bool pRemoveQuotationMarks)
        {
            try
            {
                if (pColumnLength < 0)
                {
                    throw new ArgumentOutOfRangeException("pColumnLength", "The length must not be negative.");
                }

                StringBuilder lineBuilder = new StringBuilder();

                foreach (var currentColumn in pDataRow.ItemArray)
                {
                    //if (!(currentColumn is DBNull))
                    //{
                    //    if (currentColumn.ToString().Contains("\n"))
                    //    {
                    //    }
                    //}

                    lineBuilder.Append("\"");

                    if (!(currentColumn is DBNull))
                    {
                        string stringToAppend = string.Empty;

                        stringToAppend = currentColumn.ToString().Replace("\"", "\"\"").Replace("\n", string.Empty).Replace("\r", string.Empty);

                        if (pRemoveQuotationMarks)
                        {
                            stringToAppend = stringToAppend.Replace("\"", string.Empty);
                        }

                        if (pColumnLength > 0)
                        {
                            stringToAppend = stringToAppend.Left(pColumnLength);
                        }

                        lineBuilder.Append(stringToAppend);
                    }

                    lineBuilder.Append("\",");

                    //lineBuilder.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\",", !(currentColumn is DBNull) ? currentColumn.ToString().Replace("\"", "\"\"").Replace("\n", string.Empty).Replace("\r", string.Empty) : string.Empty);
                }

                lineBuilder.Remove(lineBuilder.Length - 1, 1);

                return lineBuilder.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts a DataRow to the Csv format
        /// </summary>
        /// <param name="pDataRow">DataRow to be formatted</param>
        /// <returns>A string with the DataRow in Csv format</returns>
        public static string ToCsvFormat(this DataRow pDataRow)
        {
            try
            {
                return pDataRow.ToCsvFormat(0, false);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a CSV text from a Data table
        /// </summary>
        /// <param name="pDataTable">Data table to be converted to Csv</param>
        /// <param name="pDestination">FileInfo for the destination file</param>
        public static void CreateCSV(this DataTable pDataTable, FileInfo pDestination)
        {
            try
            {
                using (FileStream destination = new FileStream(pDestination.FullName, FileMode.CreateNew))
                {
                    // Save the rows to a new csv
                    using (StreamWriter writer = new StreamWriter(destination, Encoding.Default))
                    {
                        StringBuilder lineBuilder = new StringBuilder();

                        // Get columns names
                        foreach (DataColumn col in pDataTable.Columns)
                        {
                            lineBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0},", col.ColumnName);
                        }

                        // If there are columns name, add them to csv file
                        if (lineBuilder.Length > 0)
                        {
                            lineBuilder.Remove(lineBuilder.Length - 1, 1);

                            writer.WriteLine(lineBuilder.ToString());
                        }

                        foreach (DataRow row in pDataTable.Rows)
                        {
                            string rowToString = row.ToCsvFormat();

                            if (!string.IsNullOrEmpty(rowToString))
                            {
                                writer.WriteLine(row.ToCsvFormat());
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a CSV file from a DataRow List
        /// </summary>
        /// <param name="pRowsCollection">List of DataRows to be converted to Csv</param>
        /// <param name="pDestination">FileInfo for the destination file</param>
        public static void CreateCSV(this List<DataRow> pRowsCollection, FileInfo pDestination)
        {
            try
            {
                pRowsCollection.CreateCSV(pDestination, null, 0, false);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a CSV file from a DataRow List
        /// </summary>
        /// <param name="pRowsCollection">List of DataRows to be converted to Csv</param>
        /// <param name="pDestination">FileInfo for the destination file</param>
        /// <param name="pColumns">Collection of DataColumn of the DataRow list</param>
        /// <param name="pColumnLength">Max column length. Set it to 0 for no restrictions</param>
        /// <param name="pRemoveQuotationMarks">Removes quotation marks</param>
        public static void CreateCSV(this List<DataRow> pRowsCollection, FileInfo pDestination, DataColumnCollection pColumns, int pColumnLength, bool pRemoveQuotationMarks)
        {
            try
            {
                using (FileStream destination = new FileStream(pDestination.FullName, FileMode.Create))
                {
                    // Save the rows to a new csv
                    using (StreamWriter writer = new StreamWriter(destination, Encoding.Default))
                    {
                        StringBuilder lineBuilder = new StringBuilder();

                        // Get columns names
                        if (pColumns != null)
                        {
                            foreach (DataColumn col in pColumns)
                            {
                                lineBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0},", col.ColumnName);
                            }
                        }

                        // If there are columns name, add them to csv file
                        if (lineBuilder.Length > 0)
                        {
                            lineBuilder.Remove(lineBuilder.Length - 1, 1);

                            writer.WriteLine(lineBuilder.ToString());
                        }

                        foreach (DataRow row in pRowsCollection)
                        {
                            writer.WriteLine(row.ToCsvFormat(pColumnLength, pRemoveQuotationMarks));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}