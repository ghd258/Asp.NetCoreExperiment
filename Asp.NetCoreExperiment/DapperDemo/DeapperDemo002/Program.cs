﻿using Dapper;
using Npgsql;
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Collections.Generic;
namespace DeapperDemo002
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Test5();
        }

        static void Test5()
        {
            var connString = "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgres2018;Database=abc;";
            using (var conn = new NpgsqlConnection(connString))
            {

                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    conn.Execute("insert into test3(a) values('abcdef')",transaction:tran);
                    conn.Execute("insert into test4(name) values('abcdef')", transaction: tran);
                    tran.Rollback();
                    // tran.Commit();
                }
            }

        }
        static void Test4()
        {
            /*  get_seq函数
             CREATE OR REPLACE FUNCTION get_seq (seq_name TEXT)
RETURNS SETOF bigint as
$$
BEGIN
RETURN QUERY  EXECUTE  'select last_value from  ' || seq_name;
END;
$$ LANGUAGE plpgsql;
             
             */
            var connString = "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgres2018;Database=abc;";
            using (var conn = new NpgsqlConnection(connString))
            {
                //insert into tablename(主键，字段1，字段2) values(@主键，@字段1，@字段2) on conflict(主键) do update set 字段1=@字段1，字段2=@字段2
                var sql = @"INSERT INTO test3 (
	id
	,a
	,b
	,c
	,d
	)
values (
	case 
		when @id >= (
				select get_seq(pg_get_serial_sequence('test3', 'id'))
				) and @id not in(select id from test3)
			then (
					select nextval(pg_get_serial_sequence('test3', 'id'))
					)
		else @id
		end
	,@a
	,@b
	,@c
	,@d
	) on conflict(id) do

update
set a = @a
	,b = @b
	,c = @c
	,d = @d
where test3.id = @id";
                var result = conn.Execute(sql, new { id = 30, a = "aaaa", b = 123, c = 12.3d, d = DateTimeOffset.Now });
                Console.WriteLine(result);
            }
        }

        static void Test3()
        {
            var connString = "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgres2018;Database=abc;";
            using (var conn = new NpgsqlConnection(connString))
            {
                var sql = @"select b,a,d,c from test3";
                var list = conn.Query<dynamic>(sql);
                var itemString = new StringBuilder();
                foreach (IDictionary<string, object> item in list)
                {
                    itemString.AppendJoin(',', item?.Keys);
                    itemString.AppendLine();
                    break;
                }
                foreach (IDictionary<string, object> item in list)
                {
                    itemString.AppendJoin(',', item?.Values);
                    itemString.AppendLine();
                }
                Console.WriteLine(itemString.ToString());
            }
        }

        static void Test2()
        {
            var connString = "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgres2018;Database=abc;";
            using (var conn = new NpgsqlConnection(connString))
            {

                #region 错误做法
                //var sql = @"select * from ""T_Test"" where  date(createtime)=date(@date)";
                //var date = DateTimeOffset.Parse("2019-09-03 09:27:00 +09:00");
                //var list = conn.Query<dynamic>(sql, new { date });
                #endregion

                #region 正确做法
                var sql = @"select * from ""T_Test"" where  createtime>=@beginTime and createtime<=@endtime";
                var beginTime = DateTimeOffset.Parse("2019-09-03 00:00:00");
                var endTime = DateTimeOffset.Parse("2019-09-03 23:59:59.9999994");
                var list = conn.Query<dynamic>(sql, new { beginTime, endTime });
                #endregion
                foreach (var item in list)
                {
                    Console.WriteLine($"ID:{item.id}  Name:{item.name}  Time:{item.createtime} ");
                }
            }
        }
        static void Test1()
        {
            //var file = System.IO.Directory.GetCurrentDirectory() + "/sql.txt";

            //var content = System.IO.File.ReadAllText(file).ToLower();

            //return;
            var connString = "Server=127.0.0.1;Port=5432;UserId=postgres;Password=postgre2018;Database=TestDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"INSERT INTO Roles(RoleName) VALUES (@rolename)";
                    cmd.Parameters.AddWithValue("rolename", "aaa");
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT id FROM Roles", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
            }
        }


    }
}
