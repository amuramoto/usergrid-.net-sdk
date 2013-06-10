﻿using System.Collections;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Usergrid.Sdk.Model;
using Usergrid.Sdk.Manager;
using Usergrid.Sdk.Payload;

namespace Usergrid.Sdk
{
    public class Client : IClient
    {
        private const string UserGridEndPoint = "http://api.usergrid.com";
        private readonly IUsergridRequest _request;

        private IEntityManager _entityManager;
		private IAuthenticationManager _authenticationManager;
		private IConnectionManager _connectionManager;
        private INotificationsManager _notificationsManager;

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return _authenticationManager ?? (_authenticationManager = new AuthenticationManager (_request));
			}
		}

		private IEntityManager EntityManager 
		{
			get
			{
				return _entityManager ?? (_entityManager = new EntityManager (_request));
			}
		}

		private IConnectionManager ConnectionManager
		{
			get
			{
				return _connectionManager ?? (_connectionManager = new ConnectionManager (_request));
			}
		}

        private INotificationsManager NotificationsManager
		{
			get
			{
                return _notificationsManager ?? (_notificationsManager = new NotificationsManager(_request));
			}
		}

        public Client(string organization, string application)
            : this(organization, application, UserGridEndPoint, new UsergridRequest(UserGridEndPoint, organization, application))
        {
        }

        public Client(string organization, string application, string uri = UserGridEndPoint)
            : this(organization, application, uri, new UsergridRequest(uri, organization, application))
        {
        }

        internal Client(string organization, string application, string uri = UserGridEndPoint, IUsergridRequest request = null)
        {
            _request = request ?? new UsergridRequest(uri, organization, application);
        }

        public void Login(string loginId, string secret, AuthType authType)
        {
			AuthenticationManager.Login (loginId, secret, authType);
        }

        public void CreateEntity<T>(string collection, T entity = null) where T : class
        {
            EntityManager.CreateEntity(collection,entity);
        }

        public void DeleteEntity(string collection, string name)
        {
			EntityManager.DeleteEntity(collection, name);
        }

        public void UpdateEntity<T>(string collection, string identifier, T entity)
        {
			EntityManager.UpdateEntity (collection, identifier, entity);
        }

        public UsergridEntity<T> GetEntity<T>(string collectionName, string identifer)
        {
		    return EntityManager.GetEntity<T>(collectionName, identifer);
		}

        public T GetUser<T>(string identifer /*username or uuid or email*/) where T : UsergridUser
        {
			var user = GetEntity<T> ("users", identifer);
			if (user == null)
				return null;

			return user.Entity;
        }

        public void CreateUser<T>(T user) where T : UsergridUser
        {
            CreateEntity("users", user);
        }

		public void UpdateUser<T>(T user) where T : UsergridUser
		{
            UpdateEntity("users", user.UserName, user);
		}

        public void DeleteUser(string identifer /*username or uuid or email*/)
		{
            DeleteEntity("users", identifer);
		}

        public void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			AuthenticationManager.ChangePassword (userName, oldPassword, newPassword);
		}

		public void CreateGroup<T>(T group) where T : UsergridGroup
		{
			CreateEntity ("groups", group);
		}

		public void DeleteGroup(string path)
		{
			DeleteEntity ("groups", path);
		}

        public T GetGroup<T>(string identifer/*uuid or path*/) where T : UsergridGroup
		{
            var usergridEntity = EntityManager.GetEntity<T>("groups", identifer);
			if (usergridEntity == null)
				return null;

            return usergridEntity.Entity; 
		}

		public void UpdateGroup<T>(T group) where T : UsergridGroup
		{
			UpdateEntity<T> ("groups", group.Path, group);
		}

        public void AddUserToGroup(string groupName, string userName)
        {
			CreateEntity<object> (string.Format("/groups/{0}/users/{1}", groupName, userName));
		}

        public void DeleteUserFromGroup(string groupName, string userName)
        {
            DeleteEntity("groups/" + groupName + "/users", userName );
		}

        public IList<T> GetAllUsersInGroup<T>(string groupName) where T : UsergridUser
        {
            var response = _request.ExecuteJsonRequest(string.Format("/groups/{0}/users", groupName) , Method.GET);
            ValidateResponse(response);

            var responseObject = JsonConvert.DeserializeObject<UsergridGetResponse<T>>(response.Content);
            return responseObject.Entities;
        }

		public UsergridCollection<UsergridEntity<T>> GetEntities<T>(string collection, int limit = 10, string query = null)
		{
			return EntityManager.GetEntities<T>(collection, limit, query);
		}

		public UsergridCollection<UsergridEntity<T>> GetNextEntities<T>(string collection, string query = null)
		{
			return EntityManager.GetNextEntities<T> (collection);
		}

		public UsergridCollection<UsergridEntity<T>> GetPreviousEntities<T>(string collection, string query = null)
		{
			return EntityManager.GetPreviousEntities<T> (collection);
		}

		public void CreateConnection<TConnector, TConnectee> (TConnector connector, TConnectee connectee, string connection) where TConnector : Usergrid.Sdk.Model.UsergridEntity where TConnectee : Usergrid.Sdk.Model.UsergridEntity
        {
			ConnectionManager.CreateConnection (connector, connectee, connection);
        }

		public IList<UsergridEntity> GetConnections<TConnector> (TConnector connector, string connection) where TConnector : Usergrid.Sdk.Model.UsergridEntity
		{
			return ConnectionManager.GetConnections<TConnector> (connector, connection);
		}

		public IList<UsergridEntity<TConnectee>> GetConnections<TConnector, TConnectee> (TConnector connector, string connection) where TConnector : Usergrid.Sdk.Model.UsergridEntity where TConnectee : Usergrid.Sdk.Model.UsergridEntity
		{
			return ConnectionManager.GetConnections<TConnector, TConnectee> (connector, connection);
		}

		public void DeleteConnection<TConnector, TConnectee> (TConnector connector, TConnectee connectee, string connection) where TConnector : Usergrid.Sdk.Model.UsergridEntity where TConnectee : Usergrid.Sdk.Model.UsergridEntity
		{
			ConnectionManager.DeleteConnection (connector, connectee, connection);
		}

		public void PostActivity<T>(string userIdentifier, T activity) where T:UsergridActivity
		{
			var collection = string.Format ("/users/{0}/activities", userIdentifier);
			EntityManager.CreateEntity (collection, activity);
		}

		public void PostActivityToGroup<T>(string groupIdentifier, T activity) where T:UsergridActivity
		{
			var collection = string.Format ("/groups/{0}/activities", groupIdentifier);
			EntityManager.CreateEntity (collection, activity);
		}

		public void PostActivityToUsersFollowersInGroup<T>(string userIdentifier, string groupIdentifier, T activity) where T:UsergridActivity
		{
			var collection = string.Format ("/groups/{0}/users/{1}/activities", groupIdentifier, userIdentifier);
			EntityManager.CreateEntity (collection, activity);
		}

		public UsergridCollection<UsergridEntity<T>> GetUserActivities<T>(string userIdentifier) where T:UsergridActivity
		{
			var collection = string.Format ("/users/{0}/activities", userIdentifier);
			return EntityManager.GetEntities<T> (collection);
		}

		public UsergridCollection<UsergridEntity<T>> GetGroupActivities<T>(string groupIdentifier) where T:UsergridActivity
		{
			var collection = string.Format ("/groups/{0}/activities", groupIdentifier);
			return EntityManager.GetEntities<T> (collection);
		}

		public T GetDevice<T>(string identifer) where T : UsergridDevice
		{
			var device = GetEntity<T> ("devices", identifer);
			if (device == null)
				return null;

			return device.Entity;
		}

		public void UpdateDevice<T>(T device) where T : UsergridDevice
		{
			UpdateEntity("devices", device.Name, device);
		}

		public void CreateDevice<T>(T device) where T : UsergridDevice
		{
			CreateEntity ("devices", device);
		}

		public void DeleteDevice(string identifer)
		{
			DeleteEntity("devices", identifer);
		}

        public void CreateNotifierForApple(string notifierName, string environment, string p12CertificatePath)
        {
            NotificationsManager.CreateNotifierForApple(notifierName,environment,p12CertificatePath);
        }
        
        public void CreateNotifierForAndroid(string notifierName, string apiKey)
        {
            NotificationsManager.CreateNotifierForAndroid(notifierName,apiKey);
        }

        public T GetNotifier<T>(string identifer/*uuid or notifier name*/) where T : UsergridNotifier
        {
            var usergridNotifier = EntityManager.GetEntity<T>("notifiers", identifer);
            if (usergridNotifier == null)
				return null;

            return usergridNotifier.Entity; 
        }

        public void DeleteNotifier(string notifierName)
        {
            EntityManager.DeleteEntity("/notifiers", notifierName);
        }

		public void PublishNotification (IEnumerable<Notification> notifications, INotificationRecipients recipients, NotificationSchedulerSettings schedulerSettings = null )
		{
			NotificationsManager.PublishNotification (notifications, recipients, schedulerSettings);
		}

        public void CancelNotification(string notificationIdentifier)
        {
            EntityManager.UpdateEntity("notifications",notificationIdentifier, new {canceled = true});
        }

		//TODO: IList?
		public UsergridCollection<UsergridEntity<T>> GetUserFeed<T>(string userIdentifier) where T:UsergridActivity
		{
			var collection = string.Format ("/users/{0}/feed", userIdentifier);
			return EntityManager.GetEntities<T> (collection);
		}

		public UsergridCollection<UsergridEntity<T>> GetGroupFeed<T>(string groupIdentifier) where T:UsergridActivity
		{
			var collection = string.Format ("/groups/{0}/feed", groupIdentifier);
			return EntityManager.GetEntities<T> (collection);
		}

        private static void ValidateResponse(IRestResponse response)
		{
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				var userGridError = JsonConvert.DeserializeObject<UsergridError> (response.Content);
				throw new UsergridException (userGridError);
			}
		}
    }
}