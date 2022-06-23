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
using CliDll;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "OrderController")]
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
                    if (order.state != 1) return true;
                }
            }
            return false;
        }

        public static void updatePrice(string order_id)
        {
            DishOrder order = null;
            var orderRepo = new OrderRepository();
            var chooseRepo = new ChooseRepository();
            
            order = orderRepo.Orders.Find(order_id);
            
            if (order != null && order.state == 0)
            {
                var price = (from p in chooseRepo.Chooses.Where(p => p.order_id.Equals(order_id))
                 join a in chooseRepo.Dishes.Where(a => true)
                 on p.dish_id equals a.dish_id
                 //on p.dish_id equals a.dish_id into list
                 //from l in list.DefaultIfEmpty()
                 select new { p.num, a.price }).Sum(p=>p.price*p.num);

                order.price = price;
                orderRepo.SaveChanges();

                /*double sum = 0.0;
                Arith arith = new Arith();
                foreach (var choose in chooseRepo.Chooses.Where(p => p.order_id.Equals(order_id)).ToList())
                {
                    double? dish_price = chooseRepo.Dishes.Find(choose.dish_id).price;
                    double cost = arith.MulCli(dish_price.Value,choose.num.Value);
                    sum = arith.AddCli(sum, cost);
                }
                Console.WriteLine($"订单金额已更新，为{sum}元");
                Console.WriteLine($"订单金额已更新，为{price}元");*/
            }
        }

        public static void updateDishCount(string order_id)
        {
            using(var orderRepo = new OrderRepository())
            {
                if (orderRepo.Orders.Find(order_id) == null) return;
            }
            var chooseRepo = new ChooseRepository();
            var dishRepo = new DishRepository();
            foreach(var choose in chooseRepo.Chooses.Where(p => p.order_id.Equals(order_id)).ToList())
            {
                dishRepo.Dishes.Find(choose.dish_id).count += choose.num;
            }
            dishRepo.SaveChanges();
        }

        /// <summary>
        /// 获取未支付的订单
        /// </summary>
        /// <param name="customer_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DishOrder), 200)]
        [HttpGet("{customer_id}")]
        public Dictionary<string,object> getTemporaryOrder(string customer_id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (CustomerController.getCustomer(customer_id) == null) return null;
            DishOrder order = null;
            using (var orderRepo = new OrderRepository())
            {
                order = orderRepo.Orders.Where(p => p.customer_id.Equals(customer_id))
                    .OrderByDescending(p => p.order_id)
                    .First();
            }
            if(order!=null && order.state == 0)
            {
                result.Add("order_id",order.order_id);
                result.Add("table_id", order.table_id);
                result.Add("order_date", order.order_date.ToString().Replace(" 0:00:00",""));
                result.Add("expense", order.price);
                result.Add("customer_id", customer_id);
                var chosen_dish = new List<Dictionary<string, object>>();
                
                using (var chooseRepo = new ChooseRepository()) 
                {
                    var chooses = chooseRepo.Chooses.Where(p => p.order_id.Equals(order.order_id)).ToList();
                    foreach (var choose in chooses)
                    {
                        bool hasKey = false;
                        var element = new Dictionary<string, object>();
                        foreach (var element1 in chosen_dish)
                        {
                            if (element1["dish_id"].Equals(choose.dish_id))
                            {
                                hasKey = true;
                                element = element1;
                                break;
                            }
                        }
                        if (!hasKey)
                        {
                            element.Add("dish_id", choose.dish_id);
                            element.Add("dish_num", choose.num);
                            Dish dish = DishController.Instance.getDish(choose.dish_id);
                            element.Add("dish_price", dish.price);
                            element.Add("dish_name", dish.name);
                            element.Add("order_date", choose.order_date);
                            chosen_dish.Add(element);
                        }
                        else
                        {
                            element["dish_num"] = (int)element["dish_num"] + choose.num;
                            element["order_date"] = choose.order_date; 
                        }
                    }
                }
                result.Add("chosen_dish", chosen_dish);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 计算订单金额
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        [HttpGet]
        public double getPriceOfOrder(string order_id)
        {
            double sum = 0.0;
            DishOrder order = null;
            var orderRepo = new OrderRepository();
            var chooseRepo = new ChooseRepository();

            order = orderRepo.Orders.Find(order_id);

            if (order != null)
            {        
                Arith arith = new Arith();
                foreach (var choose in chooseRepo.Chooses.Where(p => p.order_id.Equals(order_id)).ToList())
                {
                    double dish_price = chooseRepo.Dishes.Find(choose.dish_id).price.Value;
                    int dish_num = choose.num.Value;
                    double cost = arith.MulCli(dish_price,dish_num);
                    sum = arith.AddCli(sum, cost);
                    //double cost = dish_price* dish_num;
                    //sum += cost;
                }
                Console.WriteLine($"订单金额已更新，为{sum}元");
            }
            return sum;
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

        /// <summary>
        /// 暂存订单
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="chooses"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DishOrder), 200)]
        [HttpPost]
        public bool saveOrder(string order_id, List<Choose> chooses)
        {
            DishOrder order = null;
            using(var orderRepo = new OrderRepository())
            {
                order = orderRepo.Orders.Find(order_id);
            }
            if(order==null||order.state!=0)
            {
                return false;
            }
            double? price = 0.0;
            using(var chooseRepo = new ChooseRepository())
            {
                DateTime now = DateTime.Now;
                foreach (var choose in chooses)
                {
                    if (choose.num <= 0) continue;
                    Choose choose1 = new Choose();
                    choose1.dish_id = choose.dish_id;
                    choose1.order_date = now;
                    choose1.order_id = order_id;
                    choose1.num = choose.num;
                    chooseRepo.Add(choose1);
                }
                chooseRepo.SaveChanges();

                //price = chooseRepo.Chooses.Where(p => p.order_id.Equals(order_id)).Sum(p => p.num * DishController.Instance.getDish(p.dish_id).price);
            }
            updatePrice(order_id);
            return true;
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="credit"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(DishOrder), 200)]
        [HttpPost]
        public bool payOrder(string order_id, double credit)
        {
            var orderRepo = new OrderRepository();
            DishOrder order = null;
            order = orderRepo.Orders.Find(order_id);
            if(order == null || order.state == 1)
            {
                return false;
            }
            updatePrice(order_id);
            string customer_id = order.customer_id;
            double? credit_used = credit;
            using(var customerRepo = new CustomerRepository())
            {
                Customer customer = customerRepo.Customers.Find(customer_id);
                if (customer.credit<0)
                {
                    customer.credit = 0;
                }
                if (customer.credit < credit_used) credit_used = customer.credit;
                if (credit_used / 2 > order.price / 2) credit_used = order.price;
                order.price = order.price - credit_used / 2;
                customer.credit += (int)(order.price / 10) - credit_used;
                customerRepo.SaveChanges();
            }
            order.state = 1;
            orderRepo.SaveChanges();
            updateDishCount(order_id); 
            TableController.Instance.releaseTable(order.table_id);
            return true;
        }
    }
}
