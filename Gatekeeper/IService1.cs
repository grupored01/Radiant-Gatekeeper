using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using OTE.eAdapter.DataTypes;
using OTE.eAdapter.DataTypes.Native;

namespace Gatekeeper
{
    [ServiceContract]
    public interface IService1
    {
        //Comment
        [OperationContract]
        [WebInvoke(UriTemplate = "checkHeartbeat", Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ping checkHeartbeat();

        //Comment2
        [OperationContract]
        [WebInvoke(UriTemplate = "getUser", Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<StaffData> getUser(string userCode);//getUser(string userCode);

        [OperationContract]
        [WebInvoke(UriTemplate = "getUserList", Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        userData[] getUserList(string stationCode);

        [OperationContract]
        [WebInvoke(UriTemplate = "createUser", Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        userData createUser(string loginName, string fullName, string password, string address1, string city);
    }


    [DataContract]
    public class ping
    {
        private bool _success = false;
        private int _timeStamp = 0;
        private float _responseTime = 0;

        [DataMember]
        public bool success
        {
            get { return _success; }
            set { _success = value; }
        }

        [DataMember]
        public int timeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        [DataMember]
        public float responseTime
        {
            get { return _responseTime; }
            set { _responseTime = value; }
        }
    }

    [DataContract]
    public class userData
    {
        private int _id = 0;
        private string _fullName = string.Empty;
        private string _userCode = string.Empty;
        private string _loginName = string.Empty;
        private string _password = string.Empty;
        private string _homeBranch = string.Empty;
        private string _homeDepartment = string.Empty;
        private bool _active = false;

        [DataMember]
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string fullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        [DataMember]
        public string userCode
        {
            get { return _userCode; }
            set { _userCode = value; }
        }

        [DataMember]
        public string loginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        [DataMember]
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        [DataMember]
        public string homeBranch
        {
            get { return _homeBranch; }
            set { _homeBranch = value; }
        }

        [DataMember]
        public string homeDepartment
        {
            get { return _homeDepartment; }
            set { _homeDepartment = value; }
        }

        [DataMember]
        public bool active
        {
            get { return _active; }
            set { _active = value; }
        }
    }
}
