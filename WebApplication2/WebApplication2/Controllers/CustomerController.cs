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
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static CustomerController instance = new CustomerController();
        /// <summary>对应类的实例</summary>
        public static CustomerController Instance { get => instance; set => instance = value; }

        /// <summary>
        /// 返回指定用户信息
        /// </summary>
        /// <param name="customer_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Customer), 200)]
        [HttpGet("{customer_id}")]
        public Customer getCustomer(string customer_id)
        {
            using(var customerRepo=new CustomerRepository())
            {
                Customer customer = customerRepo.Customers.Find(customer_id);
                return customer;
            }
        }
    }
}
