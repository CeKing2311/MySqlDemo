
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySqlDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 测试多线程
            //for (int i = 0; i < 20; i++)
            //{
            //    Thread thread = new Thread(showData);
            //    thread.Name = "my_thead" + i;
            //    thread.IsBackground = false;
            //    thread.Start();
            //    thread.Join(); 
            //}
            #endregion
            #region 多线程解决并发
            //testMuti();
            #endregion

            #region 写入mySql 数据
            for (int i = 0; i < 100000; i++)
            {
                string order_num = getUuid(4);
                string pay_order_num = getUuid(5);
                double order_amount = getRandom();
                int invoice = 1;
                string invoice_title = "";
                string order_remark = "测试订单"+order_num;
                string address_id = getUuid(6) ;
                string sql = string.Format("INSERT INTO t_order(order_num,pay_order_num,order_amount,invoice,invoice_title,order_remark,address_id) VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}');", order_num, pay_order_num, order_amount, invoice, invoice_title, order_remark, address_id);
                int count= MySqlHelper.ExecuteNonQuery(sql);
                Console.WriteLine("result:" + count);
            }
            #endregion
           

            Console.ReadKey();
            //tetsThreadState();
        }

        private static double getRandom()
        {
            double val = Math.Round(new Random().NextDouble() * 1000,2);
            return val;
        }

        private static string getUuid(int length)
        {
            string str= Guid.NewGuid().ToString().ToLower().Replace("-","");
            str =  str.Substring(0, length);
            return str;
        }

        private static void testMuti()
        {
            //控制并发线程的数量
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                int k = i;

                Console.WriteLine($"运行数量为**{tasks.Count(t => t.Status == TaskStatus.Running)}***");

                if (tasks.Count(t => t.Status == TaskStatus.Running) > 3)
                {
                    var runID = tasks.Count(t => t.Status == TaskStatus.Running);
                    Console.WriteLine($"运行数量为**{runID}，大于3个了，请稍等***");

                    Task.WaitAny(tasks.ToArray());
                    tasks = tasks.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                }
                else
                {
                    var taskItem = Task.Run(() =>
                    {
                        Console.WriteLine($"打开新线程**{k}  ***{Thread.CurrentThread.ManagedThreadId.ToString("00")}");
                        //处理固定的工作
                        Thread.Sleep(20000);
                    });
                    tasks.Add(taskItem);
                }
            }
        }
        private static void testSleep()
        {
            Thread.Sleep(0);                       // 释放CPU时间片
            Thread.Sleep(1000);                    // 休眠1000毫秒
            Thread.Sleep(TimeSpan.FromHours(1));  // 休眠1小时
            Thread.Sleep(Timeout.Infinite);        // 休眠直到中断
        }

        private static void tetsThreadState()
        {
            Console.WriteLine("开始测试线程1");
            // 初始化一个线程 thread1
            Thread thread1 = new Thread(Work1);
            // 这时状态：UnStarted
            PrintState(thread1);
            // 启动线程
            Console.WriteLine("现在启动线程");
            thread1.Start();
            // 这时状态：Running
            PrintState(thread1);
            // 让线程飞一会 3s
            Thread.Sleep(3 * 1000);
            // 让线程挂起
            Console.WriteLine("现在挂起线程");
            thread1.Suspend();
            // 给线程足够的时间来挂起，否则状态可能是SuspendRequested
            Thread.Sleep(1000);
            // 这时状态：Suspend
            PrintState(thread1);
            // 继续线程
            Console.WriteLine("现在继续线程");
            thread1.Resume();
            // 这时状态：Running
            PrintState(thread1);
            // 停止线程
            Console.WriteLine("现在停止线程");
            thread1.Abort();
            // 给线程足够的时间来终止，否则的话可能是AbortRequested
            Thread.Sleep(1000);
            // 这时状态：Stopped
            PrintState(thread1);
            Console.WriteLine("------------------------------");
            Console.WriteLine("开始测试线程2");
            // 初始化一个线程 thread2
            Thread thread2 = new Thread(Work2);
            // 这时状态：UnStarted
            PrintState(thread2);
            // 启动线程
            thread2.Start();
            Thread.Sleep(2 * 1000);
            // 这时状态：WaitSleepJoin
            PrintState(thread2);
            // 给线程足够的时间结束
            Thread.Sleep(10 * 1000);
            // 这时状态：Stopped
            PrintState(thread2);

            Console.ReadKey();
        }
        // 普通线程方法：一直在运行从未被超越
        private static void Work1()
        {
            Console.WriteLine("线程运行中...");
            // 模拟线程运行，但不改变线程状态
            // 采用忙等状态
            while (true) { }
        }

        // 文艺线程方法：运行10s就结束
        private static void Work2()
        {
            Console.WriteLine("线程开始睡眠：");
            // 睡眠10s
            Thread.Sleep(10 * 1000);
            Console.WriteLine("线程恢复运行");
        }

        // 打印线程的状态
        private static void PrintState(Thread thread)
        {
            Console.WriteLine("线程的状态是：{0}", thread.ThreadState.ToString());
        }
        private static void showData()
        {
            Console.WriteLine("开始执行当前函数：" + Thread.CurrentThread.Name);
            //Console.WriteLine("当前线程数：" + Thread.CurrentThread.ManagedThreadId.ToString("00"));
            Thread.Sleep(1000);
            string sql = "select * from t_admin";
            //DataTable table = MySqlHelper.ExecuteDataTable(sql);
            int totalCount = 0;
            DataTable table = MySqlHelper.getPager(out totalCount, "*", "t_admin", "", "id", 1, 10);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Console.WriteLine("当前线程:"+Thread.CurrentThread.Name+"账号：" + table.Rows[i]["login_acct"] + ";姓名：" + table.Rows[i]["user_name"] + ";邮箱：" + table.Rows[i]["email"]);
                Console.WriteLine(new DateTime().ToLocalTime());
            }
            Console.WriteLine("当前函数执行完成：" + Thread.CurrentThread.Name+"\n*********************************");
        }
    }
}
