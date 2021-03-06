﻿using Com.ACBC.Framework.Database;
using System;

namespace API_SERVER.Dao
{
    public class DBManagerEshop : IType
    {
        private DBType dbt;
        private string str = "";

        public DBManagerEshop()
        {
            var url = System.Environment.GetEnvironmentVariable("MysqlDBUrl");
            var uid = System.Environment.GetEnvironmentVariable("MysqlDBUser");
            var port = System.Environment.GetEnvironmentVariable("MysqlDBPort");
            var passd = System.Environment.GetEnvironmentVariable("MysqlDBPassword");

            this.str = "Server=" + url
                     + ";Port=" + port
                     + ";Database=new-eshop;Uid=" + uid
                     + ";Pwd=" + passd
                     + ";CharSet=utf8; Encrypt=false;";
            Console.Write(this.str);
            this.dbt = DBType.Mysql;
        }

        public DBManagerEshop(DBType d, string s)
        {
            this.dbt = d;
            this.str = s;
        }

        public DBType getDBType()
        {
            return dbt;
        }

        public string getConnString()
        {
            return str;
        }

        public void setConnString(string s)
        {
            this.str = s;
        }
    }
}
