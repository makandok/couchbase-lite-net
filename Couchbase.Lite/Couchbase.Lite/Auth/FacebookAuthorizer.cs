/**
 * Couchbase Lite for .NET
 *
 * Original iOS version by Jens Alfke
 * Android Port by Marty Schoch, Traun Leyden
 * C# Port by Zack Gramana
 *
 * Copyright (c) 2012, 2013 Couchbase, Inc. All rights reserved.
 * Portions (c) 2013 Xamarin, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the
 * License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
 * either express or implied. See the License for the specific language governing permissions
 * and limitations under the License.
 */

using System;
using System.Collections.Generic;
using Couchbase.Lite;
using Couchbase.Lite.Auth;
using Couchbase.Lite.Util;
using Sharpen;

namespace Couchbase.Lite.Auth
{
	public class FacebookAuthorizer : Authorizer
	{
		public const string LoginParameterAccessToken = "access_token";

		public const string QueryParameter = "facebookAccessToken";

		public const string QueryParameterEmail = "email";

		private static IDictionary<IList<string>, string> accessTokens;

		private string emailAddress;

		public FacebookAuthorizer(string emailAddress)
		{
			this.emailAddress = emailAddress;
		}

        public override bool UsesCookieBasedLogin {
            get { return true; }
        }

		public override IDictionary<string, string> LoginParametersForSite(Uri site)
		{
			IDictionary<string, string> loginParameters = new Dictionary<string, string>();
			string accessToken = AccessTokenForEmailAndSite(this.emailAddress, site);
			if (accessToken != null)
			{
				loginParameters.Put(LoginParameterAccessToken, accessToken);
				return loginParameters;
			}
			else
			{
				return null;
			}
		}

		public override string LoginPathForSite(Uri site)
		{
			return "/_facebook";
		}

		public static string RegisterAccessToken(string accessToken, string email, string
			 origin)
		{
			lock (typeof(FacebookAuthorizer))
			{
				IList<string> key = new AList<string>();
				key.AddItem(email);
				key.AddItem(origin);
				if (accessTokens == null)
				{
					accessTokens = new Dictionary<IList<string>, string>();
				}
				Log.D(Database.Tag, "FacebookAuthorizer registering key: " + key);
				accessTokens.Put(key, accessToken);
				return email;
			}
		}

		public static string AccessTokenForEmailAndSite(string email, Uri site)
		{
			IList<string> key = new AList<string>();
			key.AddItem(email);
            key.AddItem(site.ToString().ToLower());
			Log.D(Database.Tag, "FacebookAuthorizer looking up key: " + key + " from list of access tokens"
				);
			return accessTokens.Get(key);
		}
	}
}