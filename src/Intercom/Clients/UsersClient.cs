using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intercom.Clients;
using Intercom.Core;
using Intercom.Data;
using Intercom.Exceptions;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Intercom.Clients
{
    /// <summary>
    /// Client used to interact with users via the API
    /// </summary>
    public class UsersClient : Client
    {
        // TODO: Implement paging
        private static class UserSortBy
        {
            public const string created_at = "created_at";
            public const string updated_at = "updated_at";
            public const string signed_up_at = "signed_up_at";
        }

        private const string USERS_RESOURCE = "users";

        #region Constructors
        public UsersClient(Authentication authentication)
            : base(INTERCOM_API_BASE_URL, USERS_RESOURCE, authentication)
        {
        }

        public UsersClient(string intercomApiUrl, Authentication authentication)
            : base(string.IsNullOrEmpty(intercomApiUrl) ? INTERCOM_API_BASE_URL : intercomApiUrl, USERS_RESOURCE, authentication)
        {
        }
        #endregion

        #region Sync
        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The created user</returns>
        /// <remarks>https://developers.intercom.io/reference#create-or-update-user</remarks>
        public User Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.user_id) && string.IsNullOrEmpty(user.email))
            {
                throw new ArgumentException("you need to provide either 'user.user_id', 'user.email' to create a user.");
            }

            ClientResponse<User> result = null;
            result = Post<User>(user);
            return result.Result;
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns>The updated user</returns>
        /// <remarks>https://developers.intercom.io/reference#create-or-update-user</remarks>
        public User Update(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id) && string.IsNullOrEmpty(user.user_id) && string.IsNullOrEmpty(user.email))
            {
                throw new ArgumentException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            ClientResponse<User> result = null;
            result = Post<User>(user);

            return result.Result;
        }

        /// <summary>
        /// View a user using a dictionary of request params
        /// </summary>
        /// <param name="parameters">The request parameters</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public User View(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("'parameters' argument is null.");
            }

            if (!parameters.Any())
            {
                throw new ArgumentException("'parameters' argument should include user_id parameter.");
            }

            ClientResponse<User> result = null;

            result = Get<User>(parameters: parameters);
            return result.Result;
        }

        /// <summary>
        /// View a user, retrieving using the id
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public User View(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'parameters' argument is null.");
            }

            ClientResponse<User> result = null;
            result = Get<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + id);
            return result.Result;
        }

        /// <summary>
        /// View a user
        /// </summary>
        /// <param name="user">The user object</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public User View(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            ClientResponse<User> result = null;

            if (!string.IsNullOrEmpty(user.id))
            {
                result = Get<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + user.id);
            }
            else if (!string.IsNullOrEmpty(user.user_id))
            {
                parameters.Add(Constants.USER_ID, user.id);
                result = Get<User>(parameters: parameters);
            }
            else if (!string.IsNullOrEmpty(user.email))
            {
                parameters.Add(Constants.EMAIL, user.email);
                result = Get<User>(parameters: parameters);
            }
            else
            {
                throw new ArgumentNullException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            return result.Result;
        }

        /// <summary>
        /// Fetch a list of users
        /// </summary>
        /// <returns>The list of users</returns>
        /// <remarks>https://developers.intercom.io/reference#list-users</remarks>
        public Users List()
        {
            ClientResponse<Users> result = null;
            result = Get<Users>();
            return result.Result;
        }

        /// <summary>
        /// Fetch a list of users
        /// </summary>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The list of users</returns>
        /// <remarks>https://developers.intercom.io/reference#list-users</remarks>
        public Users List(Dictionary<string, string> parameters)
        {
            ClientResponse<Users> result = null;
            result = Get<Users>(parameters: parameters);
            return result.Result;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">The user to delete</param>
        /// <returns>The deleted user object</returns>
        /// <remarks>https://developers.intercom.io/reference#delete-a-user</remarks>
        public User Delete(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            ClientResponse<User> result = null;

            if (!string.IsNullOrEmpty(user.id))
            {
                result = Delete<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + user.id);
            }
            else if (!string.IsNullOrEmpty(user.user_id))
            {
                parameters.Add(Constants.USER_ID, user.id);
                result = Delete<User>(parameters: parameters);
            }
            else if (!string.IsNullOrEmpty(user.email))
            {
                parameters.Add(Constants.EMAIL, user.email);
                result = Delete<User>(parameters: parameters);
            }
            else
            {
                throw new ArgumentException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            return result.Result;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The deleted user object</returns>
        /// <remarks>https://developers.intercom.io/reference#delete-a-user</remarks>
        public User Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            result = Delete<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + id);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="timestamp">The new time the user was last seen</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public User UpdateLastSeenAt(string id, long timestamp)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            if (timestamp <= 0)
            {
                throw new ArgumentException("'timestamp' argument should be bigger than zero.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = id, last_request_at = timestamp });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen
        /// </summary>
        /// <param name="user"></param>
        /// <param name="timestamp">The new time the user was last seen</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public User UpdateLastSeenAt(User user, long timestamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            if (timestamp <= 0)
            {
                throw new ArgumentException("'timestamp' argument should be bigger than zero.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = user.id, last_request_at = timestamp });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen to "now"
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public User UpdateLastSeenAt(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = id, update_last_request_at = true });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen to "now"
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public User UpdateLastSeenAt(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = user.id, update_last_request_at = true });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Increment the session count for a user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#incrementing-user-sessions</remarks>
        public User IncrementUserSession(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = id, new_session = true });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Increment the session count for a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#incrementing-user-sessions</remarks>
        public User IncrementUserSession(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = user.id, new_session = true });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Remove a company (or companies) from a user
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="companyIds">The list of company ids</param>
        /// <returns>An updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#companies-and-users</remarks>
        public User RemoveCompanyFromUser(string userId, List<string> companyIds)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("'userId' argument is null.");
            }

            if (companyIds == null)
            {
                throw new ArgumentNullException("'companyIds' argument is null.");
            }

            if (!companyIds.Any())
            {
                throw new ArgumentException("'companyIds' shouldnt be empty.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = userId, companies = companyIds.Select(c => new { id = c, remove = true }) });
            result = Post<User>(body);
            return result.Result;
        }

        /// <summary>
        /// Remove a company (or companies) from a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="companyIds">The list of company ids</param>
        /// <returns>An updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#companies-and-users</remarks>
        public User RemoveCompanyFromUser(User user, List<string> companyIds)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'user.id' is null.");
            }

            if (companyIds == null)
            {
                throw new ArgumentNullException("'companyIds' argument is null.");
            }

            if (!companyIds.Any())
            {
                throw new ArgumentException("'companyIds' shouldnt be empty.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = user.id, companies = companyIds.Select(c => new { id = c, remove = true }) });
            result = Post<User>(body);
            return result.Result;
        }
        #endregion

        #region Async
        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The created user</returns>
        /// <remarks>https://developers.intercom.io/reference#create-or-update-user</remarks>
        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.user_id) && string.IsNullOrEmpty(user.email))
            {
                throw new ArgumentException("you need to provide either 'user.user_id', 'user.email' to create a user.");
            }

            ClientResponse<User> result = null;
            result = await PostAsync<User>(user).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns>The updated user</returns>
        /// <remarks>https://developers.intercom.io/reference#create-or-update-user</remarks>
        public async Task<User> UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id) && string.IsNullOrEmpty(user.user_id) && string.IsNullOrEmpty(user.email))
            {
                throw new ArgumentException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            ClientResponse<User> result = null;
            result = await PostAsync<User>(user).ConfigureAwait(false);

            return result.Result;
        }

        /// <summary>
        /// View a user using a dictionary of request params
        /// </summary>
        /// <param name="parameters">The request parameters</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public async Task<User> ViewAsync(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("'parameters' argument is null.");
            }

            if (!parameters.Any())
            {
                throw new ArgumentException("'parameters' argument should include user_id parameter.");
            }

            ClientResponse<User> result = null;

            result = await GetAsync<User>(parameters: parameters).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// View a user, retrieving using the id
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public async Task<User> ViewAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'parameters' argument is null.");
            }

            ClientResponse<User> result = null;
            result = await GetAsync<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + id).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// View a user
        /// </summary>
        /// <param name="user">The user object</param>
        /// <returns>A user object</returns>
        /// <remarks>https://developers.intercom.io/reference#view-a-user</remarks>
        public async Task<User> ViewAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            ClientResponse<User> result = null;

            if (!string.IsNullOrEmpty(user.id))
            {
                result = await GetAsync<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + user.id).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(user.user_id))
            {
                parameters.Add(Constants.USER_ID, user.id);
                result = await GetAsync<User>(parameters: parameters).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(user.email))
            {
                parameters.Add(Constants.EMAIL, user.email);
                result = await GetAsync<User>(parameters: parameters).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentNullException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            return result.Result;
        }

        /// <summary>
        /// Fetch a list of users
        /// </summary>
        /// <returns>The list of users</returns>
        /// <remarks>https://developers.intercom.io/reference#list-users</remarks>
        public async Task<Users> ListAsync()
        {
            ClientResponse<Users> result = null;
            result = await GetAsync<Users>().ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Fetch a list of users
        /// </summary>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The list of users</returns>
        /// <remarks>https://developers.intercom.io/reference#list-users</remarks>
        public async Task<Users> ListAsync(Dictionary<string, string> parameters)
        {
            ClientResponse<Users> result = null;
            result = await GetAsync<Users>(parameters: parameters).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">The user to delete</param>
        /// <returns>The deleted user object</returns>
        /// <remarks>https://developers.intercom.io/reference#delete-a-user</remarks>
        public async Task<User> DeleteAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            ClientResponse<User> result = null;

            if (!string.IsNullOrEmpty(user.id))
            {
                result = await DeleteAsync<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + user.id).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(user.user_id))
            {
                parameters.Add(Constants.USER_ID, user.id);
                result = await DeleteAsync<User>(parameters: parameters).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(user.email))
            {
                parameters.Add(Constants.EMAIL, user.email);
                result = await DeleteAsync<User>(parameters: parameters).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException("you need to provide either 'user.id', 'user.user_id', 'user.email' to view a user.");
            }

            return result.Result;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The deleted user object</returns>
        /// <remarks>https://developers.intercom.io/reference#delete-a-user</remarks>
        public async Task<User> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            result = await DeleteAsync<User>(resource: USERS_RESOURCE + Path.DirectorySeparatorChar + id).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="timestamp">The new time the user was last seen</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public async Task<User> UpdateLastSeenAtAsync(string id, long timestamp)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            if (timestamp <= 0)
            {
                throw new ArgumentException("'timestamp' argument should be bigger than zero.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = id, last_request_at = timestamp });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen
        /// </summary>
        /// <param name="user"></param>
        /// <param name="timestamp">The new time the user was last seen</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public async Task<User> UpdateLastSeenAtAsync(User user, long timestamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            if (timestamp <= 0)
            {
                throw new ArgumentException("'timestamp' argument should be bigger than zero.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = user.id, last_request_at = timestamp });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen to "now"
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public async Task<User> UpdateLastSeenAtAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = id, update_last_request_at = true });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Update the last time the user was seen to "now"
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns>The user object</returns>
        /// <remarks>https://developers.intercom.io/reference#updating-the-last-seen-time</remarks>
        public async Task<User> UpdateLastSeenAtAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { user_id = user.id, update_last_request_at = true });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Increment the session count for a user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#incrementing-user-sessions</remarks>
        public async Task<User> IncrementUserSessionAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = id, new_session = true });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Increment the session count for a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#incrementing-user-sessions</remarks>
        public async Task<User> IncrementUserSessionAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'id' argument is null.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = user.id, new_session = true });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Remove a company (or companies) from a user
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="companyIds">The list of company ids</param>
        /// <returns>An updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#companies-and-users</remarks>
        public async Task<User> RemoveCompanyFromUserAsync(string userId, List<string> companyIds)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("'userId' argument is null.");
            }

            if (companyIds == null)
            {
                throw new ArgumentNullException("'companyIds' argument is null.");
            }

            if (!companyIds.Any())
            {
                throw new ArgumentException("'companyIds' shouldnt be empty.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = userId, companies = companyIds.Select(c => new { id = c, remove = true }) });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }

        /// <summary>
        /// Remove a company (or companies) from a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="companyIds">The list of company ids</param>
        /// <returns>An updated user object</returns>
        /// <remarks>https://developers.intercom.io/reference#companies-and-users</remarks>
        public async Task<User> RemoveCompanyFromUserAsync(User user, List<string> companyIds)
        {
            if (user == null)
            {
                throw new ArgumentNullException("'user' argument is null.");
            }

            if (string.IsNullOrEmpty(user.id))
            {
                throw new ArgumentNullException("'user.id' is null.");
            }

            if (companyIds == null)
            {
                throw new ArgumentNullException("'companyIds' argument is null.");
            }

            if (!companyIds.Any())
            {
                throw new ArgumentException("'companyIds' shouldnt be empty.");
            }

            ClientResponse<User> result = null;
            string body = JsonConvert.SerializeObject(new { id = user.id, companies = companyIds.Select(c => new { id = c, remove = true }) });
            result = await PostAsync<User>(body).ConfigureAwait(false);
            return result.Result;
        }
        #endregion

        #region Helpers
        private User CreateOrUpdate(User user)
        {
            ClientResponse<User> result = null;
            result = Post<User>(user);
            return result.Result;
        }

        // TODO: Implement paging (by Pages argument)
        private Users Next(Pages pages)
        {
            return null;
        }

        // TODO: Implement paging
        private Users Next(int page = 1, int perPage = 50, OrderBy orderBy = OrderBy.Dsc, string sortBy = UserSortBy.created_at)
        {
            return null;
        }
        #endregion
    }
}