using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OTE.eAdapter.DataTypes;
using OTE.eAdapter.DataTypes.Native;



namespace WcfRadiant
{    
    public class Service1 : IService1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ping structure</returns>
        public ping checkHeartbeat()
        {
            return new ping();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userCode">Receive a user code.</param>
        /// <returns>Return a user data structure.</returns>
        public List<StaffData> getUser(string userCode)//userData getUser(string userCode)
        {            
            NativeRequest request = new NativeRequest(NativeType.Staff);

            List<NativeCriteria> criteria = new List<NativeCriteria>();

            if (!string.IsNullOrWhiteSpace(userCode))
            {
                criteria.Add(new NativeCriteria("OrgHeader", "Code", userCode));
            }
          
            request.ConditionGroups.Add(new NativeCriteriaGroup(criteria, CriteriaGroupMatchType.Partial));

            string responseTest = eAdapterClientSingleton.Native.RetrieveData(request.ToXml());

            List<StaffData> nativeOrganizations = eAdapterDataTypesFactory.UnwrapMessage<StaffData>(responseTest);

            return nativeOrganizations;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationCode">Receive a station code.</param>
        /// <returns>Return a user data array.</returns>
        public userData[] getUserList(string stationCode)
        {
            return new userData[3];
        }

        /// <summary>
        /// Receive the user information and create new.
        /// </summary>
        /// <param name="loginName">Login name.</param>
        /// <param name="fullName">Full name</param>
        /// <param name="password">Password</param>
        /// <param name="address1">Address</param>
        /// <param name="city">City</param>
        /// <returns></returns>
        public userData createUser(string loginName, string fullName, string password, string address1, string city)
        {
            userData user = new userData();

            user.loginName = loginName;
            user.fullName = fullName;
            user.password = password;
            user.homeBranch = city;

            return user;
        }
    }
}
