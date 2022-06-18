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
    [ApiExplorerSettings(GroupName = "dining_table")]
    public class TableController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static TableController instance = new TableController();
        /// <summary>对应类的实例</summary>
        public static TableController Instance { get => instance; set => instance = value; }

        /// <summary>
        /// 使用餐桌
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(bool), 200)]
        [HttpPost]
        public bool useTable(string table_id)
        {
            using(var tableRepo = new TableRepository())
            {
                DiningTable table = null;
                if (tableRepo.Tables.Find(table_id) != null) table = tableRepo.Tables.Find(table_id);
                if (table != null && table.state != null && table.state.Equals("空闲"))
                {
                    table.state = "使用";
                    tableRepo.Update(table);
                    tableRepo.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 释放餐桌
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(bool), 200)]
        [HttpPost]
        public bool releaseTable(string table_id)
        {
            using (var tableRepo = new TableRepository())
            {
                DiningTable table = null;
                if (tableRepo.Tables.Find(table_id) != null) table = tableRepo.Tables.Find(table_id);
                if (table != null && table.state != null && table.state.Equals("使用"))
                {
                    table.state = "空闲";
                    tableRepo.Update(table);
                    tableRepo.SaveChanges();
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 返回所有餐桌信息
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<DiningTable>), 200)]
        [HttpGet]
        public List<DiningTable> getAllTables()
        {
            using(var tableRepo = new TableRepository())
            {
                return tableRepo.Tables.ToList();
            }
        }


        /// <summary>
        /// 返回所有空闲的非预约用餐桌
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpGet]
        public ActionResult<List<DiningTable>> getAllEmptyTable()
        {
            using (var tableRepo = new TableRepository())
            {
                return tableRepo.Tables.Where(p=>p.state.Equals("空闲")&&p.table_id.StartsWith("c")).ToList();
            }
        }
    }
}
