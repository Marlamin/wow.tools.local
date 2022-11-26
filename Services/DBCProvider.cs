using CASCLib;
using DBCD.Providers;
using wow.tools.local.Controllers;
using System;
using System.IO;
using System.Net.Http;

namespace wow.tools.local.Services
{
    public class DBCProvider : IDBCProvider
    {
        public LocaleFlags localeFlags = LocaleFlags.All_WoW;

        public Stream StreamForTableName(string tableName, string build)
        {
            if (tableName.Contains("."))
                throw new Exception("Invalid DBC name!");

            if (string.IsNullOrEmpty(build))
                throw new Exception("No build given!");

            tableName = tableName.ToLower();

            var fullFileName = "dbfilesclient/" + tableName + ".db2";
            return CASC.GetFileByName(fullFileName);
        }
    }
}