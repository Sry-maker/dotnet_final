using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using WebApplication2.Repository;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "dish_order")]
    public class OrderController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static OrderController instance = new OrderController();
        /// <summary>对应类的实例</summary>
        public static OrderController Instance { get => instance; set => instance = value; }

        public static string generateId()
        {
            string order_id = "";
            int yr = DateTime.Now.Year;
            int mon = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            string date = yr.ToString().Substring(2, 2) + mon.ToString() + day.ToString();
            if (date.Length == 4)
            {
                date = date.Insert(2, "0");
                date = date.Insert(4, "0");
            }
            else if (date.Length == 5 && mon < 10)
            {
                date = date.Insert(2, "0");
            }
            else if (date.Length == 5 && day < 10)
            {
                date = date.Insert(4, "0");
            }
            order_id = date + "000000";
            using (var orderRepo = new OrderRepository())
            {
                string? max_id = orderRepo.Orders.
                    Where(p => p.order_id.StartsWith(date)).
                    Select(p => p.order_id).
                    Max();
                if (max_id != null)
                {
                    order_id = (long.Parse(max_id) + 1).ToString();
                }
            }
            return order_id;
        }

        public static bool existsUnpaidOrder(string customer_id)
        {
            using(var orderRepo = new OrderRepository())
            {
                if(orderRepo.Orders.Where(p => p.customer_id.Equals(customer_id)).OrderByDescending(p => p.order_id).Count() == 0)
                {
                    return false;
                }
                DishOrder order = orderRepo.Orders.Where(p => p.customer_id.Equals(customer_id))
                    .OrderByDescending(p => p.order_id)
                    .First();
                if (order != null)
                {
                    if (order.state != 0) return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 生成新订单
        /// </summary>
        /// <param name="customer_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Dictionary<string,object>), 200)]
        [HttpGet("{customer_id}")]
        public Dictionary<string,object> newOrder(string customer_id,string table_id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            

            bool success = !existsUnpaidOrder(customer_id);
            result.Add("success", success);
            if (!success) return null;
            string order_id = generateId();
            using(var orderRepo = new OrderRepository())
            {
                DishOrder order = new DishOrder();
                order.order_id = order_id;
                order.state = 0;
                order.table_id = table_id;
                order.order_date = DateTime.Now;
                order.customer_id = customer_id;
                orderRepo.Add(order);
                orderRepo.SaveChanges();
            }
            TableController.Instance.useTable(table_id);
            
            result.Add("order_id",order_id);
            return result;
        }

        
    }
}
