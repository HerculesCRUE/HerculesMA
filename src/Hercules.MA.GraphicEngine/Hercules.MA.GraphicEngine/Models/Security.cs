﻿using Gnoss.ApiWrapper;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Hercules.MA.GraphicEngine.Models
{
    public static class Security
    {
        static readonly UserApi mUserApi = new ($@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}ConfigOAuth{Path.DirectorySeparatorChar}OAuthV3.config");

        public static bool CheckUser(Guid pUserId, HttpRequest pHttpRequest)
        {
            if (pUserId == Guid.Empty)
            {
                return false;
            }
            string cookie = pHttpRequest.Cookies["_UsuarioActual"];
            Guid userIdCookie = Guid.Empty;
            try
            {
                userIdCookie = mUserApi.GetUserIDFromCookie(cookie);
            }
            catch (Exception ex)
            {
                mUserApi.Log.Error(ex.Message);
            }
            return userIdCookie == pUserId;
        }

        public static bool CheckUsers(List<Guid> pUsersId, HttpRequest pHttpRequest)
        {
            if (pUsersId == null || pUsersId.Count == 0)
            {
                return false;
            }
            string cookie = pHttpRequest.Cookies["_UsuarioActual"];
            Guid userIdCookie = Guid.Empty;
            try
            {
                userIdCookie = mUserApi.GetUserIDFromCookie(cookie);
            }
            catch (Exception ex)
            {
                mUserApi.Log.Error(ex.Message);
            }
            return pUsersId.Contains(userIdCookie);
        }
    }
}
