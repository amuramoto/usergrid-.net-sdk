﻿using System.Collections.Generic;
using Usergrid.Sdk.Model;

namespace Usergrid.Sdk
{
    public interface IClient
    {
        void Login(string loginId, string secret, AuthType authType);
        void CreateEntity<T>(string collection, T entity = null) where T : class;
        void DeleteEntity(string collection, string name);
        void UpdateEntity<T>(string collection, string identifier, T entity);
        UsergridEntity<T> GetEntity<T>(string collectionName, string identifer);
        T GetUser<T>(string identifer /*username or uuid or email*/) where T : UsergridUser;
        void CreateUser<T>(T user) where T : UsergridUser;
        void UpdateUser<T>(T user) where T : UsergridUser;
        void DeleteUser(string identifer /*username or uuid or email*/);
        void ChangePassword(string userName, string oldPassword, string newPassword);
        void CreateGroup<T>(T group) where T : UsergridGroup;
        void DeleteGroup(string path);
        T GetGroup<T>(string identifer /*uuid or path*/) where T : UsergridGroup;
        void UpdateGroup<T>(T group) where T : UsergridGroup;
        void AddUserToGroup(string groupName, string userName);
        void DeleteUserFromGroup(string groupName, string userName);
        IList<T> GetAllUsersInGroup<T>(string groupName) where T : UsergridUser;
        UsergridCollection<UsergridEntity<T>> GetEntities<T>(string collection, int limit = 10, string query = null);
        UsergridCollection<UsergridEntity<T>> GetNextEntities<T>(string collection, string query = null);
        UsergridCollection<UsergridEntity<T>> GetPreviousEntities<T>(string collection, string query = null);
        void CreateConnection<TConnector, TConnectee>(TConnector connector, TConnectee connectee, string connection) where TConnector : UsergridEntity where TConnectee : UsergridEntity;
        IList<UsergridEntity> GetConnections<TConnector>(TConnector connector, string connection) where TConnector : UsergridEntity;
        IList<UsergridEntity<TConnectee>> GetConnections<TConnector, TConnectee>(TConnector connector, string connection) where TConnector : UsergridEntity where TConnectee : UsergridEntity;
        void DeleteConnection<TConnector, TConnectee>(TConnector connector, TConnectee connectee, string connection) where TConnector : UsergridEntity where TConnectee : UsergridEntity;
        void PostActivity<T>(string userIdentifier, T activity) where T:UsergridActivity;
        void PostActivityToGroup<T>(string groupIdentifier, T activity) where T:UsergridActivity;
        void PostActivityToUsersFollowersInGroup<T>(string userIdentifier, string groupIdentifier, T activity) where T:UsergridActivity;
        UsergridCollection<UsergridEntity<T>> GetUserActivities<T>(string userIdentifier) where T:UsergridActivity;
        UsergridCollection<UsergridEntity<T>> GetGroupActivities<T>(string groupIdentifier) where T:UsergridActivity;
        void CreateNotifierForApple(string notifierName, string environment, string p12CertificatePath);
        void CreateNotifierForAndroid(string notifierName, string apiKey);
        T GetNotifier<T>(string identifer/*uuid or notifier name*/) where T : UsergridNotifier;
        void DeleteNotifier(string notifierName);
        UsergridCollection<UsergridEntity<T>> GetUserFeed<T>(string userIdentifier) where T:UsergridActivity;
        UsergridCollection<UsergridEntity<T>> GetGroupFeed<T>(string groupIdentifier) where T:UsergridActivity;
        T GetDevice<T>(string identifer) where T : UsergridDevice;
        void UpdateDevice<T>(T device) where T : UsergridDevice;
        void CreateDevice<T>(T device) where T : UsergridDevice;
        void DeleteDevice(string identifer);
        void PublishNotification (IEnumerable<Notification> notifications, INotificationRecipients recipients, NotificationSchedulerSettings schedulerSettings = null );
        void CancelNotification(string notificationIdentifier);
    }
}