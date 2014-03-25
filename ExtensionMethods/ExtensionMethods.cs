//-----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Tiempo Development">
//     Copyright Tiempo Development. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace WcfRadiant
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Text;
	//using TD.Utils.ExtensionMethods.Text;
    using WcfRadiant.Utils.ExtensionMethods.Text;
	using System.Data;    

	/// <summary>
	/// Class for Extension methods
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Returns the first element of a sequence, or a null value if the sequence contains no elements.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="source">The IEnumerable(T) to return the first element of.</param>
		/// <returns>null if source is empty; otherwise, the first element in source.</returns>
		public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : struct
		{
			try
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}

				IList<TSource> list = source as IList<TSource>;

				if (list != null)
				{
					if (list.Count > 0)
					{
						return list[0];
					}
				}
				else
				{
					using (IEnumerator<TSource> enumerator = source.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							return enumerator.Current;
						}
					}
				}

				return null;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Converts a field to the specified type
		/// </summary>
		/// <typeparam name="T">Type to be converted</typeparam>
		/// <param name="pSqlDataReader">Data reader</param>
		/// <param name="pFieldName">Name of the field</param>
		/// <returns>The converted value of the field</returns>
		public static T Field<T>(this System.Data.SqlClient.SqlDataReader pSqlDataReader, string pFieldName)
		{
			try 
			{
				if (pSqlDataReader[pFieldName] is DBNull)
				{
					return default(T);
				}
				else
				{
					return (T)pSqlDataReader[pFieldName]/*.ToString()*/;
				}
			}
			catch
			{		
				throw;
			}
		}

		/// <summary>
		/// Performs a deep clone of the object
		/// </summary>
		/// <typeparam name="T">Object type to be cloned</typeparam>
		/// <param name="pObjectToClone">Object to be cloned</param>
		/// <returns>A deep clone of the object</returns>
		public static T DeepClone<T>(this T pObjectToClone)
		{
			try
			{
				// Check if null
				if (pObjectToClone == null)
				{
					throw new ArgumentNullException("pObjectToClone");
				}

				// Start deep clone
				return (T)DeepCloneProcess(pObjectToClone, new Dictionary<object, object>(), 0);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Performs a deep clone of the object
		/// </summary>
		/// <typeparam name="T">Object type to be cloned</typeparam>
		/// <param name="pObjectToClone">Object to be cloned</param>
		/// <param name="pBaseClassDeepLevel">How many base classes deep to clone</param>
		/// <returns>A deep clone of the object</returns>
		public static T DeepClone<T>(this T pObjectToClone, int pBaseClassDeepLevel)
		{
			try
			{
				// Check if null
				if (pObjectToClone == null)
				{
					throw new ArgumentNullException("pObjectToClone");
				}

				// Start deep clone
				return (T)DeepCloneProcess(pObjectToClone, new Dictionary<object, object>(), pBaseClassDeepLevel);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Recursively perform a deep clone of an object
		/// </summary>
		/// <param name="pObjectToClone">Object to be cloned</param>
		/// <param name="pClonedObjectsCircularReference">Dictionary of already cloned objects.
		/// To avoid circular references and objects referenced multiple times</param>
		/// <param name="pBaseClassDeepLevel">How many base classes deep to clone</param>
		/// <returns>A deep clone of the object</returns>
		private static object DeepCloneProcess(object pObjectToClone, Dictionary<object, object> pClonedObjectsCircularReference, int pBaseClassDeepLevel)
		{
			try
			{
				// If null, just return null
				if (pObjectToClone == null)
				{
					return null;
				}

				// Object type
				Type objectToCloneType = pObjectToClone.GetType();

				// Return the value if it's a value type
				if (objectToCloneType.IsValueType || objectToCloneType == typeof(string))
				{
					return pObjectToClone;
				}

				// If the object to clone was already on the Dictionary of cloned objects, returns the reference to the cloned one
				if (pClonedObjectsCircularReference.ContainsKey(pObjectToClone))
				{
					return pClonedObjectsCircularReference[pObjectToClone];
				}

				// If array
				if (objectToCloneType.IsArray)
				{
					// Get name of the type of the elements in the array
					string typeNoArray = objectToCloneType.FullName.Replace("[]", string.Empty);

					// Elements type
					Type elementType = Type.GetType(string.Format("{0}, {1}", typeNoArray, objectToCloneType.Assembly.FullName));

					// Declare new array to read from
					var arrayToReadFrom = pObjectToClone as Array;

					// Array to save cloned objects
					Array clonedArray = Array.CreateInstance(elementType, arrayToReadFrom.Length);

					// Save the array to the Dictionary of cloned objects
					// This was done as first step in case in an inner object there is a reference to this object
					pClonedObjectsCircularReference[pObjectToClone] = clonedArray;

					// Set each value from the array
					for (int i = 0; i < arrayToReadFrom.Length; i++)
					{
						// Get element
						object element = arrayToReadFrom.GetValue(i);

						// Set a the cloned element to the array
						clonedArray.SetValue(DeepCloneProcess(element, pClonedObjectsCircularReference, pBaseClassDeepLevel), i);
					}

					// Return the array
					return Convert.ChangeType(clonedArray, pObjectToClone.GetType());
				}

				// If class
				if (objectToCloneType.IsClass)
				{
					// Get a new instance of the object
					object clonedClass = null;
					try
					{
						clonedClass = Activator.CreateInstance(objectToCloneType);
					}
					catch
					{
						clonedClass = FormatterServices.GetUninitializedObject(objectToCloneType);
					}

					// To control how deep the current base class is
					int currentBaseClassDeepLevel = 0;

					// To clone the properties of the base class, but just 1 level deep
					while (objectToCloneType != null && currentBaseClassDeepLevel <= pBaseClassDeepLevel)
					{
						// Get object fields
						FieldInfo[] objectFields = objectToCloneType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

						if (objectFields.Length > 0)
						{
							// Save the array to the Dictionary of cloned objects
							// This was done as first step in case in an inner object there is a reference to this object
							pClonedObjectsCircularReference[pObjectToClone] = clonedClass;

							// For each field
							foreach (FieldInfo objectField in objectFields)
							{
								// Get value
								object fieldValue = objectField.GetValue(pObjectToClone);

								// Set the cloned value to the cloned class
								objectField.SetValue(clonedClass, DeepCloneProcess(fieldValue, pClonedObjectsCircularReference, pBaseClassDeepLevel));
							}
						}

						// Move to base class
						objectToCloneType = objectToCloneType.BaseType;

						// One level deeper
						currentBaseClassDeepLevel++;
					}

					// Return the cloned class
					return clonedClass;
				}

				// If this line is reached, it means the type is not supported
				throw new ArgumentException(string.Format("The type \"{0}\" is not supported", objectToCloneType.Name), "pObjectToClone");
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Returns true if the object is contained in the set of values, like the IN clause from SQL
		/// </summary>
		/// <typeparam name="T">Object type to be compared</typeparam>
		/// <param name="pObject">Object to compare</param>
		/// <param name="pValues">Set of values to compare to</param>
		/// <returns>Returns true if the object is in the set of values</returns>
		public static bool In<T>(this T pObject, params T[] pValues)
		{
			try
			{
				if (pObject == null)
				{
					throw new ArgumentNullException("pObject");
				}

				foreach (T value in pValues)
				{
					if (pObject.Equals(value))
					{
						return true;
					}
				}

				return false;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Converts Boolean values to Bit (MSSQL)
		/// </summary>
		/// <param name="pBool">Boolean value to be converted</param>
		/// <returns>Bit value</returns>
		public static int BooleanToBit(this bool pBool)
		{
			if (pBool == true)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Convert SecureString to unsecure string.
		/// </summary>
		/// <param name="pSecurePassword">Pass SecureString for conversion.</param>
		/// <returns>Unsecure string</returns>
		public static string ToUnsecureString(this SecureString pSecurePassword)
		{
			try
			{
				if (pSecurePassword == null)
				{
					throw new ArgumentNullException("pSecurePassword");
				}

				IntPtr unmanagedString = IntPtr.Zero;

				try
				{
					unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(pSecurePassword);
					return Marshal.PtrToStringUni(unmanagedString);
				}
				finally
				{
					Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Retrieves a property from an object by property name
		/// </summary>
		/// <param name="pObject">The object</param>
		/// <param name="pPropertyName">The name of the property</param>
		/// <returns>The property</returns>
		private static object RetrievePropertyValue(this object pObject, string pPropertyName)
		{
			foreach (string part in pPropertyName.Split('.'))
			{
				if (pObject == null) 
				{ 
					return null; 
				}

				Type type = pObject.GetType();
				PropertyInfo info = type.GetProperty(part);

				if (info == null)
				{
					FieldInfo fieldInfo = type.GetField(part);

					if (fieldInfo == null)
					{
						return null;
					}
					else
					{
						pObject = fieldInfo.GetValue(pObject);
					}
				}
				else
				{
					pObject = info.GetValue(pObject, null);
				}
			}

			return pObject;
		}

		/// <summary>
		/// Retrieves a property from an object by property name
		/// </summary>
		/// <typeparam name="T">The type of the property to return</typeparam>
		/// <param name="pObject">The object</param>
		/// <param name="pPropertyName">The name of the property</param>
		/// <returns>The property</returns>
		public static T RetrievePropertyValue<T>(this object pObject, string pPropertyName)
		{
			object retval = pObject.RetrievePropertyValue(pPropertyName);
			
			if (retval == null) 
			{ 
				return default(T); 
			}

			// throws InvalidCastException if types are incompatible
			return (T)retval;
		}

		/// <summary>
		/// Calculates the next date occurrence based on the recurrence pattern
		/// </summary>
		/// <param name="pDate">Date</param>
		/// <param name="pRecurrence">Recurrence pattern</param>
		/// <returns>Next occurrence</returns>
		public static DateTime CalculateNextOccurrence(this DateTime pDate, string pRecurrence)
		{
			return pDate.CalculateNextOccurrence(pDate, pRecurrence);
		}

		/// <summary>
		/// Calculates the next date occurrence based on the recurrence pattern
		/// </summary>
		/// <param name="pDate">Date</param>
		/// <param name="pTime">Time</param>
		/// <param name="pRecurrence">Recurrence pattern</param>
		/// <returns>Next occurrence</returns>
		public static DateTime CalculateNextOccurrence(this DateTime pDate, DateTime pTime, string pRecurrence)
		{
			if (string.IsNullOrWhiteSpace(pRecurrence))
			{
				throw new ArgumentException("Missing value for Recurrence", "pRecurrence");
			}

			bool dateInThePast = false;

			DateTime calculatedDate = pDate;

			if (calculatedDate < DateTime.Now)
			{
				dateInThePast = true;
				calculatedDate = DateTime.Now;
			}

			// Remove current time from date
			calculatedDate = calculatedDate.Subtract(calculatedDate.TimeOfDay);

			// Add Time parameter
			calculatedDate = calculatedDate.Add(pTime.TimeOfDay);

			pRecurrence = pRecurrence.ToLowerInvariant();

			switch (pRecurrence.Left(1))
			{
				case "d":
					if (pRecurrence.Length > 2)
					{
						throw new ArgumentException("Invalid value for Recurrence", "pRecurrence");
					}

					if (calculatedDate < DateTime.Now || !dateInThePast)
					{
						calculatedDate = calculatedDate.AddDays(1); 
					}

					// Exclude weekends
					if (pRecurrence.Length == 2 && pRecurrence.Right(1) == "w")
					{
						if (calculatedDate.DayOfWeek == DayOfWeek.Saturday)
						{
							calculatedDate = calculatedDate.AddDays(2);
						}
						else if (calculatedDate.DayOfWeek == DayOfWeek.Sunday)
						{
							calculatedDate = calculatedDate.AddDays(1);
						}
					}

					break;
				case "m":
					if (pRecurrence.Length == 1 || pRecurrence.Length > 3)
					{
						throw new ArgumentException("Invalid value for Recurrence", "pRecurrence");
					}

					int day = int.Parse(pRecurrence.Substring(1));

					// Create date
					DateTime tempCalculatedDate = DateTime.Parse("1/1/" + calculatedDate.Year.ToString()).AddDays(day - 1).AddMonths(calculatedDate.Month - 1);
					
					// Add time
					tempCalculatedDate = tempCalculatedDate.Add(calculatedDate.TimeOfDay);

					// If the new next report date is in the past, add one month
					if (!(tempCalculatedDate > calculatedDate))
					{
						tempCalculatedDate = tempCalculatedDate.AddMonths(1);
					}

					calculatedDate = tempCalculatedDate;

					break;
				case "w":
					if (pRecurrence.Length == 1 || pRecurrence.Length > 8)
					{
						throw new ArgumentException("Invalid value for Recurrence", "pRecurrence");
					}

					List<int> days = pRecurrence.Substring(1).ToCharArray().Select(d => int.Parse(d.ToString())).OrderBy(i => i).ToList();

					int dayOfWeek = ((int)calculatedDate.DayOfWeek + 1);

					// Get the next day from the list, that is after the day of week on calculatedDate
					List<int> daysAfter = days.Where(d => d >= dayOfWeek).ToList();

					DateTime tempCalculatedDateWeekly = DateTime.MinValue;

					if (daysAfter != null && daysAfter.Count > 0)
					{
						// Current week
						tempCalculatedDateWeekly = calculatedDate.AddDays((daysAfter.First() - dayOfWeek) % 7);

						// If the new next report date is in the past, remove the day and try with the next one on the list
						if (!(tempCalculatedDateWeekly > calculatedDate) && DateTime.Now > tempCalculatedDateWeekly)
						{
							daysAfter.RemoveAt(0);

							if (daysAfter != null && daysAfter.Count > 0)
							{
								// Current week
								tempCalculatedDateWeekly = calculatedDate.AddDays((daysAfter.First() - dayOfWeek) % 7);
							}
							else
							{
								// Next week
								tempCalculatedDateWeekly = calculatedDate.AddDays(7 - dayOfWeek + days.First());
							}
						}
					}
					else
					{
						// Next week
						tempCalculatedDateWeekly = calculatedDate.AddDays(7 - dayOfWeek + days.First());
					}

					calculatedDate = tempCalculatedDateWeekly;

					break;

				default:
					throw new ArgumentException("Invalid value for Recurrence", "pRecurrence");
			}

			return calculatedDate;
		}

		/// <summary>
		/// Trims a sequence of pNumbers from (1,2,3,4) to (1-4)
		/// </summary>
		/// <param name="pNumbers">List of numbers</param>
		/// <returns>Trimmed sequence</returns>
		private static string TrimSequence(this List<int> pNumbers)
		{
			if (pNumbers == null || pNumbers.Count == 0)
			{
				return string.Empty;
			}

			pNumbers = pNumbers.OrderBy(n => n).ToList();

			StringBuilder sequence = new StringBuilder();

			int previousNumber = pNumbers.First();
			sequence.Append(previousNumber);
			int sequenceCount = 1;

			for (int i = 1; i < pNumbers.Count; i++)
			{
				if (pNumbers[i] - 1 != previousNumber)
				{
					if (sequenceCount > 2)
					{
						sequence.AppendFormat("-{0}, {1}", previousNumber, pNumbers[i]);
					}
					else if (sequenceCount == 2)
					{
						sequence.AppendFormat(", {0}, {1}", previousNumber, pNumbers[i]);
					}
					else
					{
						sequence.AppendFormat(", {0}", pNumbers[i]);
					}

					sequenceCount = 1;
				}
				else
				{
					sequenceCount++;
				}

				previousNumber = pNumbers[i];
			}

			if (sequenceCount > 2)
			{
				sequence.AppendFormat("-{0}", previousNumber);
			}
			else if (sequenceCount == 2)
			{
				sequence.AppendFormat(", {0}", previousNumber);
			}

			return sequence.ToString();
		}

		/// <summary>
		/// Convert a IEnumerable{T} to a DataTable.
		/// </summary>
		/// <param name="items">Items</param>
		/// <returns>DataTable</returns>
		public static DataTable ToDataTable<T>(this IEnumerable<T> items)
		{
			var tb = new DataTable(typeof (T).Name);

			PropertyInfo[] props = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (PropertyInfo prop in props)
			{
				Type t = GetCoreType(prop.PropertyType);
				tb.Columns.Add(prop.Name, t);
			}

			foreach (T item in items)
			{
				var values = new object[props.Length];

				for (int i = 0; i < props.Length; i++)
				{
					values[i] = props[i].GetValue(item, null);
				}

				tb.Rows.Add(values);
			}

			return tb;
		}

		/// <summary>
		/// Determine of specified type is Nullable
		/// </summary>
		private static bool IsNullable(Type t)
		{
			return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		/// <summary>
		/// Return underlying type if type is Nullable otherwise return the type
		/// </summary>
		private static Type GetCoreType(Type t)
		{
			if (t != null && IsNullable(t))
			{
				if (!t.IsValueType)
				{
					return t;
				}
				else
				{
					return Nullable.GetUnderlyingType(t);
				}
			}
			else
			{
				return t;
			}
		}
	}
}